using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;

namespace DirectoryScanner.Lib;

public class DirectoryNode : INotifyPropertyChanged
{
    public string Path { get; set; }

    public string Name { get; set; }

    private long _size;
    public long Size 
    { 
        get => _size; 
        set { _size = value; OnPropertyChanged(nameof(Size)); } 
    }

    private double _percentage;
    public double Percentage 
    { 
        get => _percentage; 
        set { _percentage = value; OnPropertyChanged(nameof(Percentage)); } 
    }

    public bool IsDirectory { get; set; }

    public ObservableCollection<DirectoryNode> Children { get; set; } = [];

    public string Icon => IsDirectory ? "📁" : "📄";

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}