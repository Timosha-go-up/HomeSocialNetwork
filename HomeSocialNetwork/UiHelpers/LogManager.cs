using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HomeSocialNetwork.UiHelpers
{
    public class LogManager : IDisposable
    {
        public event Action? OnLogsHidden;
        private readonly ConcurrentQueue<string> _logQueue = new();
        private bool _isProcessing;
        private TextBlock _logDisplay;
        private DispatcherTimer _hideTimer;

        public LogManager(TextBlock logDisplay, TimeSpan hideDelay = default)
        {
            _logDisplay = logDisplay ?? throw new ArgumentNullException(nameof(logDisplay));

            var interval = hideDelay == default ? TimeSpan.FromSeconds(10) : hideDelay;
            _hideTimer = new DispatcherTimer { Interval = interval };

            _hideTimer.Tick += OnHideTimerTick;
        }








        public void WriteLog(string message)
        {
            _logQueue.Enqueue(message);
            
            if (!_isProcessing)
                ProcessLogQueue();           
        }

        private async void ProcessLogQueue(CancellationToken cancellationToken = default)
        {
            if (_isProcessing) return;
            _isProcessing = true;

            try
            {
                while (_logQueue.TryDequeue(out string? message))
                {
                    string fullText = message + "\n";

                    foreach (char c in fullText)
                    {
                        // Выводим каждый символ отдельно
                        await UpdateLogDisplayAsync(c.ToString(), cancellationToken);

                        // Задержка между символами (эффект «печатания»)
                        await Task.Delay(30, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException) { }

            finally
            {
                _isProcessing = false;
                StartHideTimer();
            }
        }

        private async Task UpdateLogDisplayAsync(string text, CancellationToken cancellationToken)
        {
            await _logDisplay.Dispatcher.InvokeAsync(() =>
            {
                // Ограничиваем длину текста (защита от переполнения)
                if (_logDisplay.Text.Length > 10_000)
                {
                    _logDisplay.Text = _logDisplay.Text.Substring(_logDisplay.Text.Length - 5_000);
                }

                _logDisplay.Text += text;
                if (_logDisplay.Parent is ScrollViewer sv) sv.ScrollToEnd();
            }, DispatcherPriority.Background);
        }


        private void StartHideTimer()
        {
            _hideTimer.Stop();
            _hideTimer.Start();
        }

        private void OnHideTimerTick(object? sender, EventArgs e)
        {
            _hideTimer.Stop();
            OnLogsHidden?.Invoke();
        }


        public void Dispose()
        {
            _hideTimer.Stop();
            _hideTimer.Tick -= OnHideTimerTick; // Отписываемся от события
            
        }

       
    }

}
