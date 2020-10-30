using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class TreeShrubType : ITreeShrubType
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
}