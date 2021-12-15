namespace VendingMachineEmulator.Models
{
    public class Good
    {
        public int GoodId { get; set; }
        public string Name { get; set; }
        public decimal PurchaseCost { get; set; }
        public decimal SaleCost { get; set; }
        public int SupplierId { get; set; }

        public string IconPath { get; set; }
    }
}