namespace VendingMachineEmulator.Models
{
    public enum Permission
    {
        None,
        Techician,
        Manager,
        Admin
    }
    
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }

        public decimal Salary { get; set; }
        public int PermissionId { get; set; }

        public Permission getPermission()
        {
            return (Permission)(PermissionId - 1);
        }
    }
}