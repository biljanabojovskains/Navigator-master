using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
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
        public string ProcentIzgradenostOpisno
        {
            get
            {
                switch (ProcentIzgradenost.ToString())
                {
                    case "-1":
                        return "Постојна состојба";
                    case "-2":
                        return "со Архитектонско-урбанистички проект";
                    case "-3":
                        return "според АУП";
                    default:
                        return ProcentIzgradenost.ToString();
                }
            }
        }
        public string KoeficientIskoristenostOpisno
        {
            get
            {
                switch (KoeficientIskoristenost.ToString())
                {
                    case "-1":
                        return "Постојна состојба";
                    case "-2":
                        return "со Архитектонско-урбанистички проект";
                    case "-3":
                        return "според АУП";
                    default:
                        return KoeficientIskoristenost.ToString();
                }
            }
        }
        public string PovrshinaOpisno
        {
            get
            {
                switch (Povrshina.ToString())
                {
                    case "-1":
                        return "Постојна состојба";
                    case "-2":
                        return "со Архитектонско-урбанистички проект";
                    case "-3":
                        return "според АУП";
                    default:
                        return Povrshina.ToString();
                }
            }
        }
        public string PovrshinaGradenjeOpisno
        {
            get
            {
                switch (PovrshinaGradenje.ToString())
                {
                    case "-1":
                        return "Постојна состојба";
                    case "-2":
                        return "со Архитектонско-урбанистички проект";
                    case "-3":
                        return "според АУП";
                    default:
                        return PovrshinaGradenje.ToString();

                }
            }
        }
        public string BrutoPovrshinaOpisno
        {
            get
            {
                switch (BrutoPovrshina.ToString())
                {
                    case "-1":
                        return "Постојна состојба";
                    case "-2":
                        return "со Архитектонско-урбанистички проект";
                    case "-3":
                        return "според АУП";
                    default:
                        return BrutoPovrshina.ToString();

                }
            }
        }
        public string Investitor { get; set; }
        public int PovrshinaZaokruzena { get { return Convert.ToInt32(Povrshina); } }
        public int BrutoPovrshinaZaokruzena { get { return Convert.ToInt32(BrutoPovrshina); } }
        public int PovrshinaGradenjeZaokruzena { get { return Convert.ToInt32(PovrshinaGradenje); } }

        public int? NumerickiPokazateliId { get; set; }

        public string BrTehIspravka { get; set; }
        public DateTime? DatumTehIspravka { get; set; }
        public int? TehnickiIspravkiId { get; set; }

    }
}
