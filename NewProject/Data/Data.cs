using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewProject.Data
{
    public class Data : DbContext
    {
        public DbSet<Entity.Department> Departments { get; set; }
        public DbSet<Entity.Manager> Managers { get; set; }
        public Data() : base()
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=NewProject;Integrated Security=True"
                );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity.Manager>() //
                .HasOne(m => m.MainDep) // name of property
                .WithMany() // type of connection one to many
                .HasForeignKey(m => m.IdMainDep) // foreign key
                .HasPrincipalKey(d => d.Id); //main key

            modelBuilder.Entity<Entity.Manager>() //
                .HasOne(m => m.SecDep) // name of property
                .WithMany() // type of connection one to many
                .HasForeignKey(m => m.IdSecDep) // foreign key
                .HasPrincipalKey(d => d.Id); //main key

            modelBuilder.Entity<Entity.Manager>() //
                .HasOne(m => m.Chef) // name of property
                .WithMany() // type of connection one to many
                .HasForeignKey(m => m.IdChief) // foreign key
                .HasPrincipalKey(m => m.Id); //main key

            // need to add migration
        }
    }
}
