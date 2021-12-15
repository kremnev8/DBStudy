using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AsyncBridge;
using VendingMachineEmulator.Models;
using VendingMachineEmulator.Util;

namespace VendingMachineEmulator
{
    public partial class LoginForm : UserControl
    {
        public LoginForm()
        {
            InitializeComponent();

            CloseButton.Click += OnClose;
            LoginButton.Click += OnLoginPressed;
            Hide();
        }

        private void OnClose(object sender, RoutedEventArgs args)
        {
            Visibility = Visibility.Hidden;
        }

        private void OnLoginPressed(object sender, RoutedEventArgs args)
        {
            string email = EmailField.Text;
            string password = PasswordField.Password;
            LoginButton.IsEnabled = false;
            
            AsyncHelper.FireAndForget( 
                () => TryLogin(email, password),
                ex =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        MainWindow.console.LogError($"Login failed: {ex.Message}");
                        LoginButton.IsEnabled = true;
                        ResultLabel.Text = "Неверный логин или пароль!";
                        ResultLabel.Visibility = Visibility.Visible;
                    });
                });
        }

        private async Task TryLogin(string email, string password)
        {
            var data = new Dictionary<string, string>
            {
                {"grant_type", "password"}, 
                {"username", email},
                {"password", password}
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{MainWindow.backEndUrl}/api/token")
            {
                Content = new FormUrlEncodedContent(data)
            };
            TokenData token = await MainWindow.client.SendAsyncAsJson<TokenData>(request);

            Dispatcher.Invoke(() =>
            {
                MainWindow.SetEmployeeToken(token.access_token);
                LoginButton.IsEnabled = true;
            });
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
            ResultLabel.Visibility = Visibility.Hidden;
        }
        
        

        public void Show(string message = "") 
        {
            Visibility = Visibility.Visible;
            LoginButton.IsEnabled = true;
            if (message.Equals(""))
            {
                ResultLabel.Visibility = Visibility.Hidden;
            }
            else
            {
                ResultLabel.Text = message;
                ResultLabel.Visibility = Visibility.Visible;
            }
        }
    }
}
