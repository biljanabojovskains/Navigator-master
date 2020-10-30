namespace PublicNavigator.Models.Abstract
{
    public interface IGradskaCetvrtMakro
    {
        string Id { get; set; }
        string Ime { get; set; }
        double PovrshinaPresmetana { get; set; }
        void QcInput(IGradskaCetvrtMakro source);
        string GeoJson { get; set; }
    }
}
