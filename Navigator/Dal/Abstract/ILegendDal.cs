using System.Collections.Generic;
using Navigator.Models.Abstract;

namespace Navigator.Dal.Abstract
{
    public interface ILegendDal
    {
        /// <summary>
        /// Generate list of legends for given id number for opfat
        /// </summary>
        /// <param name="opfatId">ID number for opfat</param>
        /// <returns>list of legends</returns>
        List<ILegenda> Get(int opfatId);
    }
}
