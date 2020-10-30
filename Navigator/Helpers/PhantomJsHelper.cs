//using NReco.PhantomJS;

using System;
using System.Configuration;
using System.IO;
using System.Web;
using NReco.PhantomJS;

namespace Navigator.Helpers
{
    /// <summary>
    /// Export files to image or PDF
    /// </summary>
    public static class PhantomJsHelper
    {
        public static string ExportSvfMap(int tip)
        {
            var phantomJs = new PhantomJS();
            string webUri = "http://localhost:" + ConfigurationManager.AppSettings["httpPort"] + "/";
            string theUrl = webUri + "Export?tip=" + tip;

            int formatType = 0;
            int formatPaperType = 1;
            string pageOrientation = "";
            string pictureFormat;
            int dpi = 150;

            switch (formatType)
            {
                case 1:
                    //pictureFormat = "PDF";
                    pictureFormat = "PNG";
                    break;
                case 2:
                    pictureFormat = "PNG";
                    break;
                case 4:
                    pictureFormat = "JPG";
                    break;
                default:
                    pictureFormat = "PDF";
                    break;
            }

            const double cmToInchFactor = 0.393701;
            double widthInInches;
            double heightInInches;
            string paperFormat;
            switch (formatPaperType)
            {
                case 1:
                    paperFormat = "A4";
                    widthInInches = 8.27;
                    heightInInches = 11.69;
                    break;
                case 2:
                    paperFormat = "A3";
                    widthInInches = 11.69;
                    heightInInches = 16.54;
                    break;
                case 3:
                    widthInInches = 16.5;
                    heightInInches = 23.4;
                    paperFormat = "A2";
                    break;
                default:
                    paperFormat = "A4";
                    widthInInches = 8.27;
                    heightInInches = 11.69;
                    break;
            }

            //reduce by the margin (assuming 1cm margin on each side)
            widthInInches -= 2*cmToInchFactor;
            heightInInches -= 2*cmToInchFactor;
            //interchange if width is equal to height
            if (pageOrientation == "landscape")
            {
                var temp = widthInInches;
                widthInInches = heightInInches;
                heightInInches = temp;
            }
            //calculate corresponding viewport dimension in pixels
            var pdfViewportWidth = (int) (dpi*widthInInches);
            var pdfViewportHeight = (int) (dpi*heightInInches);

            //string fileName = Dal.Helpers.Files.GenerateUniqueFilename("png", "svf_");
            string fileName = DateTime.Now.ToString("yyyyMMddHHmm") + "." + pictureFormat;
            string filePath = "../ExportMap/" + fileName;
            string serverPath = HttpContext.Current.Server.MapPath("~/ExportMap/") + fileName;

            phantomJs.RunScript(@"
                var page = require('webpage').create();
                page.viewportSize = { width: " + pdfViewportWidth + @", height: " + pdfViewportHeight + @"};       
                page.settings.resourceTimeout = 60000;
                page.paperSize = {
                    format: '" + paperFormat + @"',
                    margin: '1cm',
                    orientation: '" + pageOrientation + @"',
                    width: '" + pdfViewportWidth + @"px',
                    height: '" + pdfViewportHeight + @"px', 
                };
                
	                page.open('" + theUrl + @"', function() {
		                setInterval(function(){
			                page.render('" + filePath + @"');			
			                phantom.exit();
		                }, 20000);
	                });	
                ",
                null);

            if (File.Exists(serverPath))
            {
                return fileName;
            }

            return "";
        }

        /// <summary>
        /// Export of the reports to PDF
        /// </summary>
        /// <param name="url">Link to the report</param>
        /// <param name="orientation">Landscape or Portrait orientation</param>
        /// <returns>Link to the generated pdf file</returns>
        public static string ExportReportPdf(string url, string orientation)
        {
            var phantomJs = new PhantomJS();

            string webUri = ConfigurationManager.AppSettings.Get("WebURI");

            const string fileName = "";
            const string filePath = "../Img/ReportsPdf/" + fileName;
            string serverPath = HttpContext.Current.Server.MapPath("~/Img/ReportsPdf/") + fileName;

            phantomJs.RunScript(@"
                var page = require('webpage').create();
                page.viewportSize = { width: 1754, height: 1240};
                page.settings.resourceTimeout = 20000;
                page.paperSize = {
                    format: 'A4',
                    orientation: '" + orientation + @"',
                    margin: '1cm' 
                };

                page.open('" + webUri + "Account/Login" + @"', function() {
	                page.open('" + url + @"', function() {
		                setInterval(function(){			
			                var title = page.evaluate(function() {				
				                $('nav').hide();				
			                });
			                page.render('" + filePath + @"');
			
			                phantom.exit();
		                }, 5000);   
	                });	
                });",
                null);

            if (File.Exists(serverPath))
            {
                return "Img/ReportsPdf/" + fileName;
            }

            return "";
        }
    }
}