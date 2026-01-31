using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using SmartMeal.Wpf.Models;
using SmartMeal.Wpf.Services;

namespace SmartMeal.Wpf.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private readonly IEnvironmentVariableService _service;
    private readonly ILogger<MainViewModel> _logger;

    public ObservableCollection<EnvironmentVariableItem> Variables { get; }

    public ICommand SaveCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand MinimizeCommand { get; }

    public MainViewModel(
        IEnvironmentVariableService service,
        ILogger<MainViewModel> logger)
    {
        _service = service;
        _logger = logger;

        Variables = new ObservableCollection<EnvironmentVariableItem>();

        SaveCommand = new RelayCommand(ExecuteSave);
        CloseCommand = new RelayCommand(ExecuteClose);
        MinimizeCommand = new RelayCommand(ExecuteMinimize);

        LoadVariables();
    }

    private void LoadVariables()
    {
        var variableNames = _service.GetConfiguredVariableNames();

        for (var i = 0; i < variableNames.Count; i++)
        {
            var name = variableNames[i];
            var value = _service.GetValue(name) ?? string.Empty;

            var item = new EnvironmentVariableItem
            {
                Index = i + 1,
                Name = name,
                Value = value
            };

            item.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(EnvironmentVariableItem.Value) && sender is EnvironmentVariableItem changedItem)
                {
                    _service.SetValue(changedItem.Name, changedItem.Value);
                }
            };

            Variables.Add(item);
        }

        _logger.LogInformation("Загружено {Count} переменных окружения", variableNames.Count);
    }

    private void ExecuteSave()
    {
        foreach (var variable in Variables)
        {
            _service.SetValue(variable.Name, variable.Value);
        }

        _logger.LogInformation("Все переменные окружения сохранены");
    }

    private void ExecuteClose()
    {
        Application.Current.Shutdown();
    }

    private void ExecuteMinimize()
    {
        if (Application.Current.MainWindow != null)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
    }
}
