using Navigator.Models.Concrete;

namespace Navigator.Models.Abstract
{
    public interface IStat
    {
        double? BrutoPovrsina { get; set; }
        double? PovrsinaGradezniParceli { get; set; }
        double? PovrsinaPresmetana { get; set; }
        double? KoeficientIskeristenost { get; set; }
        double? ZelenaPovrsina { get; set; }
        double? ZelenaPovrsinaPresmetana { get; set; }
        double? GradeznaPovrsina { get; set; }
        double? GradeznaPovrsinaPresmetana { get; set; }
        double? Odnos { get; }
        double? OdnosPresmetan { get; }
    }
}
