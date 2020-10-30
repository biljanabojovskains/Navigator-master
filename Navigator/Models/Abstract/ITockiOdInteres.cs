namespace Navigator.Models.Abstract
{
    public interface ITockiOdInteres
    {
        int Id { get; set; }
        string Tema { get; set; }
        string PodTema { get; set; }
        string GeoJson { get; set; }
    }
}
