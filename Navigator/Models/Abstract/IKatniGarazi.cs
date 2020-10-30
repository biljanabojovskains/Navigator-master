using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public interface IKatniGarazi
    {
        int KatniGaraziId { get; set; }
        string KatniGaraziName { get; set; }
    }
}