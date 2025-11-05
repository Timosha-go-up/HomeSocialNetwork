using SQLitePCL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HomeSocialNetwork
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

        private void CloseLogWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (_logWindow != null)
            {
                _logWindow.Close();  // Закроет окно _logWindow
            }




        }
    }
}