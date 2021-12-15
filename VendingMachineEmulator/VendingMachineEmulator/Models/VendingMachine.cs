using System.Collections.Generic;
using System.Net.Http;
using VendingMachineEmulator.Util;

namespace VendingMachineEmulator.Models
{
    public class VendingMachine
    {
        public int MachineId { get; set; }
        public int EmployeeId { get; set; }
        public string Address { get; set; }
        public decimal CashStored { get; set; }

        public List<VendingMachineSlot> slots { get; set; }
    }

    public class VendingMachineSlot
    {
        public int SlotId { get; set; }
        public int MachineId { get; set; }

        public int SlotPosition { get; set; }
        
        public int GoodId { get; set; }
        public int GoodCount { get; set; }
    }
}