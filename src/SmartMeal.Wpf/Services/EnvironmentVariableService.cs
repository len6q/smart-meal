using Microsoft.Extensions.Logging;

namespace SmartMeal.Wpf.Services;

public interface IEnvironmentVariableService
{
    string? GetValue(string name);
    void SetValue(string name, string value);
    IReadOnlyList<string> GetConfiguredVariableNames();
}

public sealed class EnvironmentVariableService : IEnvironmentVariableService
{
    private readonly IReadOnlyList<string> _variableNames;
    private readonly ILogger<EnvironmentVariableService> _logger;

    public EnvironmentVariableService(
        IReadOnlyList<string> variableNames,
        ILogger<EnvironmentVariableService> logger)
    {
        _variableNames = variableNames;
        _logger = logger;
    }

    public string? GetValue(string name)
    {
        var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);

        if (value is null)
        {
            Environment.SetEnvironmentVariable(name, string.Empty, EnvironmentVariableTarget.User);
            return string.Empty;
        }

        return value;
    }

    public void SetValue(string name, string value)
    {
        var oldValue = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User) ?? string.Empty;

        Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.User);

        _logger.LogInformation("Variable '{Name}' changed from '{OldValue}' to '{NewValue}'", name, oldValue, value);
    }

    public IReadOnlyList<string> GetConfiguredVariableNames()
    {
        return _variableNames;
    }
}
