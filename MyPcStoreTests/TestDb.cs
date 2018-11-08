using MyPcStore.Models.Abstract;
using MyPcStore.Models.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPcStoreTests
{
    class TestDb : ITestTDbSet
    {
        public TestDb()
        {
            this.Pages = new TestPageDbset();
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

        public int SaveChanges()
        {
            return 0;
        }
        public void MarkAsModified(Object item) { }
        public void Dispose() { }
    }
}
