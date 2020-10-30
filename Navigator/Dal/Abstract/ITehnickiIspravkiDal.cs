using Navigator.Models.Abstract;


namespace Navigator.Dal.Abstract
{
    public interface ITehnickiIspravkiDal
    {
        /// <summary>
        /// Generate objects of special condition for given id
        /// </summary>
        /// <param name="id">ID number of special condition</param>
        /// <returns>objects of special condition</returns>
        IUslov Get(int id);
        /// <summary>
        /// Adding special condition for given path
        /// </summary>
        /// <param name="filePath">Full path to the file</param>
        /// <returns>if condition is successful then it returns id of special condition else return -1</returns>
        int Add(string filePath);
    }
}
