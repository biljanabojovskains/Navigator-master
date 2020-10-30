namespace Navigator.Models.Abstract
{
    public interface IRegLinija
    {
        string Id { get; set; }
        double? Povrshina { get; set; }
        double PovrshinaPresmetana { get; set; }
        void QcInput(IRegLinija source);
        string GeoJson { get; set; }
    }
}
