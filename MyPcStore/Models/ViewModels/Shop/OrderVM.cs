using MyPcStore.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPcStore.Models.ViewModels.Shop
{
    public class OrderVM
    {
        public OrderVM()
        {
        }

        public OrderVM(OrderDTO rows)
        {
            OrderId = rows.OrderId;
            UserId = rows.UserId;
            CreatedAt = rows.CreatedAt;
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}