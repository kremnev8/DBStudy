using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VendingMachineEmulator.Models;
using VendingMachineEmulator.Util;

namespace VendingMachineEmulator
{
    public partial class ItemDisplay : UserControl
    {
        private Dictionary<int, ItemButton> itemSlots = new Dictionary<int, ItemButton>();

        public Action<Good, int, bool> onTrySelectGood;
        
        public ItemDisplay()
        {
            InitializeComponent();

            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    int slotIndex = (y + 1) * 10 + x;
                    
                    ItemButton newControl = new ItemButton(slotIndex);
                    newControl.Clear();
                    newControl.Button.Click += OnItemButtonPressed;
                    
                    Grid.SetColumn(newControl, x);
                    Grid.SetRow(newControl, y);
                    Items.Children.Add(newControl);
                    
                    itemSlots.Add(slotIndex, newControl);
                }
            }
        }

        public void UpdateSlot(int slotPosition, Good newGood = null, int newCount = 0, bool _phantom = false)
        {
            if (itemSlots.ContainsKey(slotPosition))
            {
                Dispatcher.Invoke(() =>
                {
                    if (newGood == null && newCount == 0)
                    {
                        itemSlots[slotPosition].Clear();
                    }
                    else if (newGood == null && newCount > 0)
                    {
                        itemSlots[slotPosition].UpdateCount(newCount);
                    }
                    else
                    {
                        itemSlots[slotPosition].SetGood(newGood, newCount, _phantom);
                    }
                });
            }
        }

        public void OnItemButtonPressed(object sender, RoutedEventArgs e)
        {
            Button grid = e.OriginalSource as Button;
            ItemButton button = grid.Parent as ItemButton;

            onTrySelectGood?.Invoke(button.good, button.slotIndex, button.phantom);
        }

        public async Task UpdateItems()
        {
            foreach (var kv in itemSlots)
            {
                kv.Value.Dispatcher.Invoke(() =>
                {
                    kv.Value.Clear();
                });
            }
            
            try
            {
                foreach (var slot in MainWindow.machineData.slots)
                {
                    if (slot.GoodId != 0 && 
                        slot.GoodCount > 0 && 
                        slot.SlotPosition != 0 && 
                        itemSlots.ContainsKey(slot.SlotPosition))
                    {
                        ItemButton button = itemSlots[slot.SlotPosition];
                        Good good = await MainWindow.client.GetJsonAsync<Good>($"{MainWindow.backEndUrl}/api/Good/Detail?id={slot.GoodId}");
                        button.Dispatcher.Invoke(() =>
                        {
                            button.SetGood(good, slot.GoodCount);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                MainWindow.console.LogError(e.Message);
            }
        }



    }
}