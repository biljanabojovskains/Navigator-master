namespace Navigator.Models.Abstract
{
    public interface IBlok
    {
        int Id { get; set; }
        string Ime { get; set; }
        string Namena { get; set; }
        double? Povrshina { get; set; }
        double? PovrshinaPresmetana { get; set; }
        string GeoJson { get; set; }
    }
}
