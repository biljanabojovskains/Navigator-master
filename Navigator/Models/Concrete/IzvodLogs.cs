using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    [Serializable]
    public class IzvodLogs : IIzvodLogs
    {
        public int LogId { get; set; }
        public string UserName { get; set; }
        public string OpfatIme { get; set; }
        public string BrParcela { get; set; }
        public DateTime Datum { get; set; }
        public string Path { get; set; }
    }
}