using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class OpfatMakro : IOpfatMakro
    {
        public string Id { get; set; }
        public string Ime { get; set; }
        public string Vaznost { get; set; }
        public string TehnickiBroj { get; set; }
        public double? Povrshina { get; set; }
        public string ZakonskaRegulativa { get; set; }
        public string Izrabotuva { get; set; }
        public double PovrshinaPresmetana { get; set; }

        public void QcInput(IOpfatMakro source)
        {
            if (!source.Povrshina.HasValue)
                throw new InvalidCastException("Невалидна вредност кај колона Површина ");
        }
        public string GeoJson { get; set; }
    }
}
