using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace VendingMachineBackend.Models
{
    public class Good
    {
        [Key]
        public int GoodId { get; set; }
        public string Name { get; set; }
        public decimal PurchaseCost { get; set; }
        public decimal SaleCost { get; set; }
        public int SupplierId { get; set; }

        public string IconPath { get; set; }

        public Supplier Supplier { get; set; }
    }
    
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string Name { get; set; }
    }
}