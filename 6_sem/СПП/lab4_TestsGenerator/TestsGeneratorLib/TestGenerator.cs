using System.Threading.Tasks.Dataflow;

namespace TestsGeneratorLib;

public static class TestGenerator {
    public static async Task GenerateAsync(List<string> filePaths, string savingPath, int maxParallelism)
    {
        var loadBlock = new TransformBlock<string, List<ClassInfo>>(
            async filePath => await CodeAnalyzer.GetClassesFromFileAsync(filePath),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxParallelism
            });
        
        var generateBlock = new TransformManyBlock<List<ClassInfo>, (string fileName, string content)>(
            classInfos =>
            {
                var result = new List<(string fileName, string content)>();
                
                foreach (var classInfo in classInfos)
                {
                    var code = TestCodeGenerator.GenerateTestClassCode(classInfo);
                    var safeNamespace = classInfo.Namespace.Replace(".", "_");
                    var fileName = $"{safeNamespace}_{classInfo.Name}Tests.cs";
                    result.Add((fileName, code));
                }
                
                return result;
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxParallelism
            });

        
        var saveBlock = new ActionBlock<(string fileName, string content)>(
            async item =>
            {
                var filePath = Path.Combine(savingPath, item.fileName);
                await using var writer = new StreamWriter(filePath);
                await writer.WriteAsync(item.content);
            },
            new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 }
        );
        
        loadBlock.LinkTo(generateBlock, new DataflowLinkOptions { PropagateCompletion = true });
        generateBlock.LinkTo(saveBlock, new DataflowLinkOptions { PropagateCompletion = true });
        
        foreach (var filePath in filePaths)
        {
            await loadBlock.SendAsync(filePath);
        }
        loadBlock.Complete();
        
        await saveBlock.Completion;
        Console.WriteLine("Сгенерировано.");
    }
}