using Navigator.Dal.Abstract;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using System.Configuration;
using Navigator.Dal.Concrete.FieldGis;

namespace Navigator.Dal.Concrete
{
    public class DrillDownInfoDa : IDrillDownInfoDal
    {
        public IDrillDownInfo GetInfo(string coordinates)
        {
            IDrillDownInfo infoItem = new DrillDownInfo();
            infoItem.ListOpfat = new OpfatDa().GenerateList(coordinates);
            infoItem.ListBlok = new BlokDa().GenerateList(coordinates);
            infoItem.ListGradParceli = new ParceliDa().GenerateList(coordinates);
            //infoItem.ListLegalizacija= new LegalizacijaDal().GenerateList(coordinates);
            var urbanaOprema = ConfigurationManager.AppSettings["urbanaOprema"];

            //infoItem.ListZelenilo = new TreeShrubDa().GetZelenilo(coordinates);
            //infoItem.ListTreeShrub = new TreeShrubDa().GetTreeShrub(coordinates);
            //infoItem.ListCvekjinja = new FlowerDa().GetCvekjinja(coordinates);
            //infoItem.ListZeleniPovrsini = new GreenSurfaceDa().GetZeleniPovrsini(coordinates);



            if (urbanaOprema == "da")
            {
                infoItem.ListOsvetluvanje = new OsvetluvanjeDa().GenerateList(coordinates);
                infoItem.ListAvtobuska = new AvtobuskaDa().GenerateList(coordinates);
                infoItem.ListDefekt = new DefektDa().GenerateList(coordinates);
                infoItem.ListJavnaPovrsina = new JavnaPovrsinaDa().GenerateList(coordinates);
                infoItem.ListOprema = new OpremaDa().GenerateList(coordinates);
                infoItem.ListVodovod = new VodovodDa().GenerateList(coordinates);
            }
            return infoItem;
        }

        public IDrillDownInfo SearchAll(string keyword)
        {
            IDrillDownInfo infoItem = new DrillDownInfo();
            infoItem.ListOpfat = new OpfatDa().Search(keyword);
            infoItem.ListBlok = new BlokDa().Search(keyword);
            infoItem.ListGradParceli = new ParceliDa().Search(keyword);
            return infoItem;
        }

        public IDrillDownInfo SearchKatastarskiParceli(string keyword)
        {
            IDrillDownInfo infoItem = new DrillDownInfo();
            infoItem.ListKatastarskaParcela = new KatParceliDa().Get(keyword);
            return infoItem;
        }

    }
}