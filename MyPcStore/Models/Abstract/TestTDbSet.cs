using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyPcStore.Models.Data;
using System.Data.Entity;

namespace MyPcStore.Models.Abstract
{
    public interface TestTDbSet : IDisposable
    {

        DbSet<PageDTO> Pages { get; }
        DbSet<SidebarDTO> Sidebar { get; }
        DbSet<CategoryDTO> Categories { get; }
        DbSet<ProductDTO> Products { get; }
        DbSet<UserDTO> Users { get; }
        DbSet<RoleDTO> Roles { get; }
        DbSet<UserRoleDTO> UserRoles { get; }
        DbSet<OrderDTO> Orders { get; }
        DbSet<OrderDetailsDTO> OrderDetails { get; }

        int SaveChanges();
        void MarkAsModified(Object item);

    }
}