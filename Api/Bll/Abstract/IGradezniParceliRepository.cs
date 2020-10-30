using System.Collections.Generic;
using Api.ViewModels.Concrete;

namespace Api.Bll.Abstract
{
    public interface IGradezniParceliRepository
    {
        List<GradParceli> GetByBuffer(double lon, double lat);
        List<GradParceli> GetByOpfat(int opfatId);
        GradParceli GetByParcela(int id);
        string GetByParcelaExcel(int id);
        string GetByParcelaExcel(List<int> ids);
        string GetGeom(int id);
        string GetImage(int id);
    }
}
