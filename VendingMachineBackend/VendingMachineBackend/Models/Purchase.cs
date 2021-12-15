using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineBackend.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }
        public int MachineId { get; set; }
        public int SlotPosition { get; set; }
        public int GoodId { get; set; }
        public int GoodCount { get; set; }
        public DateTime PurchaseTime { get; set; }
        
        public Good Good { get; set; }
    }
}