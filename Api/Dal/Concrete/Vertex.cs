using Api.Dal.Abstract;

namespace Api.Dal.Concrete
{
    public class Vertex : IVertex
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}