using System;
using System.Collections.Generic;

namespace VendingMachineEmulator.Models
{
    public class DeliveryData
    {
        public string employeeToken { get; set; }
        public Delivery delivery { get; set; }
    }
    
    public class Delivery
    {
        public int DeliveryId { get; set; }
        public int MachineId { get; set; }
        public int EmployeeId { get; set; }
        public decimal WithdrawnMoney { get; set; }
        public DateTime DeliveryDate { get; set; }
    
        public List<DeliveryContents> contents { get; set; }
    }
    
    public class DeliveryContents
    {
        public int ItemId { get; set; }
        public int DeliveryId { get; set; }
        public int SlotPosition { get; set; }
        public int GoodCount { get; set; }
        public int GoodId { get; set; }
        
        public Good Good { get; set; }
    }
}