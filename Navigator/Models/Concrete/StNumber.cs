﻿using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Navigator.Models.Concrete
{
    public class StNumber : IStNumber
    {
        public string text { get; set; }
        public string id { get; set; }
    }
}