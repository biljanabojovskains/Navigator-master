using PublicNavigator.Models.Abstract;

namespace PublicNavigator.Models.Concrete
{

    public class Uslov : IUslov
    {
        public int Id { get; set; }

        public string Path { get; set; }
    }
}