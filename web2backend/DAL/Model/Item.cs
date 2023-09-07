﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class Item
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Amount { get; set; }
        public string PictureUrl { get; set; }
        public User User { get; set; }
        public long UserId { get; set; }
    }
}
