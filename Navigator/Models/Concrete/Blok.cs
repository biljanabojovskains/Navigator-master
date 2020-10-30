using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class Blok : IBlok
    {
        public int Id { get; set; }

        public string Ime { get; set; }

        public string Namena { get; set; }

        public double? Povrshina { get; set; }
        public double? PovrshinaPresmetana { get; set; }

        public string GeoJson { get; set; }
    }
}
