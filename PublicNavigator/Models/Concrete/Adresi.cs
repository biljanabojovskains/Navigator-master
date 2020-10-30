using PublicNavigator.Models.Abstract;

namespace PublicNavigator.Models.Concrete
{
    public class Adresi : IAdresi
    {
        public int Id { get; set; }
        public string GeoJson { get; set; }
    }
}