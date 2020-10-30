using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class FlowerSeason : IFlowerSeason
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}