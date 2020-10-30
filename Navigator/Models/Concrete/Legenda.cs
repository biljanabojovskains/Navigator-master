using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class Legenda : ILegenda
    {
        public int Id { get; set; }

        public int OpfatId { get; set; }

        public int TipNaPodatokId { get; set; }

        public string Path { get; set; }
    }
}