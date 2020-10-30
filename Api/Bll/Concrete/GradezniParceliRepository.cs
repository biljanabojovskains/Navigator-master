using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using Api.Bll.Abstract;
using Api.Dal.Concrete;
using Api.ViewModels.Concrete;
using ClosedXML.Excel;
using System.Configuration;

namespace Api.Bll.Concrete
{
    public class GradezniParceliRepository : IGradezniParceliRepository
    {
        public List<GradParceli> GetByBuffer(double lon, double lat)
        {
            return new GradParceliDa().Get(lon, lat);
        }

        public List<GradParceli> GetByOpfat(int opfatId)
        {
            return new GradParceliDa().GetByOpfat(opfatId);
        }

        public GradParceli GetByParcela(int id)
        {
            return new GradParceliDa().Get(id);
        }

        public string GetGeom(int id)
        {
            var geom = new GradParceliDa().GetGeom(id);
            geom = geom.Substring(9, geom.Length - 2 - 9);
            return geom;
        }

        public string GetByParcelaExcel(int id)
        {
            var parcela = new GradParceliDa().Get(id);
            if (parcela == null) return null;

            var downloadDirectory = HttpRuntime.AppDomainAppPath + "Excel\\";
            if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
            var myGuid = Guid.NewGuid();
            var outFileName = string.Format("Excel_{0}_{1}.xlsx", parcela.Id, myGuid);
            var fs = downloadDirectory + outFileName;
            using (var workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var ws = workbook.AddWorksheet(parcela.Broj);
                ws.Cell(1, 1).Value = "Број";
                ws.Cell(1, 2).Value = parcela.Broj;
                ws.Cell(2, 1).Value = "Катност";
                ws.Cell(2, 2).Value = parcela.Katnost;
                ws.Cell(3, 1).Value = "Класа на намена";
                ws.Cell(3, 2).Value = parcela.KlasaNamena;
                ws.Cell(4, 1).Value = "Коефициент на искористенст";
                ws.Cell(4, 2).Value = parcela.KoeficientIskoristenost;
                ws.Cell(5, 1).Value = "Компатибилна класа на намена";
                ws.Cell(5, 2).Value = parcela.KompKlasaNamena;
                ws.Cell(6, 1).Value = "Максимална висина";
                ws.Cell(6, 2).Value = parcela.MaxVisina;
                ws.Cell(7, 1).Value = "Процент на изграденост";
                ws.Cell(7, 2).Value = parcela.ProcentIzgradenost;
                ws.Cell(8, 1).Value = "Површина";
                ws.Cell(8, 2).Value = parcela.Povrshina;
                ws.Cell(9, 1).Value = "Површина за градење";
                ws.Cell(9, 2).Value = parcela.PovrshinaGradenje;
                ws.Cell(10, 1).Value = "Бруто развиена површина";
                ws.Cell(10, 2).Value = parcela.BrutoPovrshina;
                ws.Cell(11, 1).Value = "Име на планска документација";
                ws.Cell(11, 2).Value = parcela.OpfatIme;
                workbook.SaveAs(fs);
                return outFileName;
            }
        }

        public string GetByParcelaExcel(List<int> ids)
        {
            var parceli = new GradParceliDa().Get(ids);
            if (parceli == null) return null;

            var downloadDirectory = HttpRuntime.AppDomainAppPath + "Excel\\";
            if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
            var myGuid = Guid.NewGuid();
            var outFileName = string.Format("Excel_{0}_{1}.xlsx", parceli[0].Id, myGuid);
            var fs = downloadDirectory + outFileName;
            using (var workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                foreach (var parcela in parceli)
                {
                    var ws = workbook.AddWorksheet(parcela.Broj);
                    ws.Cell(1, 1).Value = "Број";
                    ws.Cell(1, 2).Value = parcela.Broj;
                    ws.Cell(2, 1).Value = "Катност";
                    ws.Cell(2, 2).Value = parcela.Katnost;
                    ws.Cell(3, 1).Value = "Класа на намена";
                    ws.Cell(3, 2).Value = parcela.KlasaNamena;
                    ws.Cell(4, 1).Value = "Коефициент на искористенст";
                    ws.Cell(4, 2).Value = parcela.KoeficientIskoristenost;
                    ws.Cell(5, 1).Value = "Компатибилна класа на намена";
                    ws.Cell(5, 2).Value = parcela.KompKlasaNamena;
                    ws.Cell(6, 1).Value = "Максимална висина";
                    ws.Cell(6, 2).Value = parcela.MaxVisina;
                    ws.Cell(7, 1).Value = "Процент на изграденост";
                    ws.Cell(7, 2).Value = parcela.ProcentIzgradenost;
                    ws.Cell(8, 1).Value = "Површина";
                    ws.Cell(8, 2).Value = parcela.Povrshina;
                    ws.Cell(9, 1).Value = "Површина за градење";
                    ws.Cell(9, 2).Value = parcela.PovrshinaGradenje;
                    ws.Cell(10, 1).Value = "Бруто развиена површина";
                    ws.Cell(10, 2).Value = parcela.BrutoPovrshina;
                    ws.Cell(11, 1).Value = "Име на планска документација";
                    ws.Cell(11, 2).Value = parcela.OpfatIme;
                }
                workbook.SaveAs(fs);
                return outFileName;
            }
        }

        public string GetImage(int id)
        {
            var centroid = new GradParceliDa().GetCentroidById(id);
            var resolution = GetResolutionForScale(1000);
            var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 300, 300);
            var address =
                string.Format(
                    "http://{0}/cgi-bin/mapserv.exe?map=../../apps/{9}/htdocs/{8}.map&request=GetMap&service=WMS&version=1.1.1&layers={8}&styles=&srs=EPSG%3A6316&bbox={1},{2},{3},{4}&width={5}&height={6}&GID={7}&format=image%2Fpng",
                    ConfigurationManager.AppSettings["server"] + ":" + ConfigurationManager.AppSettings["port"],
                    bbox.Left.ToString(new CultureInfo("en-US")), bbox.Bottom.ToString(new CultureInfo("en-US")),
                    bbox.Right.ToString(new CultureInfo("en-US")), bbox.Top.ToString(new CultureInfo("en-US")), 900,
                    900, id, "sintezen2", ConfigurationManager.AppSettings["ms4w_app"]);
            return address;
        }

        /// <summary>
        /// Konvertor na rezolucija vo razmer
        /// </summary>
        /// <param name="scale">razmer</param>
        /// <returns>rezolucija</returns>
        private static double GetResolutionForScale(int scale)
        {
            var dpi = 25.4 / 0.28;
            //var mpu = ol.proj.METERS_PER_UNIT[units];
            var mpu = 1;
            var inchesPerMeter = 39.37;
            return scale / (mpu * inchesPerMeter * dpi);
        }

        /// <summary>
        /// Presmetuva bounding box
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="resolution">Rezolucija</param>
        /// <param name="w">shirina vo pikseli</param>
        /// <param name="h">visina vo pikseli</param>
        /// <returns></returns>
        private static Bbox CalculateBounds(double x, double y, double resolution, int w, int h)
        {
            var halfWDeg = (w * resolution) / 2;
            var halfHDeg = (h * resolution) / 2;
            var bbox = new Bbox { Left = x - halfWDeg, Bottom = y - halfHDeg, Right = x + halfWDeg, Top = y + halfHDeg };
            return bbox;
        }
    }
}