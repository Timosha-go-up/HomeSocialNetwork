using System.Diagnostics;
using System.Windows;



namespace WpfHomeNet
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {                 
        private LogWindow _logWindow;
        public LogWindow()
        {
            InitializeComponent();
            Debug.WriteLine($"TextBox.ActualWidth: {LogTextBox.ActualWidth}");
            Debug.WriteLine($"TextBox.ActualHeight: {LogTextBox.ActualHeight}");
            Debug.WriteLine($"TextBox.Visibility: {LogTextBox.Visibility}");
        }

        public async void AddLog(string message)
        {                                   
             LogTextBox.Text +=message + "\n"; LogTextBox.ScrollToEnd();
        }

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (char c in "Привет")
        //    {
        //        o.Text += c;
        //       o.ScrollToEnd();
        //        await Task.Delay(100);  // Ждёт 100 мс, но не блокирует UI
        //    }
        //    o.Text += "\n";

        //    foreach (char c in "еще раз привет")
        //    {
        //        o.Text += c;
        //        o.ScrollToEnd();
        //        await Task.Delay(100);  // Ждёт 100 мс, но не блокирует UI
        //    }
        //    o.Text += "\n";
        //}

       
    }
}