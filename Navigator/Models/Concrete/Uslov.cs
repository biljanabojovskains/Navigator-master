﻿using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class Uslov : IUslov
    {
        public int Id { get; set; }

        public string Path { get; set; }
    }
}