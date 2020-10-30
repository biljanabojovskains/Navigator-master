using System.Collections.Generic;
using Navigator.Models.Abstract;

namespace Navigator.Dal.Abstract
{
    public interface IKatOpstiniDal
    {
        /// <summary>
        /// Generate list of intersects for given parcel
        /// </summary>
        /// <param name="parcelaId">ID number for parcel</param>
        /// <returns>list of intersects</returns>
        List<IKatOpstina> GetIntersect(int parcelaId);
    }
}
