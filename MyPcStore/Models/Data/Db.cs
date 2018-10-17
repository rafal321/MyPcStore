using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MyPcStore.Models.Abstract;

namespace MyPcStore.Models.Data
{
    // Db - name defined in connection string
    // access data through entity framework 
    public class Db : DbContext, ITestTDbSet
    {
        public Db() : base("Db")
        {

        }

        public DbSet<PageDTO> Pages { get; set; }
        public DbSet<SidebarDTO> Sidebar { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
        public DbSet<ProductDTO> Products { get; set; }
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<RoleDTO> Roles { get; set; }
        public DbSet<UserRoleDTO> UserRoles { get; set; }
        public DbSet<OrderDTO> Orders { get; set; }
        public DbSet<OrderDetailsDTO> OrderDetails { get; set; }

        //need for unit testing
        public void MarkAsModified(object item)
        {
            Entry(item).State = EntityState.Modified;
        }

    }
}