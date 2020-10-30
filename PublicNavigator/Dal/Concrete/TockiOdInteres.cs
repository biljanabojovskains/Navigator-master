using PublicNavigator.Dal.Abstract;

namespace PublicNavigator.Dal.Concrete
{
    public class TockiOdInteres : ITockiOdInteres
    {
        public int Id { get; set; }
        public string Tema { get; set; }
        public string Podtema { get; set; }
        public string GeoJson { get; set; }
    }
}