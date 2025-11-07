using System.Runtime.CompilerServices;

namespace HomeSocialNetwork.Helpers
{


    public enum LogLevel { Debug, Information, Warning, Error, Critical }
   public interface ILogger
{
    void Log(
        LogLevel level,
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);


    // Обёртки с реализацией по умолчанию
    void LogDebug(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Debug, message, memberName, filePath, lineNumber);
    }

    void LogInformation(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Information, message, memberName, filePath, lineNumber);
    }

    void LogWarning(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Warning, message, memberName, filePath, lineNumber);
    }

    void LogError(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Error, message, memberName, filePath, lineNumber);
    }

    void LogCritical(
        string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Critical, message, memberName, filePath, lineNumber);
    }
}


}