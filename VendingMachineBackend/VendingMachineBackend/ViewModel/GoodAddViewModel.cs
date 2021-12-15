using System.ComponentModel.DataAnnotations;
using System.Web;
using Validator = System.Web.WebPages.Validator;

namespace VendingMachineBackend.ViewModel
{
    public class GoodAddViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string SuppliderId { get; set; }
        
        [Required]
        [DataType(DataType.Currency)]
        public decimal SaleCost { get; set; }
        
        [Required]
        [DataType(DataType.Currency)]
        public decimal PurchaseCost { get; set; }
        
        [Required]
        public HttpPostedFileBase GoodIcon { get; set; }
        
    }
}