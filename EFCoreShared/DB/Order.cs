using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EFCoreShared.DB
{
    public partial class Order
    {
        public Order()
        {
            Items = new HashSet<Item>();
        }

        public int Id { get; set; }
        public int Userid { get; set; }
        public DateTime Date { get; set; }
        public sbyte Completed { get; set; }
        public string Paymentmethod { get; set; } = null!;

        public virtual User? User { get; set; } = null;
        public virtual ICollection<Item> Items { get; set; }
    }
}
