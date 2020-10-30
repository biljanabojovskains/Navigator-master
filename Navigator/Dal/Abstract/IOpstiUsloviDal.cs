using Navigator.Models.Abstract;

namespace Navigator.Dal.Abstract
{
    public interface IOpstiUsloviDal
    {
        /// <summary>
        /// Generate objects of general conditions for given id
        /// </summary>
        /// <param name="id">ID number for general conditions</param>
        /// <returns>objects of general conditions</returns>
        IUslov Get(int id);
        /// <summary>
        /// Adding general condition for given path
        /// </summary>
        /// <param name="filePath">Full path to the file</param>
        /// <returns>if condition is successful then it returns id of general condition else return -1</returns>
        int Add(string filePath);
        /// <summary>
        /// Update of micro building parcels
        /// </summary>
        /// <param name="opfatId">ID number of opfat</param>
        /// <param name="uslovId">ID number of condition</param>
        /// <returns>if condition is successful then it returns true which means that the action is completed else it returns false</returns>
        bool Add(int opfatId, int uslovId);
    }
}
