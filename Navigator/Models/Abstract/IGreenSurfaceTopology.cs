using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigator.Models.Abstract
{
    public interface IGreenSurfaceTopology
    {
        int Id { get; set; }
        string Name { get; set; }
        int? Fk_Season { get; set; }
    }
}
