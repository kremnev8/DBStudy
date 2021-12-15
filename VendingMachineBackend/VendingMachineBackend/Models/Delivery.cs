using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachineBackend.Models
{
    public class Delivery
    {
        [Key]
        public int DeliveryId { get; set; }
        public int MachineId { get; set; }
        public int EmployeeId { get; set; }
        public decimal WithdrawnMoney { get; set; }
        public DateTime DeliveryDate { get; set; }
    
        public List<DeliveryContents> contents { get; set; }
    }
    
    public class DeliveryContents
    {
        [Key]
        public int ItemId { get; set; }
        public int DeliveryId { get; set; }
        public int SlotPosition { get; set; }
        public int GoodCount { get; set; }
        public int GoodId { get; set; }
        
        public Good Good { get; set; }
    }
}