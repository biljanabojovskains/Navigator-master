using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class KatOpstina : IKatOpstina
    {
        public int Id { get; set; }

        public string Ime { get; set; }
    }
}