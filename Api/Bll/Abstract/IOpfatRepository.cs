using System.Collections.Generic;
using Api.ViewModels.Concrete;

namespace Api.Bll.Abstract
{
    public interface IOpfatRepository
    {
        List<Opfat> GetAll();
    }
}
