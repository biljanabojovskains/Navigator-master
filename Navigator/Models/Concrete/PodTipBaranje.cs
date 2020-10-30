using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    [Serializable]
    public class PodTipBaranje : IPodTipBaranje
    {
        public int id { get; set; }
        public string text { get; set; }
    }
}