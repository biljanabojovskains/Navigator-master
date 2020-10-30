using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class TreeShrubSeason : ITreeShrubSeason
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}