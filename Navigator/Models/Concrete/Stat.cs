using System;
using Navigator.Models.Abstract;

namespace Navigator.Models.Concrete
{
    public class Stat : IStat
    {
        public double? BrutoPovrsina { get; set; }
        public double? PovrsinaGradezniParceli { get; set; }
        public double? PovrsinaPresmetana { get; set; }
        public double? KoeficientIskeristenost { get; set; }
        public double? ZelenaPovrsina { get; set; }
        public double? ZelenaPovrsinaPresmetana { get; set; }
        public double? GradeznaPovrsina { get; set; }
        public double? GradeznaPovrsinaPresmetana { get; set; }
        public double? Odnos
        {
            get
            {
                if (GradeznaPovrsina == 0)
                    return null;
                if (ZelenaPovrsina == 0)
                    return 0;
                double presmetka;
                presmetka = ((double.Parse(ZelenaPovrsina.ToString()) / double.Parse(GradeznaPovrsina.ToString())) * 100);
                return Math.Round(presmetka, 2);
            }          
        }
        public double? OdnosPresmetan
        {
            get
            {
                if (GradeznaPovrsinaPresmetana == 0)
                    return null;
                if (ZelenaPovrsinaPresmetana == 0)
                    return 0;
                double presmetka;
                presmetka = ((double.Parse(ZelenaPovrsinaPresmetana.ToString()) / double.Parse(GradeznaPovrsinaPresmetana.ToString())) * 100); 
                return Math.Round(presmetka, 2);
            }
        }
    }
}