namespace PublicNavigator.Models.Abstract
{
    public interface INamenskiZoniMakro
    {
        string Id { get; set; }
        string Tip { get; set; }
        double? Povrshina { get; set; }
        double PovrshinaPresmetana { get; set; }
        void QcInput(INamenskiZoniMakro source);
        string GeoJson { get; set; }
    }
}
