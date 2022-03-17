using System;
using System.Collections.Generic;

namespace EFCoreShared.DB
{
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Price { get; set; }
        public int Type { get; set; }
        public string Imgurl { get; set; } = null!;
        public int Stock { get; set; }
    }
}
