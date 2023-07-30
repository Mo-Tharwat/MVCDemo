using Demo.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Demo.DAL.Context
{
    public class MVCAppDemoDbcontext:IdentityDbContext<ApplicationUser>
    {
        public MVCAppDemoDbcontext(DbContextOptions<MVCAppDemoDbcontext> options):base(options)
        {
            
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    => optionsBuilder.UseSqlServer("Server = .; DataBase = MVCDemo; trusted_Connection=true; MultipleActiveResultSets=True;");

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }

        //public DbSet<IdentityUser> Users { get; set; }

        //public DbSet<IdentityRole> Roles { get; set; }

    }
}
