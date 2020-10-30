using Navigator.Models.Concrete;
using System;

namespace Navigator.Models.Abstract
{
    public interface IIzvodLogs
    {
        int LogId { get; set; }
        string UserName { get; set; }
        string OpfatIme { get; set; }
        string BrParcela { get; set; }
        DateTime Datum { get; set; }
        string Path { get; set; }
    }
}