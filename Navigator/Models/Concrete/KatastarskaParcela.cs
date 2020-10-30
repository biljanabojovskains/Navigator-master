using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class KatastarskaParcela : IKatastarskaParcela
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GeoJson { get; set; }
        public string Location { get; set; }
    }
}