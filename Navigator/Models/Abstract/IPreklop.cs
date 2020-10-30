using System.Collections.Generic;
using Navigator.Models.Concrete;

namespace Navigator.Models.Abstract
{
    public interface IPreklop
    {
        IGradParceli Nova { get; set; }
        List<IGradParceli> Stari { get; set; } 
    }
}
