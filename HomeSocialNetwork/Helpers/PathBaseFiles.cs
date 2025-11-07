using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
public static class PathBaseFiles
{
    private static string? _projectRoot;
    private static string? _fallbackDbPath; // Резервный путь к БД


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
            _projectRoot = FindProjectRoot(startDir);

            if (_projectRoot == null)
                throw new DirectoryNotFoundException(
                    $"Не удалось найти корень проекта. Начальная папка: {startDir}");

            return _projectRoot;
        }
    }

    /// <summary>
    /// Путь к папке Data/DB/ в проекте
    /// </summary>
    public static string DatabaseDirectory
    {
        get { return Path.Combine(ProjectRoot, "Data", "DB"); }
    }

    /// <summary>
    /// Полный путь к users.db в проекте (основной путь)
    /// </summary>
    public static string DatabasePath
    {
        get { return Path.Combine(DatabaseDirectory, "users.db"); }
    }

    /// <summary>
    /// Резервный путь к БД (рядом с .exe)
    /// </summary>
    public static string FallbackDatabasePath
    {
        get
        {
            if (_fallbackDbPath == null)
            {
                var exeDir = AppDomain.CurrentDomain.BaseDirectory;
                _fallbackDbPath = Path.Combine(exeDir, "LocalDatabase.db");
            }
            return _fallbackDbPath;
        }
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

    /// <summary>
    /// Проверяет доступность основного пути к БД и при ошибке возвращает резервный
    /// </summary>
    public static string GetValidDatabasePath()
    {
        // 1. Проверяем основной путь
        if (IsPathValidForDatabase(DatabasePath))
            return DatabasePath;

        // 2. Если основной недоступен — используем резервный
        Debug.WriteLine($"Основной путь недоступен. Используем резервный: {FallbackDatabasePath}");

        // Создаём пустую БД в резервном месте
        if (!File.Exists(FallbackDatabasePath))
        {
            File.WriteAllText(FallbackDatabasePath, string.Empty);
            Debug.WriteLine($"Создана резервная БД: {FallbackDatabasePath}");
        }

        return FallbackDatabasePath;
    }

    /// <summary>
    /// Проверяет, можно ли использовать путь для БД
    /// </summary>
    private static bool IsPathValidForDatabase(string path)
    {
        try
        {
            // Проверка формата пути
            Path.GetFullPath(path);

            // Проверка директории
            var dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                return false;

            // Проверка прав записи
            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                fs.Close();
            }

            return true;
        }
        catch
        {
            return false;
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
            if (Directory.GetFiles(currentDir, "*.csproj").Length > 0)
                return currentDir;

            var parentDir = Directory.GetParent(currentDir)?.FullName;
            if (parentDir == null || parentDir == currentDir)
                break;

            currentDir = parentDir;
        }

        return null;
    }

    #endregion
}

