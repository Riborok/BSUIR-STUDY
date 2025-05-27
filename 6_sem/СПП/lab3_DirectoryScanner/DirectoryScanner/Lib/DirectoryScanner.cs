using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace DirectoryScanner.Lib;

public class DirectoryScanner(int maxThreads) {
    public DirectoryScanner() : this(Environment.ProcessorCount) { }

    public async Task<DirectoryNode> ScanAsync(string rootPath, CancellationToken cancellationToken)
    {
        var rootNode = new DirectoryNode
        {
            Path = rootPath,
            Name = Path.GetFileName(rootPath),
            IsDirectory = true
        };

        try {
            var activeTasks = 1;
            var tcs = new TaskCompletionSource<bool>();
            using var registration = cancellationToken.Register(() => { tcs.TrySetCanceled(cancellationToken); });
            
            using var semaphore = new SemaphoreSlim(maxThreads);
            _ = ProcessDirectoryAsync(
                rootNode, semaphore, cancellationToken,
                onTaskCompleted: () =>
                {
                    if (Interlocked.Decrement(ref activeTasks) == 0)
                        tcs.TrySetResult(true);
                },
                onChildTaskScheduled: () =>
                {
                    Interlocked.Increment(ref activeTasks);
                }
            );

            await tcs.Task;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine(@"OperationCanceledException");
        }

        CalculateDirectorySizes(rootNode);
        CalculatePercentages(rootNode, rootNode.Size);
        return rootNode;
    }

    private async Task ProcessDirectoryAsync(
        DirectoryNode node,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken,
        Action onTaskCompleted,
        Action onChildTaskScheduled)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Console.WriteLine($@"Thread ID: {Thread.CurrentThread.ManagedThreadId}");
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            foreach (var file in Directory.GetFiles(node.Path))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fileInfo = new FileInfo(file);
                if ((fileInfo.Attributes & FileAttributes.ReparsePoint) != 0)
                    continue;

                var fileNode = new DirectoryNode
                {
                    Path = fileInfo.FullName,
                    Name = fileInfo.Name,
                    Size = fileInfo.Length,
                    IsDirectory = false
                };
                node.Children.Add(fileNode);
            }

            foreach (var dir in Directory.GetDirectories(node.Path))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var di = new DirectoryInfo(dir);
                if ((di.Attributes & FileAttributes.ReparsePoint) != 0)
                    continue;

                var childNode = new DirectoryNode
                {
                    Path = di.FullName,
                    Name = di.Name,
                    IsDirectory = true
                };
                node.Children.Add(childNode);

                onChildTaskScheduled();
                _ = ProcessDirectoryAsync(childNode, semaphore, cancellationToken, onTaskCompleted, onChildTaskScheduled);
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            semaphore.Release();
            onTaskCompleted();
        }
    }

    private long CalculateDirectorySizes(DirectoryNode node)
    {
        if (node.IsDirectory) {
            foreach (var child in node.Children)
                node.Size += CalculateDirectorySizes(child);
        }
        return node.Size;
    }

    private void CalculatePercentages(DirectoryNode node, long parentTotal)
    {
        if (parentTotal > 0) {
            node.Percentage = (double)node.Size * 100 / parentTotal;
        }

        foreach (var child in node.Children)
        {
            CalculatePercentages(child, node.Size);
        }
    }
}