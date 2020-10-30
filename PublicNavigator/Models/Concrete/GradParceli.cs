using System;
using PublicNavigator.Models.Abstract;

namespace PublicNavigator.Models.Concrete
{
    [Serializable]
    public class GradParceli : IGradParceli
    {
        public int Id { get; set; }

        public string Broj { get; set; }

        public string KlasaNamena { get; set; }

        public string KompKlasaNamena { get; set; }

        public double? Povrshina { get; set; }

        public double? PovrshinaGradenje { get; set; }

        public double? BrutoPovrshina { get; set; }

        public string MaxVisina { get; set; }

        public double? Extrude { get; set; }

        public string Katnost { get; set; }

        public string ParkingMesta { get; set; }

        public double? ProcentIzgradenost { get; set; }
        
        public int? OpstiUsloviId { get; set; }

        public int? PosebniUsloviId { get; set; }

        public double? ProcentIzgradenostPresmetana
        {
            get
            {
                try
                {
                    return Math.Round(Convert.ToDouble(PovrshinaGradenje/PovrshinaPresmetana*100), 2);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public double? KoeficientIskoristenost { get; set; }

        public double? PovrshinaPresmetana { get; set; }

        public string GeoJson { get; set; }

        public double? Presek { get; set; }
        public int OpfatId { get; set; }
    }
}
