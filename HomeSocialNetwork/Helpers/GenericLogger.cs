using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HomeSocialNetwork.Helpers
{
    public class GenericLogger : ILogger
    {
        private readonly Action<string> _output;

        public GenericLogger(Action<string> output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }
        public void Log(
      LogLevel level,
      string message,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string filePath = "",
      [CallerLineNumber] int lineNumber = 0)
        {
            // Извлекаем имя класса из пути к файлу
            string className = string.IsNullOrEmpty(filePath)
                ? "UnknownClass"
                : Path.GetFileNameWithoutExtension(filePath)
                     .Split('.').Last() ?? "Unknown";

            // Очищаем имена (удаляем лишние символы, если нужно)
            className = CleanName(className);
            memberName = CleanName(memberName);

            // Формируем запись лога
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var levelStr = level.ToString().ToUpper();
            var logEntry = $"[{timestamp}] [{levelStr}] {className}.{memberName}:{lineNumber} | {message}";

            _output(logEntry);
        }






        private string CleanName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Unknown";

            // Удаляем компиляторные артефакты (например, "<>c__DisplayClass...")
            int startIndex = name.IndexOf('<');
            if (startIndex >= 0)
            {
                int endIndex = name.IndexOf('>');
                if (endIndex > startIndex)
                {
                    name = name.Substring(endIndex + 1);
                }
            }

            // Удаляем лишние символы (если нужно)
            name = name.Replace("<", "").Replace(">", "").Trim();

            return string.IsNullOrEmpty(name) ? "Unknown" : name;
        }

    }

}
