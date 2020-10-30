using System.Collections.Generic;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete.FileldGis;

namespace Navigator.Models.Concrete
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

        public List<ILegalizacija> ListLegalizacija { get; set; }

        //FieldGIS
        public List<Osvetluvanje> ListOsvetluvanje { get; set; }
        public List<AvtobuskaStanica> ListAvtobuska { get; set; }
        public List<Defekt> ListDefekt { get; set; }
        public List<JavnaPovrsina> ListJavnaPovrsina { get; set; }
        public List<Oprema> ListOprema { get; set; }
        public List<Vodovod> ListVodovod { get; set; } 

        //Zelenilo
        public List<IZelenilo> ListZelenilo { get; set; }
        public List<ITreeShrubInventory> ListTreeShrub { get; set; }
        public List<IFlowerInventory> ListCvekjinja { get; set; }
        public List<IGreenSurfaceInventory> ListZeleniPovrsini { get; set; }

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
                   (ListPovrshiniGradba == null || ListPovrshiniGradba.Count == 0) &&
                   (ListZelenilo == null || ListZelenilo.Count == 0) &&
                   (ListTreeShrub == null || ListTreeShrub.Count == 0) &&
                   (ListCvekjinja == null || ListCvekjinja.Count == 0) &&
                   (ListZeleniPovrsini == null || ListZeleniPovrsini.Count == 0);
        }
    }
}
