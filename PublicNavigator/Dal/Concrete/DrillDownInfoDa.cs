using PublicNavigator.Dal.Abstract;
using PublicNavigator.Models.Abstract;
using PublicNavigator.Models.Concrete;

namespace PublicNavigator.Dal.Concrete
{
    public class DrillDownInfoDa : IDrillDownInfoDal
    {
        public IDrillDownInfo GetInfo(string coordinates)
        {
            IDrillDownInfo infoItem = new DrillDownInfo();
            infoItem.ListOpfat = new OpfatDa().GenerateList(coordinates);
            infoItem.ListBlok = new BlokDa().GenerateList(coordinates);
            infoItem.ListGradParceli = new ParceliDa().GenerateList(coordinates);
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