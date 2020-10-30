namespace PublicNavigator.Dal.Abstract
{
    public interface ITockiOdInteres
    {
        int Id { get; set; }
        string Tema { get; set; }
        string Podtema { get; set; }
        string GeoJson { get; set; }
    }
}
