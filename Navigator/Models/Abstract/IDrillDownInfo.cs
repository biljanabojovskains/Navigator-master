using System.Collections.Generic;
using Navigator.Models.Concrete.FileldGis;

namespace Navigator.Models.Abstract
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

        //FieldGIS
        List<Osvetluvanje> ListOsvetluvanje { get; set; }
        List<AvtobuskaStanica> ListAvtobuska { get; set; }
        List<Defekt> ListDefekt { get; set; }
        List<JavnaPovrsina> ListJavnaPovrsina { get; set; }
        List<Oprema> ListOprema { get; set; }
        List<Vodovod> ListVodovod { get; set; } 
        bool IsEmpty();

        //Zelenilo
        List<IZelenilo> ListZelenilo { get; set; }
        List<ITreeShrubInventory> ListTreeShrub { get; set; }
        List<IFlowerInventory> ListCvekjinja { get; set; }
        List<IGreenSurfaceInventory> ListZeleniPovrsini { get; set; }

        
        
    }
}
