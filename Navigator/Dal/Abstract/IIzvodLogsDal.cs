using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Navigator.Models.Abstract;

namespace Navigator.Dal.Abstract
{
    public interface IIzvodLogsDal
    {
        bool Insert(string username, string opfafIme, string brParcela, string path);
        List<IIzvodLogs> GetAll();
    }
}