using System.ComponentModel.DataAnnotations;
using System.Web;

namespace VendingMachineBackend.ViewModel
{
    public class MachineAddViewModel
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public string EmployeeId { get; set; }
    }
}