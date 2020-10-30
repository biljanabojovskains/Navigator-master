using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    [Serializable]
    public class Notifikacii : INotifikacii
    {
        public int Id { get; set; }
        public string Tema { get; set; }
        public string Podtema { get; set; }
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; }
        public string GeoJson { get; set; }
        
    }
}