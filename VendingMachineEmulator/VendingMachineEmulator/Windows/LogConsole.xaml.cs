using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace VendingMachineEmulator
{
    public partial class LogConsole : Window
    {
        public LogConsole() {}

        public LogConsole(MainWindow mainwindow)
        {
            InitializeComponent();

            Left = mainwindow.Left;
            Top = mainwindow.Top;
            Show();
        }

        public void Log(string message)
        {
            if (!message.EndsWith("\n"))
            {
                message += "\n";
            }
            
            AddMessage(message, Colors.White);
        }
        
        public void LogError(string message)
        {
            if (!message.EndsWith("\n"))
            {
                message += "\n";
            }
            AddMessage(message, Colors.Red);
        }
        
        public void Write(string message)
        {
            AddMessage(message, Colors.White);
        }

        private void AddMessage(string message, Color color)
        {
            if (Dispatcher.CheckAccess())
            {
                OutputBlock.Inlines.Add(message);
                OutputBlock.Inlines.Last().Foreground = new SolidColorBrush(color);
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    OutputBlock.Inlines.Add(message);
                    OutputBlock.Inlines.Last().Foreground = new SolidColorBrush(color);
                });
            }
        }

// Hide Window instead of Exiting
        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
            base.OnClosing(e);
        }
    }
}