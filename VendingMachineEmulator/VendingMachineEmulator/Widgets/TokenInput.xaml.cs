using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public partial class TokenInput : UserControl
    {
        public TokenInput()
        {
            InitializeComponent();
            ApplyButton.Click += OnLoginPressed;
        }

        private void OnLoginPressed(object sender, RoutedEventArgs args)
        {
            string token = TokenField.Text;
            ApplyButton.IsEnabled = false;
            
            AsyncHelper.FireAndForget( 
                () => TryLogin(token),
                ex =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        MainWindow.console.LogError($"Login failed: {ex.Message}");
                        ApplyButton.IsEnabled = true;
                        ResultLabel.Text = "Неверный токен!";
                        ResultLabel.Visibility = Visibility.Visible;
                    });
                });
        }

        private async Task TryLogin(string token)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{MainWindow.backEndUrl}/api/Machine/Get");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                VendingMachine machine = await MainWindow.client.SendAsyncAsJson<VendingMachine>(request);
                if (machine.MachineId != 0 && machine.EmployeeId != 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MainWindow.instance.SetMachineToken(token);
                        Hide();
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        ApplyButton.IsEnabled = true;
                        ResultLabel.Text = "Данные автомата неверны!";
                        ResultLabel.Visibility = Visibility.Visible;
                    });  
                }

            }
            catch (HttpRequestException e)
            {
                MainWindow.console.LogError($"Error getting response: {e.Message}");
                Dispatcher.Invoke(() =>
                {
                    ApplyButton.IsEnabled = true;
                    ResultLabel.Text = "Неверный токен!";
                    ResultLabel.Visibility = Visibility.Visible;
                });
            }
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
            ResultLabel.Visibility = Visibility.Hidden;
        }
        
        

        public void Show() 
        {
            Visibility = Visibility.Visible;
            ApplyButton.IsEnabled = true;
            ResultLabel.Visibility = Visibility.Hidden;
        }
    }
}
