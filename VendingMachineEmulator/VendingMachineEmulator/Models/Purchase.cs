using System;

namespace VendingMachineEmulator.Models
{
    public class Purchase
    {
        public int SlotPosition { get; set; }
        public int GoodId { get; set; }
        public int GoodCount { get; set; }
        public DateTime PurchaseTime { get; set; }
    }
}