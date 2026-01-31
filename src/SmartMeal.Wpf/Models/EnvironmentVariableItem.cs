using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartMeal.Wpf.Models;

public sealed class EnvironmentVariableItem : INotifyPropertyChanged
{
    private string _value = string.Empty;
    private string _comment = string.Empty;

    public required int Index { get; init; }
    public required string Name { get; init; }

    public string Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public string Comment
    {
        get => _comment;
        set => SetProperty(ref _comment, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
