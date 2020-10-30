using System.Collections.Generic;
using Api.ViewModels.Concrete;

namespace Api.Dal.Abstract
{
    public interface IGradParceliDa
    {
        List<GradParceli> Get(double lon, double lat);
        List<GradParceli> GetByOpfat(int opfatId);
        GradParceli Get(int id);
        List<GradParceli> Get(List<int> id);
        string GetGeom(int id);
    }
}
