using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class TreeShrubTopology : ITreeShrubTopology
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LatinName { get; set; }
        public int? Fk_Type { get; set; }
        public int? Fk_Season { get; set; }
    }
}