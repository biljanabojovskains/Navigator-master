using System;
using System.Collections.Generic;
using PublicNavigator.Models.Abstract;

namespace PublicNavigator.Dal.Abstract
{
    public interface IBiznisInfoDal
    {
        List<IBiznisInfo> GetListBiznisInfo(string coordinates);
    }
}