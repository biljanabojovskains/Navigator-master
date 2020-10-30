using PublicNavigator.Models.Abstract;

namespace PublicNavigator.Dal.Abstract
{
    public interface IDrillDownInfoDal
    {
        /// <summary>
        /// Get info for item with given coordinates
        /// </summary>
        /// <param name="coordinates">coordinates in the format x,y</param>
        /// <returns>info for item</returns>
        IDrillDownInfo GetInfo(string coordinates);
    }
}
