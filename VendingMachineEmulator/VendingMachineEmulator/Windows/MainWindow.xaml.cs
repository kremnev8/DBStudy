using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AsyncBridge;
using Newtonsoft.Json;
using VendingMachineEmulator.Models;
using VendingMachineEmulator.Util;

namespace VendingMachineEmulator
{
    public enum MachineState
    {
        IDLE,
        INPUT,
        ERROR,
        PAYMENT,
        FISHISHED,
        WAITING,
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow instance;

        public static readonly HttpClient client = new HttpClient();
        public static LogConsole console;
        public static string backEndUrl = "http://localhost:5000";

        public static string machineToken;

        public static string employeeToken = "";
        public static bool isMaintainceActive = false;

        public static VendingMachine machineData = new VendingMachine();

        public List<DeliveryContents> contentsList = new List<DeliveryContents>();

        public MachineState state
        {
            get => _state;
            set
            {
                PayButton.IsEnabled = value == MachineState.PAYMENT;
                _state = value;
            }
        }


        public static int lastSelectedPosition;
        public static int lastGoodId;
        public static Good lastDespensedItem;
        private static MachineState _state;

        public MainWindow()
        {
            InitializeComponent();
            console = new LogConsole(this);

            Loaded += MainWindow_Loaded;
            Keypad.onInputChanged += UpdateDisplay;
            Keypad.onItemSelected += TryBuy;
            Keypad.onInvalidInput += () => { ShowError("Введите номер!"); };
            PayButton.Click += OnPaymentDone;
            TrayButton.Button.Click += ItemPickedUp;
            TrayButton.Visibility = Visibility.Hidden;

            OpenButton.Button.Click += OnOpenClicked;
            ExitButton.Button.Click += OnExitClicked;

            Items.onTrySelectGood += OnTrySelectItem;

            instance = this;
            EnableUI(false);
            
            /*SetMachineToken("A0Zk9RiWC2bsbsps3sJYFT2DMaIWnXcesKluI5OPBipX5OUmCSIAA9qxLiqqH8yW");
            
            CheckEmployeePermissions(new Employee()
            {
                EmployeeId = 2,
                PermissionId = (int)Permission.Admin + 1
            });*/
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        #region MachineDataLoad

        public void EnableUI(bool value)
        {
            Control.IsEnabled = value;
            Control.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            Items.IsEnabled = value;
            Items.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            OpenButton.IsEnabled = value;
            OpenButton.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            back.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            front.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            ControlBack.Visibility = value ? Visibility.Visible : Visibility.Hidden;
        }
        

        public void SetMachineToken(string token)
        {
            machineToken = token;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", machineToken);
            console.Log("Loaded successfully!");
            EnableUI(true);
            MachineTokenInput.Hide();
            AsyncHelper.FireAndForget(
                GetMachineData,
                ex => console.LogError(ex.Message));
            
        }
        

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           MachineTokenInput.Show();
        }

        public async Task GetMachineData()
        {
            VendingMachine machine = await client.GetJsonAsync<VendingMachine>($"{backEndUrl}/api/Machine/Get");
            SetVendingMachine(machine);
            await Items.UpdateItems();
            await Selector.LoadGoodsAsync();
        }


        public static void SetVendingMachine(VendingMachine newState)
        {
            instance.Dispatcher.Invoke(() =>
            {
                instance.state = MachineState.IDLE;
                machineData = newState;
            });
        }

        #endregion

        #region EmployeeLogin

        private void OnOpenClicked(object sender, RoutedEventArgs e)
        {
            if (employeeToken.Equals("") && !isMaintainceActive)
            {
                LoginForm.Show();
            }
        }

        private void OnExitClicked(object sender, RoutedEventArgs e)
        {
            if (contentsList.Count > 0)
            {
                ConfirmPopup.Popup("Применить изменения?", () =>
                {
                    employeeToken = "";
                    isMaintainceActive = false;
                    ExitButton.Visibility = Visibility.Hidden;
                    OpenButton.Visibility = Visibility.Visible;
                    contentsList.Clear();

                    AsyncHelper.FireAndForget(
                        GetMachineData,
                        ex => console.LogError(ex.Message));
                }, ApplyDelivery);
            }
            else
            {
                employeeToken = "";
                isMaintainceActive = false;
                ExitButton.Visibility = Visibility.Hidden;
                OpenButton.Visibility = Visibility.Visible;
            }
        }

        public static void SetEmployeeToken(string token)
        {
            employeeToken = token;
            AsyncHelper.FireAndForget(
                () => instance.GetEmployeePermissions(),
                ex => { console.LogError(ex.Message); });
        }

        private async Task GetEmployeePermissions()
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, $"{backEndUrl}/api/Employee/Get");
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", employeeToken);

            Employee employee = await client.SendAsyncAsJson<Employee>(message);
            Dispatcher.Invoke(() => { CheckEmployeePermissions(employee); });
        }

        private void CheckEmployeePermissions(Employee employee)
        {
            if (employee.getPermission() == Permission.Techician && employee.EmployeeId == machineData.EmployeeId
                || employee.getPermission() >= Permission.Admin)
            {
                isMaintainceActive = true;
                console.Log("Started maintance mode!");
                LoginForm.Hide();
                contentsList.Clear();
                ExitButton.Visibility = Visibility.Visible;
                OpenButton.Visibility = Visibility.Hidden;
            }
            else
            {
                employeeToken = "";
                LoginForm.Show("У вас недотстаточно прав!");
            }
        }

        #endregion

        #region Purchase

        private void UpdateDisplay(string currentInput)
        {
            if (state != MachineState.IDLE && state != MachineState.INPUT) return;

            if (currentInput.Equals(""))
            {
                state = MachineState.IDLE;
                Display.Text = "Пожалуйста выберите товар";
            }
            else
            {
                state = MachineState.INPUT;
                Display.Text = $"Выбран {currentInput}";
            }
        }

        private void ShowError(string error)
        {
            state = MachineState.IDLE;
            Display.Text = $"Ошибка: {error}";
        }

        private void TryBuy(int slotPosition)
        {
            if (machineData == null) return;
            if (state != MachineState.INPUT) return;

            bool hasItem = false;

            foreach (var slot in machineData.slots)
            {
                if (slot.SlotPosition != slotPosition) continue;
                if (slot.GoodId == 0 || slot.GoodCount <= 0) continue;

                hasItem = true;
                lastGoodId = slot.GoodId;
                break;
            }

            if (hasItem)
            {
                state = MachineState.PAYMENT;
                lastSelectedPosition = slotPosition;
                Display.Text = $"Выбран {slotPosition}. Внесите оплату.";
            }
            else
            {
                state = MachineState.ERROR;
                Display.Text = "Ошибка. Выбранный слот пуст";
                ResetLater(1000);
            }
        }

        private void ResetLater(int time = 5000)
        {
            DelayFactory.DelayAction(time, () =>
            {
                Dispatcher.Invoke(() =>
                {
                    state = MachineState.IDLE;
                    UpdateDisplay("");
                });
            });
        }


        private void OnPaymentDone(object sender, RoutedEventArgs e)
        {
            if (state != MachineState.PAYMENT) return;
            state = MachineState.FISHISHED;

            Purchase purchase = new Purchase()
            {
                SlotPosition = lastSelectedPosition,
                GoodId = lastGoodId,
                GoodCount = 1
            };

            AsyncHelper.FireAndForget( // Will handle exceptions by writing
                () => MakePurchase(purchase), // e.Message to the console
                ex =>
                {
                    state = MachineState.ERROR;
                    Display.Text = "Покупка не удалась!";
                    ResetLater(2000);
                    console.LogError(ex.Message);
                });
        }

        public void TransactionError()
        {
            state = MachineState.ERROR;
            Display.Text = "Покупка не удалась!";
            ResetLater();
        }

        private void TransactionSuccessful()
        {
            state = MachineState.WAITING;
            Display.Text = "Успех!";

            foreach (var slot in machineData.slots)
            {
                if (slot.SlotPosition != lastSelectedPosition) continue;

                slot.GoodCount--;
                Items.UpdateSlot(slot.SlotPosition, null, slot.GoodCount);

                break;
            }

            if (lastDespensedItem != null)
            {
                var fullFilePath = $"{backEndUrl}/Images/{lastDespensedItem.IconPath}";
                TrayButton.Icon.SetImage(fullFilePath);
            }

            TrayButton.Visibility = Visibility.Visible;
        }

        private void ItemPickedUp(object sender, RoutedEventArgs e)
        {
            TrayButton.Visibility = Visibility.Hidden;
            lastDespensedItem = null;
            ResetLater();
        }


        private async Task MakePurchase(Purchase purchase)
        {
            string json = JsonConvert.SerializeObject(purchase);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync($"{backEndUrl}/api/Purchase/Make", content);

            if (result.IsSuccessStatusCode)
            {
                try
                {
                    lastDespensedItem = await client.GetJsonAsync<Good>($"{backEndUrl}/api/Good/Detail?id={purchase.GoodId}");
                }
                catch (HttpRequestException e)
                {
                    console.LogError($"Failed loading icon for item id {purchase.GoodId}");
                }

                Dispatcher.Invoke(TransactionSuccessful);
            }
            else
            {
                Dispatcher.Invoke(TransactionError);
            }
        }

        #endregion

        #region Delivery

        private void OnTrySelectItem(Good good, int slotPosition, bool phantom)
        {
            if (!isMaintainceActive || state != MachineState.IDLE) return;

            if (good != null)
            {
                bool hasItem = false;

                foreach (var slot in machineData.slots)
                {
                    if (slot.SlotPosition != slotPosition) continue;

                    hasItem = true;

                    break;
                }

                foreach (var contents in contentsList)
                {
                    if (contents.SlotPosition != slotPosition) continue;

                    Selector.Show(good, slotPosition, contents.GoodCount, hasItem);
                    return;
                }

                Selector.Show(good, slotPosition, hasItem);
            }
            else
            {
                foreach (var contents in contentsList)
                {
                    if (contents.SlotPosition != slotPosition) continue;

                    Selector.Show(contents.Good, slotPosition, contents.GoodCount, false);
                    return;
                }

                Selector.Show(slotPosition);
            }
        }

        public void ClearSlot(int slotPosition)
        {
            for (int i = 0; i < contentsList.Count; i++)
            {
                DeliveryContents contents = contentsList[i];
                if (contents.SlotPosition != slotPosition) continue;

                contentsList.RemoveAt(i);
                return;
            }
        }


        public void AddGood(Good good, int slotPosition, int count)
        {
            if (good == null || count <= 0) return;

            int currentCount = 0;
            bool hasItem = false;

            foreach (var slot in machineData.slots)
            {
                if (slot.SlotPosition != slotPosition) continue;

                currentCount = slot.GoodCount;
                hasItem = true;

                break;
            }

            foreach (var contents in contentsList)
            {
                if (contents.SlotPosition != slotPosition) continue;

                contents.GoodCount = count;
                contents.Good = good;
                contents.GoodId = good.GoodId;
                Items.UpdateSlot(slotPosition, contents.Good, count + currentCount, !hasItem);
                return;
            }

            DeliveryContents newContents = new DeliveryContents()
            {
                Good = good,
                GoodId = good.GoodId,
                GoodCount = count,
                SlotPosition = slotPosition
            };

            Items.UpdateSlot(slotPosition, good, count + currentCount, !hasItem);

            contentsList.Add(newContents);
        }

        public void ApplyDelivery()
        {
            state = MachineState.FISHISHED;

            foreach (var contents in contentsList)
            {
                contents.Good = null;
            }
            
            DeliveryData deliveryData = new DeliveryData()
            {
                delivery = new Delivery()
                {
                    WithdrawnMoney = machineData.CashStored,
                    contents = contentsList
                },
                employeeToken = employeeToken
            };
            
            AsyncHelper.FireAndForget( 
                () => MakeDelivery(deliveryData),
                ex =>
                {
                    state = MachineState.ERROR;
                    Display.Text = "Доставка не удалась!";
                    ResetLater(2000);
                    console.LogError(ex.Message);
                });
            
        }

        private async Task MakeDelivery(DeliveryData purchase)
        {
            string json = JsonConvert.SerializeObject(purchase);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync($"{backEndUrl}/api/Delivery/Make", content);

            if (result.IsSuccessStatusCode)
            {
                Dispatcher.Invoke(DeliverySuccessful);
            }
            else
            {
                string error = await result.Content.ReadAsStringAsync();
                Dispatcher.Invoke(() =>
                {
                    DeliveryError(error);
                });
            }
        }

        private void DeliveryError(string error)
        {
            state = MachineState.WAITING;
            Display.Text = $"Ошибка: {error}";
            contentsList.Clear();
            AsyncHelper.FireAndForget(
                GetMachineData,
                ex => console.LogError(ex.Message));
            OnExitClicked(null, null);
            
            OnExitClicked(null, null);
            ResetLater();
        }

        private void DeliverySuccessful()
        {
            state = MachineState.WAITING;
            Display.Text = "Доставка успешна!";
            contentsList.Clear();
            
            AsyncHelper.FireAndForget(
                GetMachineData,
                ex => console.LogError(ex.Message));
            OnExitClicked(null, null);
            ResetLater(2000);
        }

        #endregion
    }
}