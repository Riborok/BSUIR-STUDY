using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DirectoryScanner.Lib;
using System.Windows.Forms;

namespace DirectoryScanner.DirectoryScannerWpfApp;

public class MainViewModel : INotifyPropertyChanged
{
    private DirectoryNode _root;
    public DirectoryNode Root
    {
        get => _root;
        set { _root = value; OnPropertyChanged(nameof(Root)); }
    }

    private string _folderPath;
    public string FolderPath
    {
        get => _folderPath;
        set { _folderPath = value; OnPropertyChanged(nameof(FolderPath)); }
    }
    
    public ICommand ScanCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand BrowseCommand { get; }
        
    private CancellationTokenSource _cts;

    public MainViewModel()
    {
        ScanCommand = new RelayCommand(async param => await ScanAsync(param));
        CancelCommand = new RelayCommand(_ => CancelScan());
        BrowseCommand = new RelayCommand(param => BrowseFolder());
    }

    private async Task ScanAsync(object parameter)
    {
        if (parameter is not string folderPath || string.IsNullOrWhiteSpace(folderPath))
            return;

        _cts = new CancellationTokenSource();
        var scanner = new Lib.DirectoryScanner();
        Root = await Task.Run(async () => await scanner.ScanAsync(folderPath, _cts.Token));
    }

    private void CancelScan()
    {
        _cts?.Cancel();
    }
    
    private void BrowseFolder() {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = @"Выберите папку для сканирования";
        dialog.ShowNewFolderButton = false;
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            FolderPath = dialog.SelectedPath;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}