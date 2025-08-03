using System;
using System.Collections.Generic;

namespace DropBoxMarket.Models.ViewModels
{
    public class ConfirmationViewModel
    {
        public string FullName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }
}
