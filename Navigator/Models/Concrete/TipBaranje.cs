using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    [Serializable]
    public class TipBaranje : ITipBaranje
    {
        public int TipBaranjeId { get; set; }
        public string TipBaranjeName { get; set; }
    }
}