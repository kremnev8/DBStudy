using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace VendingMachineBackend.Models
{
    public class Employee : IUser<string>
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }

        public decimal Salary { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int PermissionId { get; set; }

        public Permission getPermission()
        {
            return (Permission)(PermissionId - 1);
        }

        [NotMapped]
        public string Id => EmployeeId.ToString();
        [NotMapped]
        public string UserName
        {
            get => Email;
            set => Email = value;
        }
        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Employee> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            
            string permission = getPermission().ToString();
            userIdentity.AddClaim(new Claim(ClaimTypes.Role, permission));

            return userIdentity;
        }
    }

    public class EmployeePermission
    {
        [Key]
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
    }
}