using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineBackend.Models
{
    public class VendingMachine
    {
        [Key]
        public int MachineId { get; set; }
        public int EmployeeId { get; set; }
        public string Address { get; set; }
        public decimal CashStored { get; set; }

        public string AccessToken { get; set; }

        public List<VendingMachineSlot> slots { get; set; }
    }

    public class VendingMachineSlot
    {
        [Key]   
        public int SlotId { get; set; }
        public int MachineId { get; set; }

        public int SlotPosition { get; set; }
        
        public int GoodId { get; set; }
        public int GoodCount { get; set; }
        
        public Good Good { get; set; }
    }
}