using PublicNavigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicNavigator.Models.Abstract
{
    public interface IDrillDownInfo
    {
        //MAKRO
        List<IOpfatMakro> ListOpfatMakro { get; set; }
        List<IGradskaCetvrtMakro> ListGradskaCetvrtMakro { get; set; }
        List<IBlokMakro> ListBlokMakro { get; set; }
        List<INamenskiZoniMakro> ListNamenskiZoniMakro { get; set; }
        //MIKRO
        List<IOpfat> ListOpfat { get; set; }
        List<IRegLinija> ListRegLinija { get; set; }
        List<IBlok> ListBlok { get; set; }
        List<IGradParceli> ListGradParceli { get; set; }
        List<IPovrshiniGradba> ListPovrshiniGradba { get; set; }
        //KATASTARSKI PARCELI
        List<IKatastarskaParcela> ListKatastarskaParcela { get; set; }
        bool IsEmpty();
    }
}