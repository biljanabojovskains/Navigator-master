﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigator.Models.Abstract
{
    public interface IFlowerSeason
    {
        int Id { get; set; }
        string Name { get; set; }
    }
}