namespace GeoLocator.Core.Interfaces;

/// <summary>
/// Eliminates the need to directly depend on ASP.NET Core logging types
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IAppLogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
}
