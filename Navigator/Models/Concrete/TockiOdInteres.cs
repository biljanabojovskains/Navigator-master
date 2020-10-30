using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class TockiOdInteres : ITockiOdInteres
    {
        public int Id { get; set; }
        public string Tema { get; set; }
        public string PodTema { get; set; }
        public string GeoJson { get; set; }
    }
}