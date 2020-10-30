using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PublicNavigator.Models.Abstract
{
    public interface IUslov
    {
        int Id { get; set; }
        string Path { get; set; }
    }
}