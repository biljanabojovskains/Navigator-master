using System.Collections.Generic;
using PublicNavigator.Models.Abstract;

namespace PublicNavigator.Dal.Abstract
{
    public interface IKatParceliDal
    {
        /// <summary>
        /// Generate list of parcels for given text
        /// </summary>
        /// <param name="searchedText">text which is used for search</param>
        /// <returns>list of parcels</returns>
        List<IKatastarskaParcela> Get(string searchedText);
    }
}
