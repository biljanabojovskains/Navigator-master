using System;
using System.Web.Services;
using System.Web.UI;
using Navigator.Bll;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;
using System.IO;

namespace Navigator.Maps
{
    public partial class General : Page
    {
        bool someFlag = false;
        protected override void OnPreLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                var modules = ConfigurationManager.AppSettings["mods"].Split(',');
                var modulA = modules.Contains("a1pN1");
                var modulC = modules.Contains("c1pN1");
                var modulD = modules.Contains("dxf1pN1");
                var modulV = modules.Contains("aV1pN1");
                var modulK = modules.Contains("aK1pN1");
                var modulP = modules.Contains("aP1pN1");
                //var modulCUlici = modules.Contains("aC1pN1");
                var module = ConfigurationManager.AppSettings["opstina"].Split(',');
                var modulPrilep = module.Contains("ПРИЛЕП");
                var modulKavadarci = module.Contains("КАВАДАРЦИ");
                var modulAerodrom = module.Contains("АЕРОДРОМ");
                var modulGaziBaba = module.Contains("ГАЗИ БАБА");
                var modulCentar = module.Contains("ЦЕНТАР");
                var modulKumanovo = module.Contains("КУМАНОВО");

                if (modulA)
                {
                    adresi.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                    btnInsertDoc.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                    btnInfoDoc.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                    btnVnesInvestitor.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                    btnStreetCut.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                    btnPolygon.Visible = true;
                    btnLine.Visible = true;
                }
                if (modulD )
                {
                    btnDownloadDxf.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                }
                if(modulKumanovo){
                    btnDownloadDxf.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                }
                if (modulC)
                {
                    kp.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                    //adresi.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                }
               
                if (modulV)
                {
                    adresi.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                }
                if (modulK)
                {
                    adresi.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                }
                if (modulP)
                {
                    adresi.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                }
                if (modulKavadarci || modulKumanovo )
                {
                    btnPolygon.Visible = true;
                    btnLine.Visible = true;
                 
                }
                if (modulPrilep  || modulGaziBaba)
                {
                    btnPolygon.Visible = false;
                    btnLine.Visible = false;

                }
                if (modulCentar)
                {
                    btnPolygon.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                    btnLine.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                    btnStreetCut.Attributes.CssStyle[HtmlTextWriterStyle.Visibility] = "visible";
                }

            
              
               
               


            }
            base.OnPreLoad(e);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            var modules = ConfigurationManager.AppSettings["mods"].Split(',');
            var modulA = modules.Contains("a1pN1");
            var modulV = modules.Contains("aV1pN1");
            var modulK = modules.Contains("aK1pN1");
            var modulP = modules.Contains("aP1pN1");
            var modulC = modules.Contains("aC1pN1");
            if (modulA)
            {
                FillStreets();
            }
            if (modulV)
            {
                FillVelesStreets();
            }
            if (modulK)
            {
                FillAllStreets();
            }
            if (modulP)
            {
                FillAllStreets();
            }
            if (modulC)
            {
                //FillCentarStreets();
            }
            FillOpfati();
        }
        private void FillStreets()
        {
            var ulica = Bl.GetAllStreets();
            selectUlica.DataSource = ulica;
            selectUlica.DataTextField = "text";
            selectUlica.DataValueField = "id";
            selectUlica.DataBind();
        }
        private void FillAllStreets()
        {
            var ulica = Bl.GetStreets();
            selectUlica.DataSource = ulica;
            selectUlica.DataTextField = "text";
            selectUlica.DataValueField = "id";
            selectUlica.DataBind();
        }
        private void FillVelesStreets()
        {
            var ulica = Bl.GetAllVStreets();
            selectUlica.DataSource = ulica;
            selectUlica.DataTextField = "text";
            selectUlica.DataValueField = "id";
            selectUlica.DataBind();
        }
        //private void FillCentarStreets()
        //{
        //    var ulica = Bl.GetCentarStreets();
        //    selectUlicaN.DataSource = ulica;
        //    selectUlicaN.DataTextField = "text";
        //    selectUlicaN.DataValueField = "id";
        //    selectUlicaN.DataBind();
        //}
        private void FillOpfati()
        {
            var opfat = Bl.GetAllOpfati();
            selectOpfat.DataSource = opfat;
            selectOpfat.DataTextField = "text";
            selectOpfat.DataValueField = "id";
            selectOpfat.DataBind();
        }
        [WebMethod]
        public static string FillNumbers(string ulica)
        {
            var modules = ConfigurationManager.AppSettings["mods"].Split(',');
            var modulA = modules.Contains("a1pN1");
            var result="";
            var modulV = modules.Contains("aV1pN1");
            var modulK = modules.Contains("aK1pN1");
            var modulP = modules.Contains("aP1pN1");
            var modulC = modules.Contains("aC1pN1");
            if (modulA)
            {
                var item = Bl.GetAllNumbers(ulica);
                result = JsonConvert.SerializeObject(item);
            }
            if (modulV)
            {
                var item = Bl.GetAllVNumbers(ulica);
                result = JsonConvert.SerializeObject(item);
            }
            if (modulK)
            {
                var item = Bl.GetAllStreetNumbers(ulica);
                result = JsonConvert.SerializeObject(item);
            }
            if (modulP)
            {
                var item = Bl.GetAllStreetNumbers(ulica);
                result = JsonConvert.SerializeObject(item);
            }
            if (modulC)
            {
                var item = Bl.GetAllCStreetNumbers(ulica);
                result = JsonConvert.SerializeObject(item);
            }
            return result;
        }
        [WebMethod]
        public static string FillParceli(string opfat)
        {
            var item = Bl.GetAllGParceli(opfat);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }
        
        [WebMethod]
        public static string InfoTool(string coordinates)
        {
            var item = Bl.GetInfo(coordinates);
            string output = JsonConvert.SerializeObject(item);
            return output;
        }

        [WebMethod]
        public static string GetListAdresi(string poligon)
         {
            var v1= poligon.Replace(",", " ");
            var v2 = v1.Replace("] [", ",");
            var v3 = v2.Replace("[", "");
            var final = v3.Replace("]", "");
            var item = Bl.GetAdresi(final);
            var json = JsonConvert.SerializeObject(item);
            return json;
        }

        [WebMethod]
        public static string IzvodTool(string coordinates)
        {
            var opstina = ConfigurationManager.AppSettings["opstina"];
            string filename;
            if (opstina == "ЦЕНТАР")
                filename = Bl.GenerateIzvodCentar(coordinates);
            else if (opstina == "АЕРОДРОМ")
                filename = Bl.GenerateIzvodAerodrom(coordinates);
            else if (opstina == "ГАЗИ БАБА")
                filename = Bl.GenerateIzvodGaziBaba(coordinates);
            else if ( opstina == "ПРИЛЕП" )
                filename = Bl.GenerateIzvodPrilep(coordinates);
            else if (opstina == "КАВАДАРЦИ")
                filename = Bl.GenerateIzvodKavadarci(coordinates);
            else if (opstina == "КУМАНОВО")
                filename = Bl.GenerateIzvodKumanovo(coordinates);
            else
                filename = Bl.GenerateIzvod(coordinates);
            return filename;
        }


        [WebMethod]
        public static string IzvodUlica(string poligon, string ulica_id)
        {
            
            var opstina = ConfigurationManager.AppSettings["opstina"];
            var v1 = poligon.Replace(",", " ");
            var v2 = v1.Replace("] [", ",");
            var v3 = v2.Replace("[", "");
            var final = v3.Replace("]", "");
            string filename;
            if (opstina == "ЦЕНТАР")
                filename = Bl.GenerateIzvodUlicaCentar(final, int.Parse(ulica_id));

            else if (opstina == "АЕРОДРОМ")
            {

                if (ulica_id == "null")
                {
                    filename = Bl.GenerateIzvodUliciAerodrom(final);
                }
                else
                {
                    filename = Bl.GenerateIzvodUlicaAerodrom(final, int.Parse(ulica_id));
                }

            }

            else
                filename = Bl.GenerateIzvodUlicaCentar(final, int.Parse(ulica_id));
            return filename;
        }

        

        [WebMethod]
        public static string SearchOut(string searchString)
        {
            if (searchString.Contains("quotes"))
            {
                searchString = searchString.Replace("quotes", "\"");
            }
            var item = Bl.SearchAll(searchString);
            var json = JsonConvert.SerializeObject(item);
            return json;
        }

        [WebMethod]
        public static string SearchOutK(string searchString)
        {
            var item = Bl.SearchKatastarskiParceli(searchString);
            var json = JsonConvert.SerializeObject(item);
            return json;
        }
        [WebMethod]
        public static string SearchOutA(string ulica, string broj)
        {
            var modules = ConfigurationManager.AppSettings["mods"].Split(',');
            var modulA = modules.Contains("a1pN1");
            var modulV = modules.Contains("aV1pN1");
            var modulK = modules.Contains("aK1pN1");
            var modulP = modules.Contains("aP1pN1");
            var modulC = modules.Contains("aC1pN1");
            var json = "";
            if (modulA)
            {
                var item = Bl.SearchAdresa(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            if (modulV)
            {
                var item = Bl.SearchVAdresa(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            if (modulK)
            {
                var item = Bl.SearchAdress(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            if (modulP)
            {
                var item = Bl.SearchAdress(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            if (modulC)
            {
                var item = Bl.SearchCAdress(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            return json;
        }


        public static string SearchOutUlici(string ulica, string broj,string coordinates)
        {
            var modules = ConfigurationManager.AppSettings["mods"].Split(',');
            var modulA = modules.Contains("a1pN1");
            var modulV = modules.Contains("aV1pN1");
            var modulK = modules.Contains("aK1pN1");
            var modulP = modules.Contains("aP1pN1");
            var modulC = modules.Contains("aC1pN1");
            var json = "";
            if (modulA)
            {
                var item = Bl.SearchAdresa(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            if (modulV)
            {
                var item = Bl.SearchVAdresa(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            if (modulK)
            {
                var item = Bl.SearchAdress(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            if (modulP)
            {
                var item = Bl.SearchAdress(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            if (modulC)
            {
                var item = Bl.SearchCAdress(ulica, broj);
                json = JsonConvert.SerializeObject(item);
            }
            return json;
        }




        [WebMethod]
        public static string SearchOutGP(int id)
        {
            var item = Bl.SearchParcela(id);
            var json = JsonConvert.SerializeObject(item);
            return json;
        }
        [WebMethod]
         public static string GenerateDxfFile(string coordinates)
         {
             var dxfnearby = Bl.GenerateDxfNearby(coordinates);
             return dxfnearby;
         }      
         [WebMethod]
         public static string InfoDocTool(string coordinates)
         {
             var item = Bl.GetGeneralDoc(coordinates);
             var result = JsonConvert.SerializeObject(item);
             return result;
         }

         protected void btnInvestitor_Click(object sender, EventArgs e)
         {
             string id = fkDocParcel.Value;
             int fkParcela = Convert.ToInt32(id);
             if( vnesInvestitor.Value != "" && vnesInvestitor != null)
             {
                 var insert = Bl.InsertInestitor(fkParcela, vnesInvestitor.Value);
                 vnesInvestitor.Value = "";
             }
         }
         protected void btnFileUpload_Click(object sender, EventArgs e)
         {
             string id = fkDocParcel.Value;
             int fkParcela = Convert.ToInt32(id);
             string pathDB = "";
             try
             {
                 if (file_upload.HasFile)
                 {
                     foreach (var file in file_upload.PostedFiles)
                     {
                         string ext = System.IO.Path.GetExtension(file.FileName);
                         string name = Guid.NewGuid() + ext;
                         pathDB = "Data/" + name;
                         file_upload.SaveAs(Server.MapPath("~/Data/") + name);
                         var insert = Bl.InsertGeneralDoc(fkParcela, pathDB);
                     }
                 }
                 else
                 {
                     Response.Write("<script>alert('Не избравте докуемнт');</script>");
                 }
             }
             catch (Exception ex)
             {
                 lblUploadStatus.Text = "Грешка при прикачување на документот." + ex.Message;
             }

             fkDocParcel.Value = "";
             pathDB = "";
         }

        
    }    
}