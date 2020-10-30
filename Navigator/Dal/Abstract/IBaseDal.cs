using System.Collections.Generic;
using Navigator.Models.Abstract;

namespace Navigator.Dal.Abstract
{
    public interface IBaseDal<T>
    {
        /// <summary>
        /// Generate list of blok items
        /// </summary>
        /// <param name="coordinates">coordinates in the format x,y</param>
        /// <returns>list of blok items</returns>
        List<T> GenerateList(string coordinates);
        /// <summary>
        /// Word search generator which generate list from Blok items 
        /// </summary>
        /// <param name="keyword">keyword which is using for search</param>
        /// <returns>list of blok items</returns>
        List<T> Search(string keyword);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IVertex GetCentroidById(int id);
        /// <summary>
        /// Get object by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(int id);
    }
}
