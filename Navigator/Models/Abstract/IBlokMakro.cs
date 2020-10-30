namespace Navigator.Models.Abstract
{
    public interface IBlokMakro
    {
        string Id { get; set; }
        string Ime { get; set; }
        double? Povrshina { get; set; }
        double PovrshinaPresmetana { get; set; }
        void QcInput(IBlokMakro source);
        string GeoJson { get; set; }
    }
}
