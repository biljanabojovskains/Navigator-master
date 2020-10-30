using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class GradezniParceli : IGradezniParceli
    {
        public int Id { get; set; }
        public string GeoJson { get; set; }
    }
}