using System;
namespace Navigator.Models.Abstract
{
    public interface IPovrshiniGradba
    {
        string Id { get; set; }
        double? Povrshina { get; set; }
        double? PovrshinaGradenje { get; set; }
        double? BrutoPovrshina { get; set; }
        double? MaxVisina { get; set; }
        string Katnost { get; set; }
        double PovrshinaPresmetana { get; set; }
        void QcInput(IPovrshiniGradba source);
        string GeoJson { get; set; }
    }
}
