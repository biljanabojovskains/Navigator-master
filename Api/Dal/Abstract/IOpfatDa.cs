using System.Collections.Generic;
using Api.ViewModels.Concrete;

namespace Api.Dal.Abstract
{
    public interface IOpfatDa
    {
        List<Opfat> GetAll();
    }
}
