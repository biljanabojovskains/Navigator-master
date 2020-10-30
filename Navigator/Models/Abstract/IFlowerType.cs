using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigator.Models.Abstract
{
    public interface IFlowerType
    {
        int Id { get; set; }
        string Name { get; set; }
        string LatinName { get; set; }
        int? Fk_Topology { get; set; }
    }
}
