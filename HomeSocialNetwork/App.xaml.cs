using System.Configuration;
using System.Data;
using System.Windows;

namespace HomeSocialNetwork
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Получаем пути
            string projectRoot = Helpers.PathBaseFiles.ProjectRoot;
            string dbPath = Helpers.PathBaseFiles.DatabasePath;

            // Выводим в консоль для отладки
            System.Diagnostics.Debug.WriteLine($"ProjectRoot: {projectRoot}");
            System.Diagnostics.Debug.WriteLine($"DatabasePath: {dbPath}");

            // Создаём папку Data/DB/, если её нет
            Helpers.PathBaseFiles.EnsureDatabaseDirectoryExists();

            // ЗАПУСКАЕМ ГЛАВНОЕ ОКНО
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

    }

}
