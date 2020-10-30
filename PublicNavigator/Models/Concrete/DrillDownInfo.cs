using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PublicNavigator.Models.Abstract;

namespace PublicNavigator.Models.Concrete
{
    public class DrillDownInfo : IDrillDownInfo
    {
        public List<IOpfatMakro> ListOpfatMakro { get; set; }

        public List<IGradskaCetvrtMakro> ListGradskaCetvrtMakro { get; set; }

        public List<IBlokMakro> ListBlokMakro { get; set; }

        public List<INamenskiZoniMakro> ListNamenskiZoniMakro { get; set; }

        public List<IOpfat> ListOpfat { get; set; }

        public List<IRegLinija> ListRegLinija { get; set; }

        public List<IBlok> ListBlok { get; set; }

        public List<IGradParceli> ListGradParceli { get; set; }

        public List<IPovrshiniGradba> ListPovrshiniGradba { get; set; }

        public List<IKatastarskaParcela> ListKatastarskaParcela { get; set; }

        public bool IsEmpty()
        {
            return (ListOpfatMakro == null || ListOpfatMakro.Count == 0) &&
                   (ListGradskaCetvrtMakro == null || ListGradskaCetvrtMakro.Count == 0) &&
                   (ListBlokMakro == null || ListBlokMakro.Count == 0) &&
                   (ListNamenskiZoniMakro == null || ListNamenskiZoniMakro.Count == 0) &&
                   (ListOpfat == null || ListOpfat.Count == 0) &&
                   (ListRegLinija == null || ListRegLinija.Count == 0) &&
                   (ListBlok == null || ListBlok.Count == 0) &&
                   (ListGradParceli == null || ListGradParceli.Count == 0) &&
                   (ListPovrshiniGradba == null || ListPovrshiniGradba.Count == 0);
        }
    }
}