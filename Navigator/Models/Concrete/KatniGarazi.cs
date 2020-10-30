using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    [Serializable]
    public class KatniGarazi : IKatniGarazi
    {
        public int KatniGaraziId { get; set; }
        public string KatniGaraziName { get; set; }
    }
}