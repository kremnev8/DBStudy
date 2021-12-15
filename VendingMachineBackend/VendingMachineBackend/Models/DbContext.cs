using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace VendingMachineBackend.Models
{

    public enum Permission
    {
        None,
        Techician,
        Manager,
        Admin
    }
    
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class VendingBusinessContext : DbContext
    {
        public VendingBusinessContext(): base("conn")
        { }
 
        public DbSet<Employee> employee { get; set; }
        public DbSet<EmployeePermission> permissions { get; set; }
        public DbSet<VendingMachine> vendingmachine { get; set; }
        public DbSet<VendingMachineSlot> vendingmachineslot { get; set; }
        public DbSet<Good> good { get; set; }
        public DbSet<Supplier> supplier { get; set; }
        public DbSet<Purchase> purchase { get; set; }
        
        public DbSet<Delivery> delivery { get; set; }
        public DbSet<DeliveryContents> deliverycontents { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            
            modelBuilder.Entity<Supplier>()
                .HasKey(t => t.SupplierId);

            modelBuilder.Entity<Good>()
                .HasRequired(t => t.Supplier);

            base.OnModelCreating(modelBuilder);
        }
        
        public static VendingBusinessContext Create()
        {
            return new VendingBusinessContext();
        }
    }
}