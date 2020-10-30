using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class GradskaCetvrt : IGradskaCetvrtMakro
    {
        public string Id { get; set; }

        public string Ime { get; set; }

        public double PovrshinaPresmetana { get; set; }

        public void QcInput(IGradskaCetvrtMakro source)
        {
            return;
        }
        public string GeoJson { get; set; }
    }
}
