using System;
using System.Collections.Generic;

namespace InfoBoltAPI.DB
{
    public partial class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Pw { get; set; } = null!;
        public int Role { get; set; }
    }
}
