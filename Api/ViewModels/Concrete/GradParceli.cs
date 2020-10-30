using System;

namespace Api.ViewModels.Concrete
{
    public class GradParceli
    {
        public int Id { get; set; }

        public string Broj { get; set; }

        public string KlasaNamena { get; set; }

        public string KompKlasaNamena { get; set; }

        /// <summary>
        /// -1 Постојна состојба
        /// -2 со Архитектонско-урбанистички проект
        /// -3 според АУП
        /// >0 вистинска површина
        /// </summary>
        public double? Povrshina { get; set; }
        /// <summary>
        /// -1 Постојна состојба
        /// -2 со Архитектонско-урбанистички проект
        /// -3 според АУП
        /// >0 вистинска површина
        /// </summary>
        public double? PovrshinaGradenje { get; set; }
        /// <summary>
        /// -1 Постојна состојба
        /// -2 со Архитектонско-урбанистички проект
        /// -3 според АУП
        /// >0 вистинска површина
        /// </summary>
        public double? BrutoPovrshina { get; set; }

        public string MaxVisina { get; set; }

        public string Katnost { get; set; }

        public string ParkingMesta { get; set; }

        public double? ProcentIzgradenost { get; set; }

        public double? KoeficientIskoristenost { get; set; }

        public double? PovrshinaPresmetana { get; set; }

        public int OpfatId { get; set; }
        public string OpfatIme { get; set; }
    }
}