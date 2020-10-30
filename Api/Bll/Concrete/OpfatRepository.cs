using System.Collections.Generic;
using Api.Bll.Abstract;
using Api.Dal.Concrete;
using Api.ViewModels.Concrete;

namespace Api.Bll.Concrete
{
    public class OpfatRepository : IOpfatRepository
    {
        public List<Opfat> GetAll()
        {
            return new OpfatDa().GetAll();
        }
    }
}