using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class Adresi : IAdresi
    {
        public int Id { get; set; }
        public string GeoJson { get; set; }
    }
}