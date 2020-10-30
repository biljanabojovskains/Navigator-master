using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Abstract
{
    public interface ITipBaranje
    {
        int TipBaranjeId { get; set; }
        string TipBaranjeName { get; set; }
    }
}