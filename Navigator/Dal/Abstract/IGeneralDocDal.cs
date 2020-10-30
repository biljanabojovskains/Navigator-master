using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;

namespace Navigator.Dal.Abstract
{
    public interface IGeneralDocDal
    {
        List<IGeneralDoc> GetGeneralDocuments(string coordinates);
        bool InsertGeneralDocument(int fkParcela, string path);
    }
}