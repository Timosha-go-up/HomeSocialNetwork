using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace HomeSocialNetwork.Helpers
{
    public static class PathBaseFiles
    {
        private static string? _projectRoot;

        /// <summary>
        /// Корень проекта (где лежит .csproj)
        /// </summary>
        public static string ProjectRoot
        {
            get
            {
                if (_projectRoot != null)
                    return _projectRoot;

                string startDir = AppContext.BaseDirectory;

                // Ищем .csproj, поднимаясь вверх
                _projectRoot = FindProjectRoot(startDir);
                if (_projectRoot == null)
                    throw new DirectoryNotFoundException(
                        $"Не удалось найти корень проекта. Начальная папка: {startDir}");

                return _projectRoot;
            }
        }

        /// <summary>
        /// Путь к папке Data/DB/
        /// </summary>
        public static string DatabaseDirectory
        {
            get
            {
                return Path.Combine(ProjectRoot, "Data", "DB");
            }
        }

        /// <summary>
        /// Полный путь к users.db
        /// </summary>
        public static string DatabasePath
        {
            get { return Path.Combine(DatabaseDirectory, "users.db"); }
        }

        /// <summary>
        /// Создаёт папку Data/DB/, если её нет
        /// </summary>
        public static void EnsureDatabaseDirectoryExists()
        {
            if (!Directory.Exists(DatabaseDirectory))
            {
                Directory.CreateDirectory(DatabaseDirectory);
                Debug.WriteLine($"Создана папка БД: {DatabaseDirectory}");
            }
        }

        #region Вспомогательные методы

        /// <summary>
        /// Ищет корень проекта (папку с .csproj) начиная с указанной директории
        /// </summary>
        private static string? FindProjectRoot(string startDirectory)
        {
            string currentDir = startDirectory;

            while (!string.IsNullOrEmpty(currentDir))
            {
                // Проверяем, есть ли .csproj в текущей папке
                if (Directory.GetFiles(currentDir, "*.csproj").Length > 0)
                    return currentDir;

                // Поднимаемся на уровень выше
                string? parentDir = Directory.GetParent(currentDir)?.FullName;
                if (parentDir == null || parentDir == currentDir)
                    break;

                currentDir = parentDir;
            }

            return null;
        }

        #endregion
    }
}
