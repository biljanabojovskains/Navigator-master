namespace PublicNavigator.Models.Abstract
{
    public interface IOpfatMakro
    {
        string Id { get; set; }
        string Ime { get; set; }
        string Vaznost { get; set; }
        string TehnickiBroj { get; set; }
        double? Povrshina { get; set; }
        string ZakonskaRegulativa { get; set; }
        string Izrabotuva { get; set; }
        double PovrshinaPresmetana { get; set; }
        void QcInput(IOpfatMakro source);
        string GeoJson { get; set; }
    }
}
