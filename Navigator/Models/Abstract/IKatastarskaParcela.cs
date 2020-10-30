namespace Navigator.Models.Abstract
{
    public interface IKatastarskaParcela
    {
        int Id { get; set; }
        string Name { get; set; }
        string GeoJson { get; set; }
        string Location { get; set; } 
    }
}
