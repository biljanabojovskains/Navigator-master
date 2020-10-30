using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using Navigator.Dal.Concrete;
using NLog;
using Novacode;

namespace Navigator.Admin
{
    public partial class prep : Page
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected void Page_Load(object sender, EventArgs e)
        {
            MyFunc();
        }

        private void MyFunc()
        {
            var list = new OpstiUsloviDa().Prep();
            foreach (var item in list)
            {
                if (item.Length > 3)
                {
                    var downloadDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                    if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                    var outFileName = string.Format("pu_{0}.docx", item);
                    string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\blank_0.docx";
                    string docOutputPath = downloadDirectory + outFileName;
                    ////create copy of template so that we don't overwrite it
                    File.Copy(docTemplatePath, docOutputPath);
                    // Load a .docx file
                    using (DocX document = DocX.Load(docOutputPath))
                    {
                        var imetabela = item.Split('_')[0];
                        var stranici = item.Split('_')[1];
                        foreach (var grupa in stranici.Split(';'))
                        {
                            int start = 0;
                            int stop = 0;
                            if (grupa.Contains('-'))
                            {
                                start = int.Parse(grupa.Split('-')[0]);
                                stop = int.Parse(grupa.Split('-')[1]);
                            }
                            else
                            {
                                start = stop = int.Parse(grupa);
                            }
                            for (int i = start; i <= stop; i++)
                            {
                                try
                                {
                                    Image image =
                                        document.AddImage(downloadDirectory + "izvod\\" + imetabela + "\\" + "uslovi_" +
                                                          i +
                                                          ".jpg");
                                    var picture = image.CreatePicture(1100, 778);
                                    var paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                            }
                        }
                        document.Save();
                    }
                }
            }
        }
    }
}