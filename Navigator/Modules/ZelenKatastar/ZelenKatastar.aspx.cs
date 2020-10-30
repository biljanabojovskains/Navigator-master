using System;
using System.Web.Services;
using System.Web.UI;
using Navigator.Bll;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using System.IO;
using System.Web.Hosting;


using iTextSharp.text;
using iTextSharp.text.pdf;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Navigator.Modules.ZelenKatastar
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {

                var modules = ConfigurationManager.AppSettings["mods"].Split(',');
                var modul = modules.Contains("m1zN1");
                if (!modul)
                {
                    Response.Redirect("~/NoAccess");
                }
            }
            base.OnPreLoad(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //FillTreeSeason();
           
        }
        //private void FillTreeSeason()
        //{
        //    var seasontree = Bl.GetTreeSeason();
        //    selectTreeSeason.DataSource = seasontree;
        //    selectTreeSeason.DataTextField = "text";
        //    selectTreeSeason.DataValueField = "id";
        //    selectTreeSeason.DataBind();
        
        //}

        //[WebMethod]
        //public static string FillTreeName(int treeseasonid)
        //{
        //    var item = Bl.GetTreesByName(treeseasonid);
        //    var result = JsonConvert.SerializeObject(item);
        //    return result;
        //}

        [WebMethod]
        public static string GetDrvoGrmushka(string search = null)
       {
           var items = Bl.GetDrvoGrmushka(search);
           var result = JsonConvert.SerializeObject(items);
           return result;
       }

     

        [WebMethod]
        public static string GetTreeShrubName(int? type = null, int? id=null, string search = null)
        {
            var items = Bl.GetTreeShrubName(type, id, search);
            var result = JsonConvert.SerializeObject(items);
            return result;
        }

        [WebMethod]
        public static string GetSeason(int? type = null, string search = null)
        {
            var items = Bl.GetSeason(type, search);
            var result = JsonConvert.SerializeObject(items);
            return result;
        }

        [WebMethod]
        public static string GetStreets(string search = null)
        {
            var items = Bl.GetStreetsPolygons(search);
            var result = JsonConvert.SerializeObject(items);
            return result;
        }

        [WebMethod]
        public static string SearchOutA(int treeShrub, int season, string name)
        {
            var json = "";
            var item = Bl.SearchTreeShrub(treeShrub, season, name);
            json = JsonConvert.SerializeObject(item);
            return json;

        }

        [WebMethod]
        public static string InfoZeleniloTool(string coordinates)
        {
            var item = Bl.GetInfo(coordinates);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

         [WebMethod]
        public static string ReportTreeShrub(int? treeShrub = null)
        {
            var item = Bl.GetTreeShrubCount(treeShrub);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

         [WebMethod]
         public static string SearchOutUlici(string searchString)
         {
             var item = Bl.SearchUliciZelenilo(searchString);
             var json = JsonConvert.SerializeObject(item);
             return json;
         }


         [WebMethod]

         public static string ExportReport(int tip)
         {
             string link;
             int formatType = 0;
            // if (formatType != 5)
            // {
                 link = Helpers.PhantomJsHelper.ExportSvfMap(tip);

                 if (formatType == 1)
                 {
                     using (new MemoryStream())
                     {
                         iTextSharp.text.Rectangle pageSize;
                         string srcFilename = HostingEnvironment.ApplicationPhysicalPath + "\\ExportMap\\" + link;

                         using (var srcImage = new Bitmap(srcFilename))
                         {
                             pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
                         }
                         using (var ms = new MemoryStream())
                         {
                             var document = new Document(pageSize, 0, 0, 0, 0);
                             PdfWriter.GetInstance(document, ms).SetFullCompression();
                             document.Open();
                             var image = iTextSharp.text.Image.GetInstance(srcFilename);
                             document.Add(image);
                             document.Close();

                             var oldLink = link;
                             link = Path.GetFileNameWithoutExtension(link) + ".PDF";

                             File.WriteAllBytes(HostingEnvironment.ApplicationPhysicalPath + "\\ExportMap\\" + link,
                                 ms.ToArray());
                             File.Delete(HostingEnvironment.ApplicationPhysicalPath + "\\ExportMap\\" + oldLink);
                         }
                     }
                 }
             //}
             //else
             //{
                 // link = MapBl.ExportMapToDxf(extminx, extminy, extmaxx, extmaxy);
             //}

           //  var id = HttpContext.Current.User.Identity as FormsIdentity;
            // var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);

            // string path = "../ExportMap/" + link;
            // MapBl.InsertExportMap(mapTitle, user.UserId, path);
             return link;
         }
      
    }
}