using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class PovrshiniGradba : IPovrshiniGradba
    {
        public string Id { get; set; }

        public double? Povrshina { get; set; }

        public double? PovrshinaGradenje { get; set; }

        public double? BrutoPovrshina { get; set; }

        public double? MaxVisina { get; set; }

        public string Katnost { get; set; }

        public double PovrshinaPresmetana { get; set; }

        public void QcInput(IPovrshiniGradba source)
        {
            if (!source.Povrshina.HasValue)
                throw new InvalidCastException("Nevalidna vrednost kaj kolona Povrshina kaj Id=" + Id);
            if (!source.PovrshinaGradenje.HasValue)
                throw new InvalidCastException("Nevalidna vrednost kaj kolona PovrshinaGradenje kaj Id=" + Id);
            if (!source.BrutoPovrshina.HasValue)
                throw new InvalidCastException("Nevalidna vrednost kaj kolona BrutoPovrshina kaj Id=" + Id);
            if (!source.MaxVisina.HasValue)
                throw new InvalidCastException("Nevalidna vrednost kaj kolona MaxVisina kaj Id=" + Id);
        }
        public string GeoJson { get; set; }
    }
}
