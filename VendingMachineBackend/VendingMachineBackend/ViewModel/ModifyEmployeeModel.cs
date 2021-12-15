using System.ComponentModel.DataAnnotations;

namespace VendingMachineBackend.ViewModel
{
    public class ModifyEmployeeModel
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        
        [Required]
        public decimal Salary { get; set; }
        public string Email { get; set; }
        
        [Required]
        public string PermissionId { get; set; }
    }
}