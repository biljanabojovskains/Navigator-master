using System.Collections.Generic;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class Preklop : IPreklop
    {
        public IGradParceli Nova { get; set; }

        public List<IGradParceli> Stari { get; set; }
    }
}