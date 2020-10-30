using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class RegLinija : IRegLinija
    {
        public string Id { get; set; }

        public double? Povrshina { get; set; }

        public double PovrshinaPresmetana { get; set; }

        public void QcInput(IRegLinija source)
        {
            if (!source.Povrshina.HasValue)
                throw new InvalidCastException("Nevalidna vrednost kaj kolona Povrshina kaj Id=" + Id);
        }

        public string GeoJson { get; set; }
    }
}
