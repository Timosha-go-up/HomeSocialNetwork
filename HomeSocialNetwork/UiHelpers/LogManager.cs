using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HomeSocialNetwork.UiHelpers
{
    public class LogManager
    {
       
        private readonly System.Media.SoundPlayer _soundPlayer;
        private readonly ConcurrentQueue<string> _logQueue = new();
        private bool _isProcessing;
        private LogWindow _logWindow;
        private string _currentAnimation = "";

        public LogManager(LogWindow logWindow)
        {
            _logWindow = logWindow ?? throw new ArgumentNullException(nameof(logWindow));
           

        }



        public void WriteLog(string message)
        {
            // Очищаем от лишних переносов
            message = message.Trim('\r', '\n');
            _logQueue.Enqueue(message);

            if (!_isProcessing)
                ProcessLogQueue();
        }

        private async void ProcessLogQueue()
        {
            if (_isProcessing) return;
            _isProcessing = true;

            try
            {
                while (_logQueue.TryDequeue(out string? message))
                {
                    // 1. Анимация по символам
                    _currentAnimation = "";
                    foreach (char c in message)
                    {
                        _currentAnimation += c;
                        await UpdateLogDisplayAsync(_currentAnimation, true);

                        

                        await Task.Delay(60);
                    }

                    // 2. Добавляем перенос строки (только один раз!)
                    await UpdateLogDisplayAsync("\n", false);
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task UpdateLogDisplayAsync(string text, bool isAnimating)
        {
            await _logWindow.Dispatcher.InvokeAsync(() =>
            {
                if (isAnimating)
                {
                    // Режим анимации: заменяем последнюю строку
                    var lines = _logWindow.LogTextBox.Text
                        .Split(new[] { '\n' }, StringSplitOptions.None);

                    if (lines.Length > 0)
                    {
                        lines[lines.Length - 1] = text;
                        _logWindow.LogTextBox.Text = string.Join("\n", lines);
                    }
                    else
                    {
                        _logWindow.LogTextBox.Text = text;
                    }
                }
                else
                {
                    // Финальный перенос строки
                    _logWindow.LogTextBox.Text += text;
                }

                _logWindow.LogTextBox.ScrollToEnd();
            }, DispatcherPriority.Background);
        }
    }







}
