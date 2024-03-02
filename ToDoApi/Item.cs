using System;
using System.Collections.Generic;

namespace ToDoApi
{
    public partial class Item
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? IsCompelte { get; set; }
        public int UserId { get; set; }
    }
}
