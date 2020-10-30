using Api.Dal.Abstract;

namespace Api.Dal.Concrete
{
    public class Bbox : IBbox
    {
        public double Left { get; set; }
        public double Bottom { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
    }
}