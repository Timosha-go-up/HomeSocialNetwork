using System.Runtime.CompilerServices;

namespace HomeSocialNetwork.Helpers
{
    public interface ILogger
    {
        void Log(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
    }

}