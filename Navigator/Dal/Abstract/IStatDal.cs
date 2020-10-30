using System.Collections.Generic;
using Navigator.Models.Abstract;

namespace Navigator.Dal.Abstract
{
    public interface IStatDal
    {
        IStat GetStat(int opfatId);
    }
}