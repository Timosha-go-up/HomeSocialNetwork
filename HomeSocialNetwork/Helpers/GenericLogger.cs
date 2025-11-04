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
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            // Извлекаем имя класса
            string className = string.IsNullOrEmpty(filePath)
                ? "UnknownClass"
                : Path.GetFileNameWithoutExtension(filePath)?
                    .Split('.').Last() ?? "Unknown";

            // Очищаем имена
            className = CleanName(className);
            memberName = CleanName(memberName);

            // Формируем контекст
            string context = $"{className}.{memberName}:{lineNumber}";

            // Собираем запись
            string entry = $"[{DateTime.Now:HH:mm:ss.fff}] [{context}] {message}";

            _output(entry);
        }

        private string CleanName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Unknown";
            if (name.Contains("<>")) name = name.Split('<')[0];
            return name.Trim();
        }
    }

}
