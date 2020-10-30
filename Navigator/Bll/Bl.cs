using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using Ionic.Zip;
using Navigator.Dal.Concrete;
using Navigator.Helpers;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using Newtonsoft.Json;
using NLog;
using Novacode;
using System.Diagnostics;
using System.Globalization;


namespace Navigator.Bll
{
    public class Bl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dava info na site sloevi koi se naogaat pod taa tocka
        /// </summary>
        /// <param name="coordinates">koordinati vo format x,y</param>
        /// <returns>DrillDownInfo objekt</returns>
        public static IDrillDownInfo GetInfo(string coordinates)
        {
            var info = new DrillDownInfoDa().GetInfo(coordinates);
            return info;
        }

        //public static List<IZelenilo> GetInfoZelenilo(string coordinates)
        //{
        //    var info = new TreeShrubDa().GetZelenilo(coordinates);
        //    return info;
        //}


        /// <summary>
        /// Generira izvod od plan
        /// </summary>
        /// <param name="coordinates">koordinati vo format x,y</param>
        /// <returns>file path</returns>
        public static string GenerateIzvod(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod.docx";
                string docOutputPath = downloadDirectory + outFileName;
                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);
                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {
                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));
                    document.AddCustomProperty(new CustomProperty("ime_plan", opfat.Ime));
                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                    document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                    document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                    document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                    document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                    document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                    document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                    document.AddCustomProperty(parcela.Povrshina != null
                        ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                        : new CustomProperty("povrsina", ""));
                    document.AddCustomProperty(parcela.PovrshinaGradenje != null
                        ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                        : new CustomProperty("povrsina_za_gradba", ""));
                    document.AddCustomProperty(parcela.BrutoPovrshina != null
                        ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                        : new CustomProperty("bruto_razviena", ""));
                    document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                    document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                        ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                        : new CustomProperty("iskoristenost", ""));
                    document.AddCustomProperty(parcela.ProcentIzgradenost != null
                        ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                        : new CustomProperty("izgradenost", ""));

                    //generate picture
                    var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                    var resolution = GetResolutionForScale(1000);
                    var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "sintezen2");

                    Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                    // Create a picture (A custom view of an Image).
                    Picture picture = image.CreatePicture(420, 630);
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph = document.Paragraphs[12];
                    paragraph.InsertPicture(picture);
                    paragraph.Alignment = Alignment.center;
                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "komunalen");
                                    image = document.AddImage(downloadDirectory + myGuid + "komunalen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "soobrakaen");
                                    image = document.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 4:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "spomenici");
                                    image = document.AddImage(downloadDirectory + myGuid + "spomenici");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "parking");
                                    image = document.AddImage(downloadDirectory + myGuid + "parking");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 6:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "infrastrukturen");
                                    image = document.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 7:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "zelenilo");
                                    image = document.AddImage(downloadDirectory + myGuid + "zelenilo");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;



                        }
                    }

                    document.Save();
                } // Release this document from memory.

                //generate Zip
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var folderName = year + "" + month;
                if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                    Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                List<string> filesToAdd = new List<string>();
                filesToAdd.Add(docOutputPath);

                var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                if (parcela.OpstiUsloviId.HasValue)
                {
                    var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                        uri = usloviDirectory + opsti.Path;
                    else
                        uri = opsti.Path;
                    filesToAdd.Add(uri);
                }
                if (parcela.PosebniUsloviId.HasValue)
                {
                    var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                        uri = usloviDirectory + posebni.Path;
                    else
                        uri = posebni.Path;
                    filesToAdd.Add(uri);
                }
                var ime = "Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" + Guid.NewGuid() + ".zip";
                var fullPath = folderName + "\\" + ime;

                CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                DeleteFilesFromSystem(new List<string> { docOutputPath });

                InsertLogs(user.UserName, opfat.Ime, parcela.Broj, year + "" + month + "\\" + ime);
                return fullPath;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }

        public static string GenerateIzvodKumanovo(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod.docx";
                string docOutputPath = downloadDirectory + outFileName;
                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);
                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {
                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));
                    document.AddCustomProperty(new CustomProperty("ime_plan", opfat.Ime));
                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                    document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                    document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                    document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                    document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                    document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                    document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                    document.AddCustomProperty(parcela.Povrshina != null
                        ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                        : new CustomProperty("povrsina", ""));
                    document.AddCustomProperty(parcela.PovrshinaGradenje != null
                        ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                        : new CustomProperty("povrsina_za_gradba", ""));
                    document.AddCustomProperty(parcela.BrutoPovrshina != null
                        ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                        : new CustomProperty("bruto_razviena", ""));
                    document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                    document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                        ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                        : new CustomProperty("iskoristenost", ""));
                    document.AddCustomProperty(parcela.ProcentIzgradenost != null
                        ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                        : new CustomProperty("izgradenost", ""));

                    //generate picture
                    var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                    var resolution = GetResolutionForScale(1000);
                    var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "sintezen2");

                    Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                    // Create a picture (A custom view of an Image).
                    Picture picture = image.CreatePicture(420, 630);
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph = document.Paragraphs[12];
                    //paragraph.InsertPicture(picture);
                    //paragraph.Alignment = Alignment.center;
                    document.InsertSectionPageBreak();
                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Синтезна - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "sintezen2");
                                    image = document.AddImage(downloadDirectory + myGuid + "sintezen2");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "komunalen");
                                    image = document.AddImage(downloadDirectory + myGuid + "komunalen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "soobrakaen");
                                    image = document.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 4:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "spomenici");
                                    image = document.AddImage(downloadDirectory + myGuid + "spomenici");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "parking");
                                    image = document.AddImage(downloadDirectory + myGuid + "parking");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 6:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "infrastrukturen");
                                    image = document.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 7:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "zelenilo");
                                    image = document.AddImage(downloadDirectory + myGuid + "zelenilo");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;



                        }
                    }

                    document.Save();
                } // Release this document from memory.

                //generate Zip
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var folderName = year + "" + month;
                if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                    Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                List<string> filesToAdd = new List<string>();
                filesToAdd.Add(docOutputPath);

                var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                if (parcela.OpstiUsloviId.HasValue)
                {
                    var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                        uri = usloviDirectory + opsti.Path;
                    else
                        uri = opsti.Path;
                    filesToAdd.Add(uri);
                }
                if (parcela.PosebniUsloviId.HasValue)
                {
                    var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                        uri = usloviDirectory + posebni.Path;
                    else
                        uri = posebni.Path;
                    filesToAdd.Add(uri);
                }
                var ime = "Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" + Guid.NewGuid() + ".zip";
                var fullPath = folderName + "\\" + ime;

                CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                DeleteFilesFromSystem(new List<string> { docOutputPath });

                InsertLogs(user.UserName, opfat.Ime, parcela.Broj, year + "" + month + "\\" + ime);
                return fullPath;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }
        /// <summary>
        /// Generira izvod od plan
        /// </summary>
        /// <param name="coordinates">koordinati vo format x,y</param>
        /// <returns>file path</returns>
        public static string GenerateIzvodCentar(string coordinates)
        {
            try
            {
                
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath;

                if (parcela.BrTehIspravka != null && parcela.DatumTehIspravka != null)
                     docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_centar_ti.docx";
                else
                     docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_centar.docx";
                 
                string docOutputPath = downloadDirectory + outFileName;
                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);

                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {

                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));
                    document.AddCustomProperty(new CustomProperty("ime_plan", opfat.Ime));
                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                    document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                    document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                    document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                    document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                    document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                    document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                    document.AddCustomProperty(parcela.Povrshina != null
                        ? new CustomProperty("povrsina", parcela.PovrshinaZaokruzena)
                        : new CustomProperty("povrsina", ""));
                    document.AddCustomProperty(parcela.PovrshinaGradenje != null
                        ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenjeZaokruzena)
                        : new CustomProperty("povrsina_za_gradba", ""));
                    document.AddCustomProperty(parcela.BrutoPovrshina != null
                        ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshinaZaokruzena)
                        : new CustomProperty("bruto_razviena", ""));
                    document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                    document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                        ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                        : new CustomProperty("iskoristenost", ""));
                    document.AddCustomProperty(parcela.ProcentIzgradenost != null
                        ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                        : new CustomProperty("izgradenost", ""));

                    document.AddCustomProperty(new CustomProperty("br_teh_ispravka", parcela.BrTehIspravka));
                    document.AddCustomProperty(parcela.DatumTehIspravka != null
                       ? new CustomProperty("datum_teh_ispravka", parcela.DatumTehIspravka.Value.ToString("dd.MM.yyyy"))
                       : new CustomProperty("datum_teh_ispravka", ""));

                    //generate picture
                    var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                    var resolution = GetResolutionForScale(1000);
                    var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "sintezen2");

                    Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                    // Create a picture (A custom view of an Image).
                    Picture picture = image.CreatePicture(420, 630);
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph = document.Paragraphs[22];
                    paragraph.InsertPicture(picture);
                    paragraph.Alignment = Alignment.center;

                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "komunalen");
                                    image = document.AddImage(downloadDirectory + myGuid + "komunalen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "soobrakaen");
                                    image = document.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 4:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "spomenici");
                                    image = document.AddImage(downloadDirectory + myGuid + "spomenici");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "parking");
                                    image = document.AddImage(downloadDirectory + myGuid + "parking");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 6:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "infrastrukturen");
                                    image = document.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 7:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "zelenilo");
                                    image = document.AddImage(downloadDirectory + myGuid + "zelenilo");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 8:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Хидротехничкa инфрастуктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "hidrotehnicki");
                                    image = document.AddImage(downloadDirectory + myGuid + "hidrotehnicki");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Хидротехничкa инфрастуктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 9:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Електротехничка инфрастуктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "elektrotehnicki");
                                    image = document.AddImage(downloadDirectory + myGuid + "elektrotehnicki");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Електротехничка инфрастуктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                        }
                    }

                    var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                    if (parcela.TehnickiIspravkiId.HasValue)
                    {
                        var teh_ispravki = new TehnickiIspravkiDa().Get(parcela.TehnickiIspravkiId.Value);
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(teh_ispravki.Path))
                            uri = usloviDirectory + teh_ispravki.Path;
                        else
                            uri = teh_ispravki.Path;

                        //da se vidi kako e so memorijata
                        DocX documentTehIspavki = DocX.Load(uri);
                        document.InsertDocument(documentTehIspavki);
                        document.InsertSectionPageBreak();
                    }
                    if (parcela.OpstiUsloviId.HasValue)
                    {
                        var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                            uri = usloviDirectory + opsti.Path;
                        else
                            uri = opsti.Path;

                        //da se vidi kako e so memorijata
                        DocX documentOpsti = DocX.Load(uri);
                        document.InsertDocument(documentOpsti);
                        document.InsertSectionPageBreak();
                    }
                    if (parcela.PosebniUsloviId.HasValue)
                    {
                        var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                            uri = usloviDirectory + posebni.Path;
                        else
                            uri = posebni.Path;
                        //da se vidi kako e so memorijata
                        DocX documentOpsti = DocX.Load(uri);
                        document.InsertDocument(documentOpsti);
                    }

                    document.Save();
                } // Release this document from memory.

                InsertLogs(user.UserName, opfat.Ime, parcela.Broj, outFileName.ToString());
                return outFileName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }




        public static string GenerateIzvodUlicaCentar(string poligon, int ulica_id)
        {
            try
            {

                var coordinateXYmin = poligon.Split(',')[0];
                double xmin = Double.Parse(coordinateXYmin.Split(' ')[0], new CultureInfo("en-US"));
                double ymin = Double.Parse(coordinateXYmin.Split(' ')[1], new CultureInfo("en-US"));

                var coordinateXYmax = poligon.Split(',')[2];
                double xmax = Double.Parse(coordinateXYmax.Split(' ')[0], new CultureInfo("en-US"));
                double ymax = Double.Parse(coordinateXYmax.Split(' ')[1], new CultureInfo("en-US"));

                var coordinateXY1 = poligon.Split(',')[1];
                double x1 = Double.Parse(coordinateXY1.Split(' ')[0], new CultureInfo("en-US"));
                double y1 = Double.Parse(coordinateXY1.Split(' ')[1], new CultureInfo("en-US"));

                var coordinateXY3 = poligon.Split(',')[3];
                double x3 = Double.Parse(coordinateXY3.Split(' ')[0], new CultureInfo("en-US"));
                double y3 = Double.Parse(coordinateXY3.Split(' ')[1], new CultureInfo("en-US"));


                int width = Convert.ToInt32(Math.Abs(xmax - xmin));
                int height = Convert.ToInt32(Math.Abs(ymax - ymin));

               
                var listOpfat = new OpfatDa().ListaOpfat(poligon);
                if (listOpfat == null || listOpfat.Count <= 0) return "";

                var listSegmentiUlica = new UliciDa().GetSegmentiUlica(poligon, ulica_id);

                var ulicaInfo = new UliciDa().Get(ulica_id);


                List<int> lstSegmentId = new List<int>();

                foreach (var i in listSegmentiUlica)
                {
                    lstSegmentId.Add(i.Id);
                }
                //var segment=listSegmenti[0];

                if (listSegmentiUlica == null || listSegmentiUlica.Count <= 0) return "";

                var listUlici = new UliciDa().GetListUlici(poligon);
                if (listUlici == null || listUlici.Count <= 0) return "";

               
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listOpfat[0].Id);

              
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", opfat.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_ulica.docx";
                string docOutputPath = downloadDirectory + outFileName;

                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);

                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {
                    document.PageHeight = 842;
                    document.PageWidth = 595;
                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));
                    document.AddCustomProperty(new CustomProperty("ime_plan", opfat.Ime));
                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("ulica_ime", ulicaInfo.Ime_ulica));
                    document.AddCustomProperty(new CustomProperty("izgotvil",
                        ConfigurationManager.AppSettings["izgotvil"]));


                    document.InsertSectionPageBreak();

              
                    foreach (var item in listSegmentiUlica)
                    {
                        Paragraph paragraph1 ;
                        var t = document.AddTable(10,5);
                        t.Design = TableDesign.LightShading;
                        t.Alignment = Alignment.center;

                        //Row 1
                        t.Rows[0].Cells[0].Paragraphs[0].Append("Име на улица");
                        t.Rows[1].Cells[0].Paragraphs[0].Append(item.Ime_ulica);

                        t.Rows[0].Cells[1].Paragraphs[0].Append("Тип на улица");
                        t.Rows[1].Cells[1].Paragraphs[0].Append(item.Tip_Ulica);

                        t.Rows[0].Cells[2].Paragraphs[0].Append("Сегмент");
                        t.Rows[1].Cells[2].Paragraphs[0].Append(Convert.ToString(item.SegmentBr));

                        t.Rows[0].Cells[3].Paragraphs[0].Append("Ширина(m)");
                        t.Rows[1].Cells[3].Paragraphs[0].Append((item.Shirina).ToString());


                        t.Rows[0].Cells[4].Paragraphs[0].Append("Тротоари");
                        if (item.Trotoari == true)
                            t.Rows[1].Cells[4].Paragraphs[0].Append("Има");
                        else
                            t.Rows[1].Cells[4].Paragraphs[0].Append("Нема");

                        //ROW2
                        t.Rows[2].Cells[0].Paragraphs[0].Append("Велосипедска патека");
                        if (item.Velosipedska_pateka == true)
                            t.Rows[3].Cells[0].Paragraphs[0].Append("Има");
                        else
                            t.Rows[3].Cells[0].Paragraphs[0].Append("Нема");

                        t.Rows[2].Cells[1].Paragraphs[0].Append("Зеленило");
                        if (item.Zelenilo == true)
                            t.Rows[3].Cells[1].Paragraphs[0].Append("Има");
                        else
                            t.Rows[3].Cells[1].Paragraphs[0].Append("Нема");

                        t.Rows[2].Cells[2].Paragraphs[0].Append("Атмосферска планирана");
                        if (item.Atmosferska_planirana == true)
                            t.Rows[3].Cells[2].Paragraphs[0].Append("Има");
                        else
                            t.Rows[3].Cells[2].Paragraphs[0].Append("Нема");


                        t.Rows[2].Cells[3].Paragraphs[0].Append("Атмосферска постојна");
                        if (item.Atmosferska_postojna == true)
                            t.Rows[3].Cells[3].Paragraphs[0].Append("Има");
                        else
                            t.Rows[3].Cells[3].Paragraphs[0].Append("Нема");

                        t.Rows[2].Cells[4].Paragraphs[0].Append("Водоводна планирана");
                        if (item.Vodovodna_planirana == true)
                            t.Rows[3].Cells[4].Paragraphs[0].Append("Има");
                        else
                            t.Rows[3].Cells[4].Paragraphs[0].Append("Нема");

                        //ROW3
                        t.Rows[4].Cells[0].Paragraphs[0].Append("Водоводна постојна");
                        if (item.Vodovodna_postojna == true)
                            t.Rows[5].Cells[0].Paragraphs[0].Append("Има");
                        else
                            t.Rows[5].Cells[0].Paragraphs[0].Append("Нема");

                        t.Rows[4].Cells[1].Paragraphs[0].Append("Гасоводна постојна");
                        if (item.Gasovodna_postojna == true)
                            t.Rows[5].Cells[1].Paragraphs[0].Append("Има");
                        else
                            t.Rows[5].Cells[1].Paragraphs[0].Append("Нема");

                        t.Rows[4].Cells[2].Paragraphs[0].Append("Гасоводна планирана");
                        if (item.Gasovodna_planirana == true)
                            t.Rows[5].Cells[2].Paragraphs[0].Append("Има");
                        else
                            t.Rows[5].Cells[2].Paragraphs[0].Append("Нема");

                        t.Rows[4].Cells[3].Paragraphs[0].Append("Телекомуникациска постојна");
                        if (item.Telekomunikaciska_postojna == true)
                            t.Rows[5].Cells[3].Paragraphs[0].Append("Има");
                        else
                            t.Rows[5].Cells[3].Paragraphs[0].Append("Нема");

                        t.Rows[4].Cells[4].Paragraphs[0].Append("Телекомуникациска планирана");
                        if (item.Telekomunikaciska_planirana == true)
                            t.Rows[5].Cells[4].Paragraphs[0].Append("Има");
                        else
                            t.Rows[5].Cells[4].Paragraphs[0].Append("Нема");


                        //ROW4
                        t.Rows[6].Cells[0].Paragraphs[0].Append("Електрика постојна");
                        if (item.Elektrika_postojna == true)
                            t.Rows[7].Cells[0].Paragraphs[0].Append("Има");
                        else
                            t.Rows[7].Cells[0].Paragraphs[0].Append("Нема");

                        t.Rows[6].Cells[1].Paragraphs[0].Append("Електрика планирана");
                        if (item.Elektrika_planirana == true)
                            t.Rows[7].Cells[1].Paragraphs[0].Append("Има");
                        else
                            t.Rows[7].Cells[1].Paragraphs[0].Append("Нема");

                        t.Rows[6].Cells[2].Paragraphs[0].Append("Фекална постојна");
                        if (item.Fekalna_postojna == true)
                            t.Rows[7].Cells[2].Paragraphs[0].Append("Има");
                        else
                            t.Rows[7].Cells[2].Paragraphs[0].Append("Нема");

                        t.Rows[6].Cells[3].Paragraphs[0].Append("Фекална планирана");
                        if (item.Fekalna_planirana == true)
                            t.Rows[7].Cells[3].Paragraphs[0].Append("Има");
                        else
                            t.Rows[7].Cells[3].Paragraphs[0].Append("Нема");

                        t.Rows[6].Cells[4].Paragraphs[0].Append("Топлификација постојна");
                        if (item.Toplifikacija_postojna == true)
                            t.Rows[7].Cells[4].Paragraphs[0].Append("Има");
                        else
                            t.Rows[7].Cells[4].Paragraphs[0].Append("Нема");

                        //ROW 5

                        t.Rows[8].Cells[0].Paragraphs[0].Append("Топлификација планирана");
                        if (item.Toplifikacija_planirana == true)
                            t.Rows[9].Cells[0].Paragraphs[0].Append("Има");
                        else
                            t.Rows[9].Cells[0].Paragraphs[0].Append("Нема");

                        t.Rows[8].Cells[1].Paragraphs[0].Append("");
                        t.Rows[9].Cells[1].Paragraphs[0].Append("");

                        t.Rows[8].Cells[2].Paragraphs[0].Append("");
                        t.Rows[9].Cells[2].Paragraphs[0].Append("");

                        t.Rows[8].Cells[3].Paragraphs[0].Append("");
                        t.Rows[9].Cells[3].Paragraphs[0].Append("");

                        t.Rows[8].Cells[4].Paragraphs[0].Append("");
                        t.Rows[9].Cells[4].Paragraphs[0].Append("");


                        document.InsertTable(t);
                       paragraph1 = document.InsertParagraph("", false);

                    }

                    document.InsertSectionPageBreak();


                   // var centroidSegment = new UliciDa().GetCentroidSegment(segment.Id);
                   


                     var resolution = GetResolutionForScaleUlica(1000);
                     int w = Convert.ToInt32(width / resolution);
                     int h = Convert.ToInt32(height / resolution);
                     var bbox2 = BoundsUlica(x1, y1, x3, y3);

                    //DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id, "komunalen");
                     DownloadWmsPlanUlicaSegmenti(bbox2, downloadDirectory + myGuid, w, h, lstSegmentId, "segment");

                     Image image = document.AddImage(downloadDirectory + myGuid + "segment"); ;

                    // Create a picture (A custom view of an Image).
                    Picture picture ;
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph ;
                    //paragraph.InsertPicture(picture);
                    //paragraph.Alignment = Alignment.center;

                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {

                                    DocX doc1 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc1.PageHeight = 842;
                                        doc1.PageWidth = 595;

                                        paragraph = doc1.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc1.InsertParagraph("", false);
                                        DownloadWmsPlanUlicaSegmenti(bbox2, downloadDirectory + myGuid, w, h, lstSegmentId, "segment");
                                        image = doc1.AddImage(downloadDirectory + myGuid + "segment");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc1.InsertSectionPageBreak();
                                        document.InsertDocument(doc1);
                                        doc1.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;

                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc1.PageHeight = h;
                                        doc1.PageWidth = w;
                                        doc1.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        doc1.MarginTop = 20;
                                        paragraph = doc1.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc1.InsertParagraph("", false);
                                        DownloadWmsPlanUlicaSegmenti(bbox2, downloadDirectory + myGuid, w, h, lstSegmentId,
                                         "segment");
                                        image = doc1.AddImage(downloadDirectory + myGuid + "segment");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc1.InsertSectionPageBreak();
                                        document.InsertDocument(doc1);
                                        doc1.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {

                                    DocX doc2 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc2.PageHeight = 842;
                                        doc2.PageWidth = 595;
                                        
                                        paragraph = doc2.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc2.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,"komunalen");
                                        image = doc2.AddImage(downloadDirectory + myGuid + "komunalen");

                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc2.InsertSectionPageBreak();
                                        document.InsertDocument(doc2);
                                        doc2.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;

                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc2.PageHeight = h;
                                        doc2.PageWidth = w;
                                        doc2.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        doc2.MarginTop = 20;
                                        paragraph = doc2.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc2.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                         "komunalen");
                                        image = doc2.AddImage(downloadDirectory + myGuid + "komunalen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc2.InsertSectionPageBreak();
                                        document.InsertDocument(doc2);
                                        doc2.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {
                                    
                                    DocX doc3 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc3.PageHeight = 842;
                                        doc3.PageWidth = 595;
                                        //doc3.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        //doc3.MarginTop = 5;
                                        paragraph = doc3.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc3.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "soobrakaen");
                                        image = doc3.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc3.InsertSectionPageBreak();
                                        document.InsertDocument(doc3);
                                        doc3.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc3.PageHeight = h;
                                        doc3.PageWidth = w;
                                        doc3.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        doc3.MarginTop = 20;
                                        paragraph = doc3.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc3.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "soobrakaen");
                                        image = doc3.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc3.InsertSectionPageBreak();
                                        document.InsertDocument(doc3);
                                        doc3.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 4:
                                try
                                {

                                    DocX doc4 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc4.PageHeight = 842;
                                        doc4.PageWidth = 595;
                                        //doc4.MarginBottom = 5;
                                        //doc4.MarginLeft = 5;
                                        //doc4.MarginRight = 10;
                                        //doc4.MarginTop = 5;
                                        paragraph = doc4.InsertParagraph("", false);
                                        paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc4.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "spomenici");
                                        image = doc4.AddImage(downloadDirectory + myGuid + "spomenici");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc4.InsertSectionPageBreak();
                                        document.InsertDocument(doc4);
                                        doc4.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc4.PageHeight = h;
                                        doc4.PageWidth = w;
                                        doc4.MarginBottom = 5;
                                        //doc4.MarginLeft = 5;
                                        //doc4.MarginRight = 10;
                                        doc4.MarginTop = 20;
                                        paragraph = doc4.InsertParagraph("", false);
                                        paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc4.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "spomenici");
                                        image = doc4.AddImage(downloadDirectory + myGuid + "spomenici");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc4.InsertSectionPageBreak();
                                        document.InsertDocument(doc4);
                                        doc4.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {

                                    DocX doc5 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc5.PageHeight = 842;
                                        doc5.PageWidth = 595;
                                        //doc5.MarginBottom = 5;
                                        //doc5.MarginLeft = 5;
                                        //doc5.MarginRight = 10;
                                        //doc5.MarginTop = 5;
                                        paragraph = doc5.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc5.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "parking");
                                        image = doc5.AddImage(downloadDirectory + myGuid + "parking");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc5.InsertSectionPageBreak();
                                        document.InsertDocument(doc5);
                                        doc5.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc5.PageHeight = h;
                                        doc5.PageWidth = w;
                                        doc5.MarginBottom = 5;
                                        //doc5.MarginLeft = 5;
                                        //doc5.MarginRight = 10;
                                        doc5.MarginTop = 20;
                                        paragraph = doc5.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc5.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "parking");
                                        image = doc5.AddImage(downloadDirectory + myGuid + "parking");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc5.InsertSectionPageBreak();
                                        document.InsertDocument(doc5);
                                        doc5.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 6:
                                try
                                {

                                    DocX doc6 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc6.PageHeight = 842;
                                        doc6.PageWidth = 595;
                                        //doc6.MarginBottom = 5;
                                        //doc6.MarginLeft = 5;
                                        //doc6.MarginRight = 10;
                                        //doc6.MarginTop = 5;
                                        paragraph = doc6.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc6.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "infrastrukturen");
                                        image = doc6.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc6.InsertSectionPageBreak();
                                        document.InsertDocument(doc6);
                                        doc6.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc6.PageHeight = h;
                                        doc6.PageWidth = w;
                                        doc6.MarginBottom = 5;
                                        //doc6.MarginLeft = 5;
                                        //doc6.MarginRight = 10;
                                        doc6.MarginTop = 20;
                                        paragraph = doc6.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc6.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "infrastrukturen");
                                        image = doc6.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc6.InsertSectionPageBreak();
                                        document.InsertDocument(doc6);
                                        doc6.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 7:
                                try
                                {

                                    DocX doc7 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc7.PageHeight = 842;
                                        doc7.PageWidth = 595;
                                        //doc7.MarginBottom = 5;
                                        //doc7.MarginLeft = 5;
                                        //doc7.MarginRight = 10;
                                        //doc7.MarginTop = 5;
                                        paragraph = doc7.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc7.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "zelenilo");
                                        image = doc7.AddImage(downloadDirectory + myGuid + "zelenilo");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc7.InsertSectionPageBreak();
                                        document.InsertDocument(doc7);
                                        doc7.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc7.PageHeight = h;
                                        doc7.PageWidth = w;
                                        doc7.MarginBottom = 5;
                                        //doc7.MarginLeft = 5;
                                        //doc7.MarginRight = 10;
                                        doc7.MarginTop = 20;
                                        paragraph = doc7.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc7.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "zelenilo");
                                        image = doc7.AddImage(downloadDirectory + myGuid + "zelenilo");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc7.InsertSectionPageBreak();
                                        document.InsertDocument(doc7);
                                        doc7.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 8:
                                try
                                {

                                    DocX doc8 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc8.PageHeight = 842;
                                        doc8.PageWidth = 595;
                                        //doc8.MarginBottom = 5;
                                        //doc8.MarginLeft = 5;
                                        //doc8.MarginRight = 10;
                                        //doc8.MarginTop = 5;
                                        paragraph = doc8.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc8.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "hidrotehnicki");
                                        image = doc8.AddImage(downloadDirectory + myGuid + "hidrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc8.InsertSectionPageBreak();
                                        document.InsertDocument(doc8);
                                        doc8.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc8.PageHeight = h;
                                        doc8.PageWidth = w;
                                        doc8.MarginBottom = 5;
                                        //doc8.MarginLeft = 5;
                                        //doc8.MarginRight = 10;
                                        doc8.MarginTop = 20;
                                        paragraph = doc8.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc8.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "hidrotehnicki");
                                        image = doc8.AddImage(downloadDirectory + myGuid + "hidrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc8.InsertSectionPageBreak();
                                        document.InsertDocument(doc8);
                                        doc8.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 9:
                                try
                                {

                                    DocX doc9 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc9.PageHeight = 842;
                                        doc9.PageWidth = 595;
                                        //doc9.MarginBottom = 5;
                                        //doc9.MarginLeft = 5;
                                        //doc9.MarginRight = 10;
                                        //doc9.MarginTop = 5;
                                        paragraph = doc9.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc9.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "elektrotehnicki");
                                        image = doc9.AddImage(downloadDirectory + myGuid + "elektrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc9.InsertSectionPageBreak();
                                        document.InsertDocument(doc9);
                                        doc9.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc9.PageHeight = h;
                                        doc9.PageWidth = w;
                                        doc9.MarginBottom = 5;
                                        //doc9.MarginLeft = 5;
                                        //doc9.MarginRight = 10;
                                        doc9.MarginTop = 20;
                                        paragraph = doc9.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc9.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "elektrotehnicki");
                                        image = doc9.AddImage(downloadDirectory + myGuid + "elektrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc9.InsertSectionPageBreak();
                                        document.InsertDocument(doc9);
                                        doc9.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;


                           
                        }
                    }

                    //var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                    //if (parcela.OpstiUsloviId.HasValue)
                    //{
                    //    var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                    //    string uri;
                    //    if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                    //        uri = usloviDirectory + opsti.Path;
                    //    else
                    //        uri = opsti.Path;

                    //    //da se vidi kako e so memorijata
                    //    DocX documentOpsti = DocX.Load(uri);
                    //    document.InsertDocument(documentOpsti);
                    //    document.InsertSectionPageBreak();
                    //}
                    //if (parcela.PosebniUsloviId.HasValue)
                    //{
                    //    var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                    //    string uri;
                    //    if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                    //        uri = usloviDirectory + posebni.Path;
                    //    else
                    //        uri = posebni.Path;
                    //    //da se vidi kako e so memorijata
                    //    DocX documentOpsti = DocX.Load(uri);
                    //    document.InsertDocument(documentOpsti);
                    //}

                    document.Save();
                    
                } // Release this document from memory.

                InsertLogsUlici(user.UserName, opfat.Ime, outFileName.ToString());
                return outFileName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }


        public static string GenerateIzvodUlicaAerodrom(string poligon, int ulica_id)
        {
            try
            {

                var coordinateXYmin = poligon.Split(',')[0];
                double xmin = Double.Parse(coordinateXYmin.Split(' ')[0], new CultureInfo("en-US"));
                double ymin = Double.Parse(coordinateXYmin.Split(' ')[1], new CultureInfo("en-US"));

                var coordinateXYmax = poligon.Split(',')[2];
                double xmax = Double.Parse(coordinateXYmax.Split(' ')[0], new CultureInfo("en-US"));
                double ymax = Double.Parse(coordinateXYmax.Split(' ')[1], new CultureInfo("en-US"));

                var coordinateXY1 = poligon.Split(',')[1];
                double x1 = Double.Parse(coordinateXY1.Split(' ')[0], new CultureInfo("en-US"));
                double y1 = Double.Parse(coordinateXY1.Split(' ')[1], new CultureInfo("en-US"));

                var coordinateXY3 = poligon.Split(',')[3];
                double x3 = Double.Parse(coordinateXY3.Split(' ')[0], new CultureInfo("en-US"));
                double y3 = Double.Parse(coordinateXY3.Split(' ')[1], new CultureInfo("en-US"));


                int width = Convert.ToInt32(Math.Abs(xmax - xmin));
                int height = Convert.ToInt32(Math.Abs(ymax - ymin));


                //var listOpfat = new OpfatDa().ListaOpfatAerodrom(poligon);
                var listOpfat = new OpfatDa().ListaOpfatUlicaAerodrom(poligon,ulica_id);
                var opfati = listOpfat.Aggregate("", (current, item) => current + (item.Ime + "  ;  "));
                
                if (listOpfat == null || listOpfat.Count <= 0) return "";
                

                var listSegmentiUlica = new UliciDa().GetSegmentiUlica(poligon, ulica_id);

                var ulicaInfo = new UliciDa().Get(ulica_id);

                List<int> lstOpfatId = new List<int>();

                foreach (var i in listOpfat)
                {
                    lstOpfatId.Add(i.Id);
                }


                List<int> lstSegmentId = new List<int>();

                foreach (var i in listSegmentiUlica)
                {
                    lstSegmentId.Add(i.Id);
                }
                

                if (listSegmentiUlica == null || listSegmentiUlica.Count <= 0) return "";

                var listUlici = new UliciDa().GetListUlici(poligon);
                if (listUlici == null || listUlici.Count <= 0) return "";


                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listOpfat[0].Id);


          
                //proveri sto piovi na planovi ima
                //var legendi = new LegendDa().Get(opfat.Id);


                var legendi = new LegendDa().GetLegends(lstOpfatId);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", opfat.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_ulica_aerodrom.docx";
                string docOutputPath = downloadDirectory + outFileName;

                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);

                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {
                    //document.PageHeight = 842;
                    //document.PageWidth = 595;
                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));

                    
                        if (opfat.TipPlan == 5)
                        {
                            document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", opfati));
                            document.AddCustomProperty(new CustomProperty("dup_ime_plan", ""));
                        }
                        else
                        {
                            document.AddCustomProperty(new CustomProperty("dup_ime_plan", opfati));
                            document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", ""));
                        }

                    //document.AddCustomProperty(new CustomProperty("ime_plan", opfat.Ime));
                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("ulica_ime", ulicaInfo.Ime_ulica));
                    document.AddCustomProperty(new CustomProperty("izgotvil",
                        ConfigurationManager.AppSettings["izgotvil"]));


                    document.InsertSectionPageBreak();

                    //TABELA
                    //foreach (var item in listSegmentiUlica)
                    //{

                        
                    //    Paragraph paragraph1;
                    //    var t = document.AddTable(10, 5);
                    //    t.Design = TableDesign.LightShading;
                    //    t.Alignment = Alignment.center;

                    //    //Row 1
                    //    t.Rows[0].Cells[0].Paragraphs[0].Append("Име на улица");
                    //    t.Rows[1].Cells[0].Paragraphs[0].Append(item.Ime_ulica);

                    //    t.Rows[0].Cells[1].Paragraphs[0].Append("Тип на улица");
                    //    t.Rows[1].Cells[1].Paragraphs[0].Append(item.Tip_Ulica);

                    //    t.Rows[0].Cells[2].Paragraphs[0].Append("Сегмент");
                    //    t.Rows[1].Cells[2].Paragraphs[0].Append(Convert.ToString(item.SegmentBr));

                    //    t.Rows[0].Cells[3].Paragraphs[0].Append("Ширина(m)");
                    //    t.Rows[1].Cells[3].Paragraphs[0].Append((item.Shirina).ToString());


                    //    t.Rows[0].Cells[4].Paragraphs[0].Append("Тротоари");
                    //    if (item.Trotoari == true)
                    //        t.Rows[1].Cells[4].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[1].Cells[4].Paragraphs[0].Append("Нема");

                    //    //ROW2
                    //    t.Rows[2].Cells[0].Paragraphs[0].Append("Велосипедска патека");
                    //    if (item.Velosipedska_pateka == true)
                    //        t.Rows[3].Cells[0].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[0].Paragraphs[0].Append("Нема");

                    //    t.Rows[2].Cells[1].Paragraphs[0].Append("Зеленило");
                    //    if (item.Zelenilo == true)
                    //        t.Rows[3].Cells[1].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[1].Paragraphs[0].Append("Нема");

                    //    t.Rows[2].Cells[2].Paragraphs[0].Append("Паркинг");
                    //    if (item.Parking == true)
                    //        t.Rows[3].Cells[2].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[2].Paragraphs[0].Append("Нема");


                    //    t.Rows[2].Cells[3].Paragraphs[0].Append("Пешачка патека");
                    //    if (item.Pesacka_pateka == true)
                    //        t.Rows[3].Cells[3].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[3].Paragraphs[0].Append("Нема");

                    //    t.Rows[2].Cells[4].Paragraphs[0].Append("Водоводна планирана");
                    //    if (item.Vodovodna_planirana == true)
                    //        t.Rows[3].Cells[4].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[4].Paragraphs[0].Append("Нема");

                    //    //ROW3
                    //    t.Rows[4].Cells[0].Paragraphs[0].Append("Водоводна постојна");
                    //    if (item.Vodovodna_postojna == true)
                    //        t.Rows[5].Cells[0].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[0].Paragraphs[0].Append("Нема");

                    //    t.Rows[4].Cells[1].Paragraphs[0].Append("Гасоводна постојна");
                    //    if (item.Gasovodna_postojna == true)
                    //        t.Rows[5].Cells[1].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[1].Paragraphs[0].Append("Нема");

                    //    t.Rows[4].Cells[2].Paragraphs[0].Append("Гасоводна планирана");
                    //    if (item.Gasovodna_planirana == true)
                    //        t.Rows[5].Cells[2].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[2].Paragraphs[0].Append("Нема");

                    //    t.Rows[4].Cells[3].Paragraphs[0].Append("Телекомуникациска постојна");
                    //    if (item.Telekomunikaciska_postojna == true)
                    //        t.Rows[5].Cells[3].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[3].Paragraphs[0].Append("Нема");

                    //    t.Rows[4].Cells[4].Paragraphs[0].Append("Телекомуникациска планирана");
                    //    if (item.Telekomunikaciska_planirana == true)
                    //        t.Rows[5].Cells[4].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[4].Paragraphs[0].Append("Нема");


                    //    //ROW4
                    //    t.Rows[6].Cells[0].Paragraphs[0].Append("Електрика постојна");
                    //    if (item.Elektrika_postojna == true)
                    //        t.Rows[7].Cells[0].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[0].Paragraphs[0].Append("Нема");

                    //    t.Rows[6].Cells[1].Paragraphs[0].Append("Електрика планирана");
                    //    if (item.Elektrika_planirana == true)
                    //        t.Rows[7].Cells[1].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[1].Paragraphs[0].Append("Нема");

                    //    t.Rows[6].Cells[2].Paragraphs[0].Append("Фекална постојна");
                    //    if (item.Fekalna_postojna == true)
                    //        t.Rows[7].Cells[2].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[2].Paragraphs[0].Append("Нема");

                    //    t.Rows[6].Cells[3].Paragraphs[0].Append("Фекална планирана");
                    //    if (item.Fekalna_planirana == true)
                    //        t.Rows[7].Cells[3].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[3].Paragraphs[0].Append("Нема");

                    //    t.Rows[6].Cells[4].Paragraphs[0].Append("Топлификација постојна");
                    //    if (item.Toplifikacija_postojna == true)
                    //        t.Rows[7].Cells[4].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[4].Paragraphs[0].Append("Нема");

                    //    //ROW 5

                    //    t.Rows[8].Cells[0].Paragraphs[0].Append("Топлификација планирана");
                    //    if (item.Toplifikacija_planirana == true)
                    //        t.Rows[9].Cells[0].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[9].Cells[0].Paragraphs[0].Append("Нема");

                    //    t.Rows[8].Cells[1].Paragraphs[0].Append("Атмосферска планирана");
                    //    if (item.Atmosferska_planirana == true)
                    //        t.Rows[9].Cells[1].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[9].Cells[1].Paragraphs[0].Append("Нема");

                    //    t.Rows[8].Cells[2].Paragraphs[0].Append("Атмосферска постојна");
                    //    if (item.Atmosferska_postojna == true)
                    //        t.Rows[9].Cells[2].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[9].Cells[2].Paragraphs[0].Append("Нема");

                    //    t.Rows[8].Cells[3].Paragraphs[0].Append("");
                    //    t.Rows[9].Cells[3].Paragraphs[0].Append("");

                    //    t.Rows[8].Cells[4].Paragraphs[0].Append("");
                    //    t.Rows[9].Cells[4].Paragraphs[0].Append("");

                     


                    //    document.InsertTable(t);
                    //    paragraph1 = document.InsertParagraph("", false);

                    //}

                    //document.InsertSectionPageBreak();


                     //var centroidSegment = new UliciDa().GetCentroidSegment(segment.Id);



                    var resolution = GetResolutionForScaleUlica(1000);
                    int w = Convert.ToInt32(width / resolution);
                    int h = Convert.ToInt32(height / resolution);
                    var bbox2 = BoundsUlica(x1, y1, x3, y3);

                    //DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id, "komunalen");
                    DownloadWmsPlanUlicaSegmenti(bbox2, downloadDirectory + myGuid, w, h, lstSegmentId, "segment");

                    Image image = document.AddImage(downloadDirectory + myGuid + "segment"); ;

                    // Create a picture (A custom view of an Image).
                    Picture picture;
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph;
                    //paragraph.InsertPicture(picture);
                    //paragraph.Alignment = Alignment.center;

                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {

                                    DocX doc1 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc1.PageHeight = 842;
                                        doc1.PageWidth = 595;


                                        paragraph = doc1.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc1.InsertParagraph("", false);
                                        DownloadWmsPlanUlicaSegmenti(bbox2, downloadDirectory + myGuid, w, h, lstSegmentId, "segment");
                                        image = doc1.AddImage(downloadDirectory + myGuid + "segment");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc1.InsertSectionPageBreak();
                                        document.InsertDocument(doc1);
                                        doc1.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;

                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc1.PageHeight = h + 300;
                                        doc1.PageWidth = w;
                                        //doc1.MarginBottom = 500;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        //doc1.MarginTop = 20;
                                        paragraph = doc1.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc1.InsertParagraph("", false);
                                        DownloadWmsPlanUlicaSegmenti(bbox2, downloadDirectory + myGuid, w, h, lstSegmentId,
                                         "segment");
                                        image = doc1.AddImage(downloadDirectory + myGuid + "segment");
                                       //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc1.InsertSectionPageBreak();
                                        document.InsertDocument(doc1);
                                        doc1.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {

                                    DocX doc2 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc2.PageHeight = 842;
                                        doc2.PageWidth = 595;

                                        paragraph = doc2.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc2.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id, "komunalen");
                                        image = doc2.AddImage(downloadDirectory + myGuid + "komunalen");

                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc2.InsertSectionPageBreak();
                                        document.InsertDocument(doc2);
                                        doc2.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;

                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc2.PageHeight = h + 300;
                                        doc2.PageWidth = w;
                                        doc2.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        doc2.MarginTop = 20;
                                        paragraph = doc2.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc2.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                         "komunalen");
                                        image = doc2.AddImage(downloadDirectory + myGuid + "komunalen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc2.InsertSectionPageBreak();
                                        document.InsertDocument(doc2);
                                        doc2.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {

                                    DocX doc3 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc3.PageHeight = 842;
                                        doc3.PageWidth = 595;
                                        //doc3.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        //doc3.MarginTop = 5;
                                        paragraph = doc3.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc3.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "soobrakaen");
                                        image = doc3.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc3.InsertSectionPageBreak();
                                        document.InsertDocument(doc3);
                                        doc3.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc3.PageHeight = h + 300;
                                        doc3.PageWidth = w;
                                        doc3.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        doc3.MarginTop = 20;
                                        paragraph = doc3.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc3.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "soobrakaen");
                                        image = doc3.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc3.InsertSectionPageBreak();
                                        document.InsertDocument(doc3);
                                        doc3.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 4:
                                try
                                {

                                    DocX doc4 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc4.PageHeight = 842;
                                        doc4.PageWidth = 595;
                                        //doc4.MarginBottom = 5;
                                        //doc4.MarginLeft = 5;
                                        //doc4.MarginRight = 10;
                                        //doc4.MarginTop = 5;
                                        paragraph = doc4.InsertParagraph("", false);
                                        paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc4.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "spomenici");
                                        image = doc4.AddImage(downloadDirectory + myGuid + "spomenici");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc4.InsertSectionPageBreak();
                                        document.InsertDocument(doc4);
                                        doc4.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc4.PageHeight = h + 300;
                                        doc4.PageWidth = w;
                                        doc4.MarginBottom = 5;
                                        //doc4.MarginLeft = 5;
                                        //doc4.MarginRight = 10;
                                        doc4.MarginTop = 20;
                                        paragraph = doc4.InsertParagraph("", false);
                                        paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc4.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "spomenici");
                                        image = doc4.AddImage(downloadDirectory + myGuid + "spomenici");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc4.InsertSectionPageBreak();
                                        document.InsertDocument(doc4);
                                        doc4.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {

                                    DocX doc5 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc5.PageHeight = 842;
                                        doc5.PageWidth = 595;
                                        //doc5.MarginBottom = 5;
                                        //doc5.MarginLeft = 5;
                                        //doc5.MarginRight = 10;
                                        //doc5.MarginTop = 5;
                                        paragraph = doc5.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc5.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "parking");
                                        image = doc5.AddImage(downloadDirectory + myGuid + "parking");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc5.InsertSectionPageBreak();
                                        document.InsertDocument(doc5);
                                        doc5.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc5.PageHeight = h + 300;
                                        doc5.PageWidth = w;
                                        doc5.MarginBottom = 5;
                                        //doc5.MarginLeft = 5;
                                        //doc5.MarginRight = 10;
                                        doc5.MarginTop = 20;
                                        paragraph = doc5.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc5.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "parking");
                                        image = doc5.AddImage(downloadDirectory + myGuid + "parking");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc5.InsertSectionPageBreak();
                                        document.InsertDocument(doc5);
                                        doc5.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 6:
                                try
                                {

                                    DocX doc6 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc6.PageHeight = 842;
                                        doc6.PageWidth = 595;
                                        //doc6.MarginBottom = 5;
                                        //doc6.MarginLeft = 5;
                                        //doc6.MarginRight = 10;
                                        //doc6.MarginTop = 5;
                                        paragraph = doc6.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc6.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "infrastrukturen");
                                        image = doc6.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc6.InsertSectionPageBreak();
                                        document.InsertDocument(doc6);
                                        doc6.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc6.PageHeight = h + 300;
                                        doc6.PageWidth = w;
                                        doc6.MarginBottom = 5;
                                        //doc6.MarginLeft = 5;
                                        //doc6.MarginRight = 10;
                                        doc6.MarginTop = 20;
                                        paragraph = doc6.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc6.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "infrastrukturen");
                                        image = doc6.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc6.InsertSectionPageBreak();
                                        document.InsertDocument(doc6);
                                        doc6.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 7:
                                try
                                {

                                    DocX doc7 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc7.PageHeight = 842;
                                        doc7.PageWidth = 595;
                                        //doc7.MarginBottom = 5;
                                        //doc7.MarginLeft = 5;
                                        //doc7.MarginRight = 10;
                                        //doc7.MarginTop = 5;
                                        paragraph = doc7.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc7.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "zelenilo");
                                        image = doc7.AddImage(downloadDirectory + myGuid + "zelenilo");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc7.InsertSectionPageBreak();
                                        document.InsertDocument(doc7);
                                        doc7.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc7.PageHeight = h + 300;
                                        doc7.PageWidth = w;
                                        doc7.MarginBottom = 5;
                                        //doc7.MarginLeft = 5;
                                        //doc7.MarginRight = 10;
                                        doc7.MarginTop = 20;
                                        paragraph = doc7.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc7.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "zelenilo");
                                        image = doc7.AddImage(downloadDirectory + myGuid + "zelenilo");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc7.InsertSectionPageBreak();
                                        document.InsertDocument(doc7);
                                        doc7.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 8:
                                try
                                {

                                    DocX doc8 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc8.PageHeight = 842;
                                        doc8.PageWidth = 595;
                                        //doc8.MarginBottom = 5;
                                        //doc8.MarginLeft = 5;
                                        //doc8.MarginRight = 10;
                                        //doc8.MarginTop = 5;
                                        paragraph = doc8.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc8.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "hidrotehnicki");
                                        image = doc8.AddImage(downloadDirectory + myGuid + "hidrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc8.InsertSectionPageBreak();
                                        document.InsertDocument(doc8);
                                        doc8.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc8.PageHeight = h + 300;
                                        doc8.PageWidth = w;
                                        doc8.MarginBottom = 5;
                                        //doc8.MarginLeft = 5;
                                        //doc8.MarginRight = 10;
                                        doc8.MarginTop = 20;
                                        paragraph = doc8.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc8.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "hidrotehnicki");
                                        image = doc8.AddImage(downloadDirectory + myGuid + "hidrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc8.InsertSectionPageBreak();
                                        document.InsertDocument(doc8);
                                        doc8.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 9:
                                try
                                {

                                    DocX doc9 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc9.PageHeight = 842;
                                        doc9.PageWidth = 595;
                                        //doc9.MarginBottom = 5;
                                        //doc9.MarginLeft = 5;
                                        //doc9.MarginRight = 10;
                                        //doc9.MarginTop = 5;
                                        paragraph = doc9.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc9.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "elektrotehnicki");
                                        image = doc9.AddImage(downloadDirectory + myGuid + "elektrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc9.InsertSectionPageBreak();
                                        document.InsertDocument(doc9);
                                        doc9.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc9.PageHeight = h + 300;
                                        doc9.PageWidth = w;
                                        doc9.MarginBottom = 5;
                                        //doc9.MarginLeft = 5;
                                        //doc9.MarginRight = 10;
                                        doc9.MarginTop = 20;
                                        paragraph = doc9.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc9.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "elektrotehnicki");
                                        image = doc9.AddImage(downloadDirectory + myGuid + "elektrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc9.InsertSectionPageBreak();
                                        document.InsertDocument(doc9);
                                        doc9.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;



                        }
                    }

                    //var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                    //if (parcela.OpstiUsloviId.HasValue)
                    //{
                    //    var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                    //    string uri;
                    //    if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                    //        uri = usloviDirectory + opsti.Path;
                    //    else
                    //        uri = opsti.Path;

                    //    //da se vidi kako e so memorijata
                    //    DocX documentOpsti = DocX.Load(uri);
                    //    document.InsertDocument(documentOpsti);
                    //    document.InsertSectionPageBreak();
                    //}
                    //if (parcela.PosebniUsloviId.HasValue)
                    //{
                    //    var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                    //    string uri;
                    //    if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                    //        uri = usloviDirectory + posebni.Path;
                    //    else
                    //        uri = posebni.Path;
                    //    //da se vidi kako e so memorijata
                    //    DocX documentOpsti = DocX.Load(uri);
                    //    document.InsertDocument(documentOpsti);
                    //}

                    document.Save();

                } // Release this document from memory.

                InsertLogsUlici(user.UserName, opfat.Ime, outFileName.ToString());
                return outFileName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }

        public static string GenerateIzvodUliciAerodrom(string poligon)
        {
            try
            {

                var coordinateXYmin = poligon.Split(',')[0];
                double xmin = Double.Parse(coordinateXYmin.Split(' ')[0], new CultureInfo("en-US"));
                double ymin = Double.Parse(coordinateXYmin.Split(' ')[1], new CultureInfo("en-US"));

                var coordinateXYmax = poligon.Split(',')[2];
                double xmax = Double.Parse(coordinateXYmax.Split(' ')[0], new CultureInfo("en-US"));
                double ymax = Double.Parse(coordinateXYmax.Split(' ')[1], new CultureInfo("en-US"));

                var coordinateXY1 = poligon.Split(',')[1];
                double x1 = Double.Parse(coordinateXY1.Split(' ')[0], new CultureInfo("en-US"));
                double y1 = Double.Parse(coordinateXY1.Split(' ')[1], new CultureInfo("en-US"));

                var coordinateXY3 = poligon.Split(',')[3];
                double x3 = Double.Parse(coordinateXY3.Split(' ')[0], new CultureInfo("en-US"));
                double y3 = Double.Parse(coordinateXY3.Split(' ')[1], new CultureInfo("en-US"));


                int width = Convert.ToInt32(Math.Abs(xmax - xmin));
                int height = Convert.ToInt32(Math.Abs(ymax - ymin));


                var listOpfat = new OpfatDa().ListaOpfatAerodrom(poligon);
                //var listOpfat = new OpfatDa().ListaOpfatUlicaAerodrom(poligon, ulica_id);

                var opfati = listOpfat.Aggregate("", (current, item) => current + (item.Ime + "  ;  "));


                if (listOpfat == null || listOpfat.Count <= 0) return "";


                //var listSegmentiUlica = new UliciDa().GetSegmentiUlica(poligon, ulica_id);

                //var ulicaInfo = new UliciDa().Get(ulica_id);

                List<int> lstOpfatId = new List<int>();

                foreach (var i in listOpfat)
                {
                    lstOpfatId.Add(i.Id);
                }


                //List<int> lstSegmentId = new List<int>();

                //foreach (var i in listSegmentiUlica)
                //{
                //    lstSegmentId.Add(i.Id);
                //}


                //if (listSegmentiUlica == null || listSegmentiUlica.Count <= 0) return "";

                var listUlici = new UliciDa().GetListUlici(poligon);
                if (listUlici == null || listUlici.Count <= 0) return "";


                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listOpfat[0].Id);



                //proveri sto piovi na planovi ima
                //var legendi = new LegendDa().Get(opfat.Id);


                var legendi = new LegendDa().GetLegends(lstOpfatId);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", opfat.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_ulici_aerodrom.docx";
                string docOutputPath = downloadDirectory + outFileName;

                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);

                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {
                    //document.PageHeight = 842;
                    //document.PageWidth = 595;
                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));


                    if (opfat.TipPlan == 5)
                    {
                        document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", opfati));
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", ""));
                    }
                    else
                    {
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", opfati));
                        document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", ""));
                    }

                    //document.AddCustomProperty(new CustomProperty("ime_plan", opfat.Ime));
                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    //document.AddCustomProperty(new CustomProperty("ulica_ime", ulicaInfo.Ime_ulica));
                    document.AddCustomProperty(new CustomProperty("izgotvil",
                        ConfigurationManager.AppSettings["izgotvil"]));


                    document.InsertSectionPageBreak();

                    //TABELA
                    //foreach (var item in listSegmentiUlica)
                    //{


                    //    Paragraph paragraph1;
                    //    var t = document.AddTable(10, 5);
                    //    t.Design = TableDesign.LightShading;
                    //    t.Alignment = Alignment.center;

                    //    //Row 1
                    //    t.Rows[0].Cells[0].Paragraphs[0].Append("Име на улица");
                    //    t.Rows[1].Cells[0].Paragraphs[0].Append(item.Ime_ulica);

                    //    t.Rows[0].Cells[1].Paragraphs[0].Append("Тип на улица");
                    //    t.Rows[1].Cells[1].Paragraphs[0].Append(item.Tip_Ulica);

                    //    t.Rows[0].Cells[2].Paragraphs[0].Append("Сегмент");
                    //    t.Rows[1].Cells[2].Paragraphs[0].Append(Convert.ToString(item.SegmentBr));

                    //    t.Rows[0].Cells[3].Paragraphs[0].Append("Ширина(m)");
                    //    t.Rows[1].Cells[3].Paragraphs[0].Append((item.Shirina).ToString());


                    //    t.Rows[0].Cells[4].Paragraphs[0].Append("Тротоари");
                    //    if (item.Trotoari == true)
                    //        t.Rows[1].Cells[4].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[1].Cells[4].Paragraphs[0].Append("Нема");

                    //    //ROW2
                    //    t.Rows[2].Cells[0].Paragraphs[0].Append("Велосипедска патека");
                    //    if (item.Velosipedska_pateka == true)
                    //        t.Rows[3].Cells[0].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[0].Paragraphs[0].Append("Нема");

                    //    t.Rows[2].Cells[1].Paragraphs[0].Append("Зеленило");
                    //    if (item.Zelenilo == true)
                    //        t.Rows[3].Cells[1].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[1].Paragraphs[0].Append("Нема");

                    //    t.Rows[2].Cells[2].Paragraphs[0].Append("Паркинг");
                    //    if (item.Parking == true)
                    //        t.Rows[3].Cells[2].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[2].Paragraphs[0].Append("Нема");


                    //    t.Rows[2].Cells[3].Paragraphs[0].Append("Пешачка патека");
                    //    if (item.Pesacka_pateka == true)
                    //        t.Rows[3].Cells[3].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[3].Paragraphs[0].Append("Нема");

                    //    t.Rows[2].Cells[4].Paragraphs[0].Append("Водоводна планирана");
                    //    if (item.Vodovodna_planirana == true)
                    //        t.Rows[3].Cells[4].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[3].Cells[4].Paragraphs[0].Append("Нема");

                    //    //ROW3
                    //    t.Rows[4].Cells[0].Paragraphs[0].Append("Водоводна постојна");
                    //    if (item.Vodovodna_postojna == true)
                    //        t.Rows[5].Cells[0].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[0].Paragraphs[0].Append("Нема");

                    //    t.Rows[4].Cells[1].Paragraphs[0].Append("Гасоводна постојна");
                    //    if (item.Gasovodna_postojna == true)
                    //        t.Rows[5].Cells[1].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[1].Paragraphs[0].Append("Нема");

                    //    t.Rows[4].Cells[2].Paragraphs[0].Append("Гасоводна планирана");
                    //    if (item.Gasovodna_planirana == true)
                    //        t.Rows[5].Cells[2].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[2].Paragraphs[0].Append("Нема");

                    //    t.Rows[4].Cells[3].Paragraphs[0].Append("Телекомуникациска постојна");
                    //    if (item.Telekomunikaciska_postojna == true)
                    //        t.Rows[5].Cells[3].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[3].Paragraphs[0].Append("Нема");

                    //    t.Rows[4].Cells[4].Paragraphs[0].Append("Телекомуникациска планирана");
                    //    if (item.Telekomunikaciska_planirana == true)
                    //        t.Rows[5].Cells[4].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[5].Cells[4].Paragraphs[0].Append("Нема");


                    //    //ROW4
                    //    t.Rows[6].Cells[0].Paragraphs[0].Append("Електрика постојна");
                    //    if (item.Elektrika_postojna == true)
                    //        t.Rows[7].Cells[0].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[0].Paragraphs[0].Append("Нема");

                    //    t.Rows[6].Cells[1].Paragraphs[0].Append("Електрика планирана");
                    //    if (item.Elektrika_planirana == true)
                    //        t.Rows[7].Cells[1].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[1].Paragraphs[0].Append("Нема");

                    //    t.Rows[6].Cells[2].Paragraphs[0].Append("Фекална постојна");
                    //    if (item.Fekalna_postojna == true)
                    //        t.Rows[7].Cells[2].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[2].Paragraphs[0].Append("Нема");

                    //    t.Rows[6].Cells[3].Paragraphs[0].Append("Фекална планирана");
                    //    if (item.Fekalna_planirana == true)
                    //        t.Rows[7].Cells[3].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[3].Paragraphs[0].Append("Нема");

                    //    t.Rows[6].Cells[4].Paragraphs[0].Append("Топлификација постојна");
                    //    if (item.Toplifikacija_postojna == true)
                    //        t.Rows[7].Cells[4].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[7].Cells[4].Paragraphs[0].Append("Нема");

                    //    //ROW 5

                    //    t.Rows[8].Cells[0].Paragraphs[0].Append("Топлификација планирана");
                    //    if (item.Toplifikacija_planirana == true)
                    //        t.Rows[9].Cells[0].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[9].Cells[0].Paragraphs[0].Append("Нема");

                    //    t.Rows[8].Cells[1].Paragraphs[0].Append("Атмосферска планирана");
                    //    if (item.Atmosferska_planirana == true)
                    //        t.Rows[9].Cells[1].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[9].Cells[1].Paragraphs[0].Append("Нема");

                    //    t.Rows[8].Cells[2].Paragraphs[0].Append("Атмосферска постојна");
                    //    if (item.Atmosferska_postojna == true)
                    //        t.Rows[9].Cells[2].Paragraphs[0].Append("Има");
                    //    else
                    //        t.Rows[9].Cells[2].Paragraphs[0].Append("Нема");

                    //    t.Rows[8].Cells[3].Paragraphs[0].Append("");
                    //    t.Rows[9].Cells[3].Paragraphs[0].Append("");

                    //    t.Rows[8].Cells[4].Paragraphs[0].Append("");
                    //    t.Rows[9].Cells[4].Paragraphs[0].Append("");




                    //    document.InsertTable(t);
                    //    paragraph1 = document.InsertParagraph("", false);

                    //}

                    //document.InsertSectionPageBreak();


                    //var centroidSegment = new UliciDa().GetCentroidSegment(segment.Id);



                    var resolution = GetResolutionForScaleUlica(1000);
                    int w = Convert.ToInt32(width / resolution);
                    int h = Convert.ToInt32(height / resolution);
                    var bbox2 = BoundsUlica(x1, y1, x3, y3);

                    //DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id, "komunalen");
                    DownloadWmsPlanUlicaSegmenti(bbox2, downloadDirectory + myGuid, w, h, lstOpfatId, "sintezen2");

                    Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2"); ;

                    // Create a picture (A custom view of an Image).
                    Picture picture;
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph;
                    //paragraph.InsertPicture(picture);
                    //paragraph.Alignment = Alignment.center;

                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {

                                    DocX doc1 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc1.PageHeight = 842;
                                        doc1.PageWidth = 595;


                                        paragraph = doc1.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc1.InsertParagraph("", false);
                                        DownloadWmsPlanUlicaSegmenti(bbox2, downloadDirectory + myGuid, w, h, lstOpfatId, "sintezen2");
                                        image = doc1.AddImage(downloadDirectory + myGuid + "sintezen2");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc1.InsertSectionPageBreak();
                                        document.InsertDocument(doc1);
                                        doc1.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;

                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc1.PageHeight = h + 300;
                                        doc1.PageWidth = w;
                                        //doc1.MarginBottom = 500;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        //doc1.MarginTop = 20;
                                        paragraph = doc1.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc1.InsertParagraph("", false);
                                        DownloadWmsPlanUlicaSegmenti(bbox2, downloadDirectory + myGuid, w, h, lstOpfatId,
                                         "sintezen2");
                                        image = doc1.AddImage(downloadDirectory + myGuid + "sintezen2");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc1.InsertSectionPageBreak();
                                        document.InsertDocument(doc1);
                                        doc1.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {

                                    DocX doc2 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc2.PageHeight = 842;
                                        doc2.PageWidth = 595;

                                        paragraph = doc2.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc2.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id, "komunalen");
                                        image = doc2.AddImage(downloadDirectory + myGuid + "komunalen");

                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc2.InsertSectionPageBreak();
                                        document.InsertDocument(doc2);
                                        doc2.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;

                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc2.PageHeight = h + 300;
                                        doc2.PageWidth = w;
                                        doc2.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        doc2.MarginTop = 20;
                                        paragraph = doc2.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc2.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                         "komunalen");
                                        image = doc2.AddImage(downloadDirectory + myGuid + "komunalen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc2.InsertSectionPageBreak();
                                        document.InsertDocument(doc2);
                                        doc2.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {

                                    DocX doc3 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc3.PageHeight = 842;
                                        doc3.PageWidth = 595;
                                        //doc3.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        //doc3.MarginTop = 5;
                                        paragraph = doc3.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc3.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "soobrakaen");
                                        image = doc3.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc3.InsertSectionPageBreak();
                                        document.InsertDocument(doc3);
                                        doc3.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc3.PageHeight = h + 300;
                                        doc3.PageWidth = w;
                                        doc3.MarginBottom = 5;
                                        //doc3.MarginLeft = 5;
                                        //doc3.MarginRight = 10;
                                        doc3.MarginTop = 20;
                                        paragraph = doc3.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc3.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "soobrakaen");
                                        image = doc3.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc3.InsertSectionPageBreak();
                                        document.InsertDocument(doc3);
                                        doc3.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 4:
                                try
                                {

                                    DocX doc4 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc4.PageHeight = 842;
                                        doc4.PageWidth = 595;
                                        //doc4.MarginBottom = 5;
                                        //doc4.MarginLeft = 5;
                                        //doc4.MarginRight = 10;
                                        //doc4.MarginTop = 5;
                                        paragraph = doc4.InsertParagraph("", false);
                                        paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc4.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "spomenici");
                                        image = doc4.AddImage(downloadDirectory + myGuid + "spomenici");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc4.InsertSectionPageBreak();
                                        document.InsertDocument(doc4);
                                        doc4.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc4.PageHeight = h + 300;
                                        doc4.PageWidth = w;
                                        doc4.MarginBottom = 5;
                                        //doc4.MarginLeft = 5;
                                        //doc4.MarginRight = 10;
                                        doc4.MarginTop = 20;
                                        paragraph = doc4.InsertParagraph("", false);
                                        paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc4.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "spomenici");
                                        image = doc4.AddImage(downloadDirectory + myGuid + "spomenici");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc4.InsertSectionPageBreak();
                                        document.InsertDocument(doc4);
                                        doc4.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {

                                    DocX doc5 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc5.PageHeight = 842;
                                        doc5.PageWidth = 595;
                                        //doc5.MarginBottom = 5;
                                        //doc5.MarginLeft = 5;
                                        //doc5.MarginRight = 10;
                                        //doc5.MarginTop = 5;
                                        paragraph = doc5.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc5.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "parking");
                                        image = doc5.AddImage(downloadDirectory + myGuid + "parking");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc5.InsertSectionPageBreak();
                                        document.InsertDocument(doc5);
                                        doc5.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc5.PageHeight = h + 300;
                                        doc5.PageWidth = w;
                                        doc5.MarginBottom = 5;
                                        //doc5.MarginLeft = 5;
                                        //doc5.MarginRight = 10;
                                        doc5.MarginTop = 20;
                                        paragraph = doc5.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc5.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "parking");
                                        image = doc5.AddImage(downloadDirectory + myGuid + "parking");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc5.InsertSectionPageBreak();
                                        document.InsertDocument(doc5);
                                        doc5.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 6:
                                try
                                {

                                    DocX doc6 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc6.PageHeight = 842;
                                        doc6.PageWidth = 595;
                                        //doc6.MarginBottom = 5;
                                        //doc6.MarginLeft = 5;
                                        //doc6.MarginRight = 10;
                                        //doc6.MarginTop = 5;
                                        paragraph = doc6.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc6.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "infrastrukturen");
                                        image = doc6.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc6.InsertSectionPageBreak();
                                        document.InsertDocument(doc6);
                                        doc6.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc6.PageHeight = h + 300;
                                        doc6.PageWidth = w;
                                        doc6.MarginBottom = 5;
                                        //doc6.MarginLeft = 5;
                                        //doc6.MarginRight = 10;
                                        doc6.MarginTop = 20;
                                        paragraph = doc6.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc6.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "infrastrukturen");
                                        image = doc6.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc6.InsertSectionPageBreak();
                                        document.InsertDocument(doc6);
                                        doc6.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 7:
                                try
                                {

                                    DocX doc7 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc7.PageHeight = 842;
                                        doc7.PageWidth = 595;
                                        //doc7.MarginBottom = 5;
                                        //doc7.MarginLeft = 5;
                                        //doc7.MarginRight = 10;
                                        //doc7.MarginTop = 5;
                                        paragraph = doc7.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc7.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "zelenilo");
                                        image = doc7.AddImage(downloadDirectory + myGuid + "zelenilo");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc7.InsertSectionPageBreak();
                                        document.InsertDocument(doc7);
                                        doc7.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc7.PageHeight = h + 300;
                                        doc7.PageWidth = w;
                                        doc7.MarginBottom = 5;
                                        //doc7.MarginLeft = 5;
                                        //doc7.MarginRight = 10;
                                        doc7.MarginTop = 20;
                                        paragraph = doc7.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc7.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "zelenilo");
                                        image = doc7.AddImage(downloadDirectory + myGuid + "zelenilo");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc7.InsertSectionPageBreak();
                                        document.InsertDocument(doc7);
                                        doc7.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 8:
                                try
                                {

                                    DocX doc8 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc8.PageHeight = 842;
                                        doc8.PageWidth = 595;
                                        //doc8.MarginBottom = 5;
                                        //doc8.MarginLeft = 5;
                                        //doc8.MarginRight = 10;
                                        //doc8.MarginTop = 5;
                                        paragraph = doc8.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc8.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "hidrotehnicki");
                                        image = doc8.AddImage(downloadDirectory + myGuid + "hidrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc8.InsertSectionPageBreak();
                                        document.InsertDocument(doc8);
                                        doc8.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc8.PageHeight = h + 300;
                                        doc8.PageWidth = w;
                                        doc8.MarginBottom = 5;
                                        //doc8.MarginLeft = 5;
                                        //doc8.MarginRight = 10;
                                        doc8.MarginTop = 20;
                                        paragraph = doc8.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc8.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "hidrotehnicki");
                                        image = doc8.AddImage(downloadDirectory + myGuid + "hidrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc8.InsertSectionPageBreak();
                                        document.InsertDocument(doc8);
                                        doc8.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничкa инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                            case 9:
                                try
                                {

                                    DocX doc9 = DocX.Create(docOutputPath);
                                    if (h < 842 && w < 595)
                                    {
                                        doc9.PageHeight = 842;
                                        doc9.PageWidth = 595;
                                        //doc9.MarginBottom = 5;
                                        //doc9.MarginLeft = 5;
                                        //doc9.MarginRight = 10;
                                        //doc9.MarginTop = 5;
                                        paragraph = doc9.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc9.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "elektrotehnicki");
                                        image = doc9.AddImage(downloadDirectory + myGuid + "elektrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc9.InsertSectionPageBreak();
                                        document.InsertDocument(doc9);
                                        doc9.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    else
                                    {

                                        doc9.PageHeight = h + 300;
                                        doc9.PageWidth = w;
                                        doc9.MarginBottom = 5;
                                        //doc9.MarginLeft = 5;
                                        //doc9.MarginRight = 10;
                                        doc9.MarginTop = 20;
                                        paragraph = doc9.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = doc9.InsertParagraph("", false);
                                        DownloadWmsPlanUlica(bbox2, downloadDirectory + myGuid, w, h, opfat.Id,
                                            "elektrotehnicki");
                                        image = doc9.AddImage(downloadDirectory + myGuid + "elektrotehnicki");
                                        //picture = image.CreatePicture(420, 630);
                                        picture = image.CreatePicture();
                                        paragraph.InsertPicture(picture);
                                        doc9.InsertSectionPageBreak();
                                        document.InsertDocument(doc9);
                                        doc9.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Електротехничка инфрастуктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;



                        }
                    }

                    //var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                    //if (parcela.OpstiUsloviId.HasValue)
                    //{
                    //    var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                    //    string uri;
                    //    if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                    //        uri = usloviDirectory + opsti.Path;
                    //    else
                    //        uri = opsti.Path;

                    //    //da se vidi kako e so memorijata
                    //    DocX documentOpsti = DocX.Load(uri);
                    //    document.InsertDocument(documentOpsti);
                    //    document.InsertSectionPageBreak();
                    //}
                    //if (parcela.PosebniUsloviId.HasValue)
                    //{
                    //    var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                    //    string uri;
                    //    if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                    //        uri = usloviDirectory + posebni.Path;
                    //    else
                    //        uri = posebni.Path;
                    //    //da se vidi kako e so memorijata
                    //    DocX documentOpsti = DocX.Load(uri);
                    //    document.InsertDocument(documentOpsti);
                    //}

                    document.Save();

                } // Release this document from memory.

                InsertLogsUlici(user.UserName, opfat.Ime, outFileName.ToString());
                return outFileName;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }

        /// <summary>
        /// Generira izvod od plan
        /// </summary>
        /// <param name="coordinates">koordinati vo format x,y</param>
        /// <returns>file path</returns>
        public static string GenerateIzvodAerodrom(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                var resolution = GetResolutionForScale(1000);
                var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);

                //var check = CheckBbox(bbox.Bottom, bbox.Left, bbox.Right, bbox.Top, parcela.Id);

                if (parcela.Id == 479)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.01_JI_01.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                else if (parcela.Id == 489)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.03_JI_01.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 487)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.06_JI_01.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;

                }
                else if (parcela.Id == 1781)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_6.24_I_10.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 943)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.1_Crkva_B.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 958)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_7.1_Crkva_B.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 11)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.1_I14.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 8)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.4_I14.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 5)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.7_I14.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.10_I14.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 14299)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.10_DUP_LisiceBaraki.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 14306)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_3.1_DUP_LisiceBaraki.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 14307)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_6.1_DUP_LisiceBaraki.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1659)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_3.23_DUP_GradskiCetvrt_JI02.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1655)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_3.24_DUP_GradskiCetvrt_JI02s.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1771)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_2.1_DUP_GradskiCetvrt_I10.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1767)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_2.7_DUP_GradskiCetvrt_I10.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1791)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_3.1_DUP_GradskiCetvrt_I10.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1811)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_4.1_DUP_GradskiCetvrt_I10.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1699)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_4.2_DUP_GradskiCetvrt_I10.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1726)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5.32_DUP_GradskiCetvrt_I10.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 155)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_8.2_DUP_I11.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 535)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_6.8_DUP_GornoLisice_UE_E.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 520)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_7.14_DUP_GornoLisice_UE_E.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1406)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_2.1_DUP_GornoLisice_UE_D.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1173)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_3.45_DUP_GornoLisice_UE_D.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 355)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1_36_1_DUP_Jane_sandanski_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 419)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1_37_DUP_Jane_sandanski_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 367)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_2_1_DUP_Jane_sandanski_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 341)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_2_29_DUP_Jane_sandanski_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 88)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_3.1_DUP_Crkva_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 92)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_3.3_DUP_Crkva_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 61)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_4.5_DUP_Crkva_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 13902)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_1_DUP_UB_3.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 13804)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_19_1_DUP_UB_3.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 443)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1_36_DUP_Jane_Sandanski_UE_B.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 875)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_DUP_biser_ARM_delA_UE_B.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 870)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_7_DUP_biser_ARM_delA_UE_B.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 992)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_1_DUP_Reonski_centar_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 991)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_2_DUP_Reonski_centar_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 990)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_3_DUP_Reonski_centar_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 1010)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_4_DUP_Reonski_centar_UE_A.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 13067)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_13_11_DUP_nas_lisice_UB_1.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 12643)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1_13_DUP_Mite_bogoevski.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 313)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1_2_DUP_UE_B_Blok_6_7_8.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 311)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_2_1_DUP_UE_B_Blok_6_7_8.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 312)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_2_2_DUP_UE_B_Blok_6_7_8.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 796)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1_2_LUPD_ind_zona_UE_B.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }


                else if (parcela.Id == 33)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_5_DUP_UE_V.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 288)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_8_Ind_zona_UE_V.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 289)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_10_Ind_zona_UE_V.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 488)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1_2_JI_01.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 485)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1_05_JI_01.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 19083)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_2.1_DUP_I12_blok_2.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else if (parcela.Id == 19086)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_2.4_DUP_I12_blok_2.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());
                    return fullPath;
                }
                else
                {
                    var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                    if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                    var myGuid = Guid.NewGuid();
                    var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                    //get path to template and instance output
                    string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_aerodrom.docx";
                    string docOutputPath = downloadDirectory + outFileName;
                    ////create copy of template so that we don't overwrite it
                    File.Copy(docTemplatePath, docOutputPath);
                    // Load a .docx file
                    using (DocX document = DocX.Load(docOutputPath))
                    {
                        document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                        document.AddCustomProperty(new CustomProperty("gradonacalnik",
                            ConfigurationManager.AppSettings["gradonacalnik"]));
                        if (opfat.TipPlan == 5)
                        {
                            document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", opfat.Ime));
                            document.AddCustomProperty(new CustomProperty("dup_ime_plan", ""));
                        }
                        else
                        {
                            document.AddCustomProperty(new CustomProperty("dup_ime_plan", opfat.Ime));
                            document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", ""));
                        }
                        document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                        document.AddCustomProperty(new CustomProperty("sl_vesnik", opfat.SlVesnik));
                        document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                            ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                            : new CustomProperty("odluka_od", ""));
                        document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                        document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                        document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                        document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                        document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                        document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                        document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                        document.AddCustomProperty(parcela.Povrshina != null
                            ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                            : new CustomProperty("povrsina", ""));
                        document.AddCustomProperty(parcela.PovrshinaGradenje != null
                            ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                            : new CustomProperty("povrsina_za_gradba", ""));
                        document.AddCustomProperty(parcela.BrutoPovrshina != null
                            ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                            : new CustomProperty("bruto_razviena", ""));
                        document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                        document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                            ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                            : new CustomProperty("iskoristenost", ""));
                        document.AddCustomProperty(parcela.ProcentIzgradenost != null
                            ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                            : new CustomProperty("izgradenost", ""));

                        //generate picture
                        centroid = new ParceliDa().GetCentroidById(parcela.Id);
                        resolution = GetResolutionForScale(1000);
                        bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "sintezen2");

                        Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                        // Create a picture (A custom view of an Image).
                        Picture picture = image.CreatePicture(420, 630);
                        // Insert an emptyParagraph into this document.
                        Paragraph paragraph = document.Paragraphs[12];
                        //paragraph.InsertPicture(picture);
                        //paragraph.Alignment = Alignment.center;
                        // Save changes made to this document

                        foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                        {
                            switch (legenda.TipNaPodatokId)
                            {
                                case 1:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "sintezen2");
                                        image = document.AddImage(downloadDirectory + myGuid + "sintezen2");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 2:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "komunalen");
                                        image = document.AddImage(downloadDirectory + myGuid + "komunalen");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 3:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "soobrakaen");
                                        image = document.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 4:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "spomenici");
                                        image = document.AddImage(downloadDirectory + myGuid + "spomenici");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 5:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "parking");
                                        image = document.AddImage(downloadDirectory + myGuid + "parking");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 6:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "infrastrukturen");
                                        image = document.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 7:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "zelenilo");
                                        image = document.AddImage(downloadDirectory + myGuid + "zelenilo");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                            }
                        }

                        document.Save();
                    } // Release this document from memory.

                    //generate Zip
                    var month = DateTime.Now.Month;
                    var year = DateTime.Now.Year;
                    var folderName = year + "" + month;
                    if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                        Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                    List<string> filesToAdd = new List<string>();
                    filesToAdd.Add(docOutputPath);

                    var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                    if (parcela.OpstiUsloviId.HasValue)
                    {
                        var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                        //da se vidi kako e so memorijata
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                            uri = usloviDirectory + opsti.Path;
                        else
                            uri = opsti.Path;
                        filesToAdd.Add(uri);
                    }
                    if (parcela.PosebniUsloviId.HasValue)
                    {
                        var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                        //da se vidi kako e so memorijata
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                            uri = usloviDirectory + posebni.Path;
                        else
                            uri = posebni.Path;
                        filesToAdd.Add(uri);
                    }
                    if (parcela.NumerickiPokazateliId.HasValue)
                    {
                        var numericki = new NumerickiPokazateliDa().GetNumericki(parcela.NumerickiPokazateliId.Value);
                        //da se vidi kako e so memorijata
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(numericki.Path))
                            uri = usloviDirectory + numericki.Path;
                        else
                            uri = numericki.Path;
                        filesToAdd.Add(uri);
                    }
                    var fullPath = folderName + "\\Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" +
                                   Guid.NewGuid() + ".zip";

                    CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                    DeleteFilesFromSystem(new List<string> { docOutputPath });

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }

        public static string GenerateIzvodPrilep(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                var resolution = GetResolutionForScale(1000);
                var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                var check = CheckBbox(bbox.Bottom, bbox.Left, bbox.Right, bbox.Top, parcela.Id);

                if (parcela.Id == 22590)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_UPVNM_PK_3177_1_3177_2_3177_3_4_6_8_1.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 22919)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_UE_1_UB_1_04_126.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 22918)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_UE_1_UB_1_04_126.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 22903)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_UE_1_UB_1_04_159.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 22901)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_UE_1_UB_1_04_160.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 23053)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_UE_1_UB_1_04_161.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 23055)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_LUPD_lesna_nezagaduvacka_industrija_1_1.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 23523)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_UE_15_UB_15_01_opfat_2_grobista_2.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                
                 if (parcela.Id == 28621)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_UPD_cetvrt_2_blok_2_19_1.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 28619)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_UPD_cetvrt_2_blok_2_19_2.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 29690)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_UAE21_UB21_2_UM1_8.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 29693)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_UAE21_UB21_2_UM1_11.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 30510)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_UE18_UB18_03_opfat_1_PIVARA_3.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 41387)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_cetvrt_10_UB_10.3_5.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 41394)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_DUP_cetvrt_10_UB_10.3_31.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                else if (check == false)
                {
                    return GenerateIzvodPrilepA3(coordinates);
                }
               else
                {

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_prilep.docx";
                string docOutputPath = downloadDirectory + outFileName;

                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);
                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {

                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));
                    document.AddCustomProperty(new CustomProperty("kontroliral",
                        ConfigurationManager.AppSettings["kontroliral"]));
                    if (opfat.TipPlan == 6)
                    {
                        document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", opfat.Ime));
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", ""));
                    }
                    else
                    {
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", opfat.Ime));
                        document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", ""));
                    }

                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                    document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                    document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                    document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                    document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                    document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                    document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                    document.AddCustomProperty(parcela.Povrshina != null
                        ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                        : new CustomProperty("povrsina", ""));
                    document.AddCustomProperty(parcela.PovrshinaGradenje != null
                        ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                        : new CustomProperty("povrsina_za_gradba", ""));
                    document.AddCustomProperty(parcela.BrutoPovrshina != null
                        ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                        : new CustomProperty("bruto_razviena", ""));
                    document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                    document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                        ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                        : new CustomProperty("iskoristenost", ""));
                    document.AddCustomProperty(parcela.ProcentIzgradenost != null
                        ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                        : new CustomProperty("izgradenost", ""));

                    //generate picture
                    centroid = new ParceliDa().GetCentroidById(parcela.Id);
                    resolution = GetResolutionForScale(1000);
                    bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "sintezen2");

                    Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                    // Create a picture (A custom view of an Image).
                    Picture picture = image.CreatePicture(420, 630);
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph = document.Paragraphs[12];
                    //paragraph.InsertPicture(picture);
                    //paragraph.Alignment = Alignment.center;
                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Синтезна - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "sintezen2");
                                    image = document.AddImage(downloadDirectory + myGuid + "sintezen2");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "komunalen");
                                    image = document.AddImage(downloadDirectory + myGuid + "komunalen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "soobrakaen");
                                    image = document.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 4:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "spomenici");
                                    image = document.AddImage(downloadDirectory + myGuid + "spomenici");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "parking");
                                    image = document.AddImage(downloadDirectory + myGuid + "parking");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 6:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                        "infrastrukturen");
                                    image = document.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 7:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "zelenilo");
                                    image = document.AddImage(downloadDirectory + myGuid + "zelenilo");
                                    picture = image.CreatePicture(420, 630);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;


                            }
                        }

                        document.Save();
                    } // Release this document from memory.

                    //generate Zip
                    var month = DateTime.Now.Month;
                    var year = DateTime.Now.Year;
                    var folderName = year + "" + month;
                    if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                        Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                    List<string> filesToAdd = new List<string>();
                    filesToAdd.Add(docOutputPath);

                    var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                    if (parcela.OpstiUsloviId.HasValue)
                    {
                        var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                        //da se vidi kako e so memorijata
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                            uri = usloviDirectory + opsti.Path;
                        else
                            uri = opsti.Path;
                        filesToAdd.Add(uri);
                    }
                    if (parcela.PosebniUsloviId.HasValue)
                    {
                        var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                        //da se vidi kako e so memorijata
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                            uri = usloviDirectory + posebni.Path;
                        else
                            uri = posebni.Path;
                        filesToAdd.Add(uri);
                    }
                    var ime = "Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" + Guid.NewGuid() + ".zip";
                    var fullPath = folderName + "\\" + ime;

                    CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                    DeleteFilesFromSystem(new List<string> { docOutputPath });

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, year + "" + month + "\\" + ime);
                    return fullPath;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }

        public static string GenerateIzvodPrilepA3(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_prilep_a3.docx";
                string docOutputPath = downloadDirectory + outFileName;

                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);
                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {
                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));
                    document.AddCustomProperty(new CustomProperty("kontroliral",
                        ConfigurationManager.AppSettings["kontroliral"]));
                    if (opfat.TipPlan == 6)
                    {
                        document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", opfat.Ime));
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", ""));
                    }
                    else
                    {
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", opfat.Ime));
                        document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", ""));
                    }

                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                    document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                    document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                    document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                    document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                    document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                    document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                    document.AddCustomProperty(parcela.Povrshina != null
                        ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                        : new CustomProperty("povrsina", ""));
                    document.AddCustomProperty(parcela.PovrshinaGradenje != null
                        ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                        : new CustomProperty("povrsina_za_gradba", ""));
                    document.AddCustomProperty(parcela.BrutoPovrshina != null
                        ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                        : new CustomProperty("bruto_razviena", ""));
                    document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                    document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                        ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                        : new CustomProperty("iskoristenost", ""));
                    document.AddCustomProperty(parcela.ProcentIzgradenost != null
                        ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                        : new CustomProperty("izgradenost", ""));

                    //generate picture
                    var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                    var resolution = GetResolutionForScale(1000);
                    var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 1800, 1800);
                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "sintezen2");

                    Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                    // Create a picture (A custom view of an Image).
                    Picture picture = image.CreatePicture(1000, 1000);
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph = document.Paragraphs[24];
                    //paragraph.InsertPicture(picture);
                    //paragraph.Alignment = Alignment.center;
                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {
                                    document.InsertSectionPageBreak();
                                    DocX doc1 = DocX.Create(docOutputPath);
                                    doc1.PageHeight = 1191;
                                    doc1.PageWidth = 842;
                                    paragraph = doc1.InsertParagraph("", false);
                                    paragraph.Append("Синтезна - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc1.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1900, 1900, parcela.Id,
                                        "sintezen2");
                                    image = doc1.AddImage(downloadDirectory + myGuid + "sintezen2");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc1.InsertSectionPageBreak();


                                    paragraph = doc1.InsertParagraph("", false);
                                    paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc1.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc1.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc1.InsertSectionPageBreak();
                                    doc1.Save();
                                    document.InsertDocument(doc1);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {
                                    DocX doc2 = DocX.Create(docOutputPath);
                                    doc2.PageHeight = 1191;
                                    doc2.PageWidth = 842;
                                    paragraph = doc2.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc2.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "komunalen");
                                    image = doc2.AddImage(downloadDirectory + myGuid + "komunalen");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc2.InsertSectionPageBreak();
                                    paragraph = doc2.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc2.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc2.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc2.InsertSectionPageBreak();
                                    doc2.Save();
                                    document.InsertDocument(doc2);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {
                                    DocX doc3 = DocX.Create(docOutputPath);
                                    doc3.PageHeight = 1191;
                                    doc3.PageWidth = 842;
                                    paragraph = doc3.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc3.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1900, 1900, parcela.Id,
                                        "soobrakaen");
                                    image = doc3.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc3.InsertSectionPageBreak();

                                    paragraph = doc3.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc3.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc3.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc3.InsertSectionPageBreak();
                                    doc3.Save();
                                    document.InsertDocument(doc3);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 4:
                                try
                                {
                                    DocX doc4 = DocX.Create(docOutputPath);
                                    doc4.PageHeight = 1191;
                                    doc4.PageWidth = 842;
                                    paragraph = doc4.InsertParagraph("", false);
                                    paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc4.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "spomenici");
                                    image = doc4.AddImage(downloadDirectory + myGuid + "spomenici");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc4.InsertSectionPageBreak();
                                    paragraph = doc4.InsertParagraph("", false);
                                    paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc4.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc4.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc4.InsertSectionPageBreak();
                                    doc4.Save();
                                    document.InsertDocument(doc4);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {
                                    DocX doc5 = DocX.Create(docOutputPath);
                                    doc5.PageHeight = 1191;
                                    doc5.PageWidth = 842;
                                    paragraph = doc5.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc5.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "parking");
                                    image = doc5.AddImage(downloadDirectory + myGuid + "parking");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc5.InsertSectionPageBreak();
                                    paragraph = doc5.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc5.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc5.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc5.InsertSectionPageBreak();
                                    doc5.Save();
                                    document.InsertDocument(doc5);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 6:
                                try
                                {
                                    DocX doc6 = DocX.Create(docOutputPath);
                                    doc6.PageHeight = 1191;
                                    doc6.PageWidth = 842;
                                    paragraph = doc6.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc6.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "infrastrukturen");
                                    image = doc6.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc6.InsertSectionPageBreak();
                                    paragraph = doc6.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc6.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc6.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc6.InsertSectionPageBreak();
                                    doc6.Save();
                                    document.InsertDocument(doc6);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 7:
                                try
                                {
                                    DocX doc7 = DocX.Create(docOutputPath);
                                    doc7.PageHeight = 1191;
                                    doc7.PageWidth = 842;
                                    paragraph = doc7.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc7.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "zelenilo");
                                    image = doc7.AddImage(downloadDirectory + myGuid + "zelenilo");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc7.InsertSectionPageBreak();
                                    paragraph = doc7.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc7.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc7.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc7.InsertSectionPageBreak();
                                    doc7.Save();
                                    document.InsertDocument(doc7);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                        }
                    }

                    document.Save();
                } // Release this document from memory.

                //generate Zip
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var folderName = year + "" + month;
                if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                    Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                List<string> filesToAdd = new List<string>();
                filesToAdd.Add(docOutputPath);

                var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                if (parcela.OpstiUsloviId.HasValue)
                {
                    var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                        uri = usloviDirectory + opsti.Path;
                    else
                        uri = opsti.Path;
                    filesToAdd.Add(uri);
                }
                if (parcela.PosebniUsloviId.HasValue)
                {
                    var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                        uri = usloviDirectory + posebni.Path;
                    else
                        uri = posebni.Path;
                    filesToAdd.Add(uri);
                }
                var ime = "Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" + Guid.NewGuid() + ".zip";
                var fullPath = folderName + "\\" + ime;

                CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                DeleteFilesFromSystem(new List<string> { docOutputPath });

                InsertLogs(user.UserName, opfat.Ime, parcela.Broj, year + "" + month + "\\" + ime);
                return fullPath;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }
        public static string GenerateIzvodGaziBaba(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                var resolution = GetResolutionForScale(1000);
                var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                var check = CheckBbox(bbox.Bottom, bbox.Left, bbox.Right, bbox.Top, parcela.Id);


                if (parcela.Id == 516)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_4_fakulteti.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 515)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_5_fakulteti.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 1945)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_11.74_Hipodrom.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                if (parcela.Id == 2433)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.1_SI_10.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 3125)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_3.2_UPVNM_RekreativenCentar.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 3169)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.1_S_17.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 3170)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.2_S_17.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 3167)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.3_S_17.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 3165)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.5_S_17.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }


                if (parcela.Id == 3160)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.6_S_17.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 3399)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_16_CS_09.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 3639)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1.1_Izmena_Madzari_IndustriskaZona(centralen_del).zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }

                if (parcela.Id == 5627)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_12.1_Obikolnica.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                else if (check == false)
                {
                    return GenerateIzvodGaziBabaA3(coordinates);
                }


                else
                {

                    var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                    if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                    var myGuid = Guid.NewGuid();
                    var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                    //get path to template and instance output
                    string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_gazibaba.docx";
                    string docOutputPath = downloadDirectory + outFileName;

                    ////create copy of template so that we don't overwrite it
                    File.Copy(docTemplatePath, docOutputPath);
                    // Load a .docx file
                    using (DocX document = DocX.Load(docOutputPath))
                    {

                        document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                        document.AddCustomProperty(new CustomProperty("gradonacalnik",
                            ConfigurationManager.AppSettings["gradonacalnik"]));
                        document.AddCustomProperty(new CustomProperty("kontroliral",
                            ConfigurationManager.AppSettings["kontroliral"]));
                        if (opfat.TipPlan == 6)
                        {
                            document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", opfat.Ime));
                            document.AddCustomProperty(new CustomProperty("dup_ime_plan", ""));
                        }
                        else
                        {
                            document.AddCustomProperty(new CustomProperty("dup_ime_plan", opfat.Ime));
                            document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", ""));
                        }

                        document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                        document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                            ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                            : new CustomProperty("odluka_od", ""));
                        document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                        document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                        document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                        document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                        document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                        document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                        document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                        document.AddCustomProperty(parcela.Povrshina != null
                            ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                            : new CustomProperty("povrsina", ""));
                        document.AddCustomProperty(parcela.PovrshinaGradenje != null
                            ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                            : new CustomProperty("povrsina_za_gradba", ""));
                        document.AddCustomProperty(parcela.BrutoPovrshina != null
                            ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                            : new CustomProperty("bruto_razviena", ""));
                        document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                        document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                            ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                            : new CustomProperty("iskoristenost", ""));
                        document.AddCustomProperty(parcela.ProcentIzgradenost != null
                            ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                            : new CustomProperty("izgradenost", ""));

                        //generate picture
                        centroid = new ParceliDa().GetCentroidById(parcela.Id);
                        resolution = GetResolutionForScale(1000);
                        bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "sintezen2");

                        Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                        // Create a picture (A custom view of an Image).
                        Picture picture = image.CreatePicture(420, 630);
                        // Insert an emptyParagraph into this document.
                        Paragraph paragraph = document.Paragraphs[12];
                        //paragraph.InsertPicture(picture);
                        //paragraph.Alignment = Alignment.center;
                        // Save changes made to this document

                        foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                        {
                            switch (legenda.TipNaPodatokId)
                            {
                                case 1:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "sintezen2");
                                        image = document.AddImage(downloadDirectory + myGuid + "sintezen2");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 2:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "komunalen");
                                        image = document.AddImage(downloadDirectory + myGuid + "komunalen");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 3:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "soobrakaen");
                                        image = document.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 4:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "spomenici");
                                        image = document.AddImage(downloadDirectory + myGuid + "spomenici");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 5:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "parking");
                                        image = document.AddImage(downloadDirectory + myGuid + "parking");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 6:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "infrastrukturen");
                                        image = document.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 7:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "zelenilo");
                                        image = document.AddImage(downloadDirectory + myGuid + "zelenilo");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 8:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Електроенергетска инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "elektroenergetski");
                                        image = document.AddImage(downloadDirectory + myGuid + "elektroenergetski");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Електроенергетска инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 9:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничка инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "hidrotehnicki");
                                        image = document.AddImage(downloadDirectory + myGuid + "hidrotehnicki");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Хидротехничка инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 10:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Водоводна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "vodovoden");
                                        image = document.AddImage(downloadDirectory + myGuid + "vodovoden");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Водоводна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 11:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Гасоводна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "gasovoden");
                                        image = document.AddImage(downloadDirectory + myGuid + "gasovoden");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Гасоводна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 12:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Фекална и атмосферска инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "fekalen_kanalizaciski");
                                        image = document.AddImage(downloadDirectory + myGuid + "fekalen_kanalizaciski");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Фекална и атмосферска инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 13:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Телекомуникациска инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "telefonska_mreza");
                                        image = document.AddImage(downloadDirectory + myGuid + "telefonska_mreza");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Телекомуникациска инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;

                            }
                        }

                        document.Save();
                    } // Release this document from memory.

                    //generate Zip
                    var month = DateTime.Now.Month;
                    var year = DateTime.Now.Year;
                    var folderName = year + "" + month;
                    if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                        Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                    List<string> filesToAdd = new List<string>();
                    filesToAdd.Add(docOutputPath);

                    var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                    if (parcela.OpstiUsloviId.HasValue)
                    {
                        var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                        //da se vidi kako e so memorijata
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                            uri = usloviDirectory + opsti.Path;
                        else
                            uri = opsti.Path;
                        filesToAdd.Add(uri);
                    }
                    if (parcela.PosebniUsloviId.HasValue)
                    {
                        var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                        //da se vidi kako e so memorijata
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                            uri = usloviDirectory + posebni.Path;
                        else
                            uri = posebni.Path;
                        filesToAdd.Add(uri);
                    }
                    var ime = "Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" + Guid.NewGuid() + ".zip";
                    var fullPath = folderName + "\\" + ime;

                    CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                    DeleteFilesFromSystem(new List<string> { docOutputPath });

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, year + "" + month + "\\" + ime);
                    return fullPath;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }



        public static string GenerateIzvodGaziBabaA3(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_gazibaba2.docx";
                string docOutputPath = downloadDirectory + outFileName;



                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);
                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {

                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));
                    document.AddCustomProperty(new CustomProperty("kontroliral",
                        ConfigurationManager.AppSettings["kontroliral"]));
                    if (opfat.TipPlan == 6)
                    {
                        document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", opfat.Ime));
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", ""));
                    }
                    else
                    {
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", opfat.Ime));
                        document.AddCustomProperty(new CustomProperty("upvnm_ime_plan", ""));
                    }

                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                    document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                    document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                    document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));
                    document.InsertSectionPageBreak();

                    document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                    document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                    document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                    document.AddCustomProperty(parcela.Povrshina != null
                        ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                        : new CustomProperty("povrsina", ""));
                    document.AddCustomProperty(parcela.PovrshinaGradenje != null
                        ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                        : new CustomProperty("povrsina_za_gradba", ""));
                    document.AddCustomProperty(parcela.BrutoPovrshina != null
                        ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                        : new CustomProperty("bruto_razviena", ""));
                    document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                    document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                        ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                        : new CustomProperty("iskoristenost", ""));
                    document.AddCustomProperty(parcela.ProcentIzgradenost != null
                        ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                        : new CustomProperty("izgradenost", ""));

                    //generate picture
                    var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                    var resolution = GetResolutionForScale(1000);
                    var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 1800, 1800);
                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "sintezen2");

                    Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                    // Create a picture (A custom view of an Image).
                    Picture picture = image.CreatePicture(1000, 1000);
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph = document.Paragraphs[24];
                    //paragraph.InsertPicture(picture);
                    //paragraph.Alignment = Alignment.center;
                    // Save changes made to this document


                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {
                                    DocX doc1 = DocX.Create(docOutputPath);
                                    doc1.PageHeight = 1191;
                                    doc1.PageWidth = 842;
                                    paragraph = doc1.InsertParagraph("", false);
                                    paragraph.Append("Синтезна - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc1.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1900, 1900, parcela.Id,
                                        "sintezen2");
                                    image = doc1.AddImage(downloadDirectory + myGuid + "sintezen2");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc1.InsertSectionPageBreak();


                                    paragraph = doc1.InsertParagraph("", false);
                                    paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc1.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc1.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc1.InsertSectionPageBreak();
                                    doc1.Save();
                                    document.InsertDocument(doc1);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {
                                    DocX doc2 = DocX.Create(docOutputPath);
                                    doc2.PageHeight = 1191;
                                    doc2.PageWidth = 842;
                                    paragraph = doc2.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc2.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "komunalen");
                                    image = doc2.AddImage(downloadDirectory + myGuid + "komunalen");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc2.InsertSectionPageBreak();
                                    paragraph = doc2.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc2.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc2.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc2.InsertSectionPageBreak();
                                    doc2.Save();
                                    document.InsertDocument(doc2);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {
                                    DocX doc3 = DocX.Create(docOutputPath);
                                    doc3.PageHeight = 1191;
                                    doc3.PageWidth = 842;
                                    paragraph = doc3.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc3.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1900, 1900, parcela.Id,
                                        "soobrakaen");
                                    image = doc3.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc3.InsertSectionPageBreak();

                                    paragraph = doc3.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc3.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc3.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc3.InsertSectionPageBreak();
                                    doc3.Save();
                                    document.InsertDocument(doc3);

                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 4:
                                try
                                {
                                    DocX doc4 = DocX.Create(docOutputPath);
                                    doc4.PageHeight = 1191;
                                    doc4.PageWidth = 842;
                                    paragraph = doc4.InsertParagraph("", false);
                                    paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc4.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "spomenici");
                                    image = doc4.AddImage(downloadDirectory + myGuid + "spomenici");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc4.InsertSectionPageBreak();
                                    paragraph = doc4.InsertParagraph("", false);
                                    paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc4.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc4.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc4.InsertSectionPageBreak();
                                    doc4.Save();
                                    document.InsertDocument(doc4);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {
                                    DocX doc5 = DocX.Create(docOutputPath);
                                    doc5.PageHeight = 1191;
                                    doc5.PageWidth = 842;
                                    paragraph = doc5.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc5.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "parking");
                                    image = doc5.AddImage(downloadDirectory + myGuid + "parking");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc5.InsertSectionPageBreak();
                                    paragraph = doc5.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc5.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc5.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc5.InsertSectionPageBreak();
                                    doc5.Save();
                                    document.InsertDocument(doc5);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 6:
                                try
                                {
                                    DocX doc6 = DocX.Create(docOutputPath);
                                    doc6.PageHeight = 1191;
                                    doc6.PageWidth = 842;
                                    paragraph = doc6.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc6.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "infrastrukturen");
                                    image = doc6.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc6.InsertSectionPageBreak();
                                    paragraph = doc6.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc6.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc6.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc6.InsertSectionPageBreak();
                                    doc6.Save();
                                    document.InsertDocument(doc6);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 7:
                                try
                                {
                                    DocX doc7 = DocX.Create(docOutputPath);
                                    doc7.PageHeight = 1191;
                                    doc7.PageWidth = 842;
                                    paragraph = doc7.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc7.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "zelenilo");
                                    image = doc7.AddImage(downloadDirectory + myGuid + "zelenilo");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc7.InsertSectionPageBreak();
                                    paragraph = doc7.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc7.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc7.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc7.InsertSectionPageBreak();
                                    doc7.Save();
                                    document.InsertDocument(doc7);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 8:
                                try
                                {
                                    DocX doc8 = DocX.Create(docOutputPath);
                                    doc8.PageHeight = 1191;
                                    doc8.PageWidth = 842;
                                    paragraph = doc8.InsertParagraph("", false);
                                    paragraph.Append("Електроенергетска инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc8.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "elektroenergetski");
                                    image = doc8.AddImage(downloadDirectory + myGuid + "elektroenergetski");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc8.InsertSectionPageBreak();
                                    paragraph = doc8.InsertParagraph("", false);
                                    paragraph.Append("Електроенергетска инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc8.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc8.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc8.InsertSectionPageBreak();
                                    doc8.Save();
                                    document.InsertDocument(doc8);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 9:
                                try
                                {
                                    DocX doc9 = DocX.Create(docOutputPath);
                                    doc9.PageHeight = 1191;
                                    doc9.PageWidth = 842;
                                    paragraph = doc9.InsertParagraph("", false);
                                    paragraph.Append("Хидротехничка инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc9.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "hidrotehnicki");
                                    image = doc9.AddImage(downloadDirectory + myGuid + "hidrotehnicki");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc9.InsertSectionPageBreak();
                                    paragraph = doc9.InsertParagraph("", false);
                                    paragraph.Append("Хидротехничка инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc9.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc9.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc9.InsertSectionPageBreak();
                                    doc9.Save();
                                    document.InsertDocument(doc9);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 10:
                                try
                                {
                                    DocX doc10 = DocX.Create(docOutputPath);
                                    doc10.PageHeight = 1191;
                                    doc10.PageWidth = 842;
                                    paragraph = doc10.InsertParagraph("", false);
                                    paragraph.Append("Водоводна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc10.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "vodovoden");
                                    image = doc10.AddImage(downloadDirectory + myGuid + "vodovoden");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc10.InsertSectionPageBreak();
                                    paragraph = doc10.InsertParagraph("", false);
                                    paragraph.Append("Водоводна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc10.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc10.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc10.InsertSectionPageBreak();
                                    doc10.Save();
                                    document.InsertDocument(doc10);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 11:
                                try
                                {
                                    DocX doc11 = DocX.Create(docOutputPath);
                                    doc11.PageHeight = 1191;
                                    doc11.PageWidth = 842;
                                    paragraph = doc11.InsertParagraph("", false);
                                    paragraph.Append("Гасоводна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc11.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "gasovoden");
                                    image = doc11.AddImage(downloadDirectory + myGuid + "gasovoden");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc11.InsertSectionPageBreak();
                                    paragraph = doc11.InsertParagraph("", false);
                                    paragraph.Append("Гасоводна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc11.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc11.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc11.InsertSectionPageBreak();
                                    doc11.Save();
                                    document.InsertDocument(doc11);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 12:
                                try
                                {
                                    DocX doc12 = DocX.Create(docOutputPath);
                                    doc12.PageHeight = 1191;
                                    doc12.PageWidth = 842;
                                    paragraph = doc12.InsertParagraph("", false);
                                    paragraph.Append("Фекална и атмосферска инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc12.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "fekalen_kanalizaciski");
                                    image = doc12.AddImage(downloadDirectory + myGuid + "fekalen_kanalizaciski");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc12.InsertSectionPageBreak();
                                    paragraph = doc12.InsertParagraph("", false);
                                    paragraph.Append("Фекална и атмосферска инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc12.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc12.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc12.InsertSectionPageBreak();
                                    doc12.Save();
                                    document.InsertDocument(doc12);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 13:
                                try
                                {
                                    DocX doc13 = DocX.Create(docOutputPath);
                                    doc13.PageHeight = 1191;
                                    doc13.PageWidth = 842;
                                    paragraph = doc13.InsertParagraph("", false);
                                    paragraph.Append("Телекомуникациска инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc13.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "telefonska_mreza");
                                    image = doc13.AddImage(downloadDirectory + myGuid + "telefonska_mreza");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc13.InsertSectionPageBreak();
                                    paragraph = doc13.InsertParagraph("", false);
                                    paragraph.Append("Телекомуникациска инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc13.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc13.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc13.InsertSectionPageBreak();
                                    doc13.Save();
                                    document.InsertDocument(doc13);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;

                        }
                    }

                    document.Save();
                } // Release this document from memory.

                //generate Zip
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var folderName = year + "" + month;
                if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                    Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                List<string> filesToAdd = new List<string>();
                filesToAdd.Add(docOutputPath);

                var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                if (parcela.OpstiUsloviId.HasValue)
                {
                    var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                        uri = usloviDirectory + opsti.Path;
                    else
                        uri = opsti.Path;
                    filesToAdd.Add(uri);
                }
                if (parcela.PosebniUsloviId.HasValue)
                {
                    var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                        uri = usloviDirectory + posebni.Path;
                    else
                        uri = posebni.Path;
                    filesToAdd.Add(uri);
                }
                var ime = "Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" + Guid.NewGuid() + ".zip";
                var fullPath = folderName + "\\" + ime;

                CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                DeleteFilesFromSystem(new List<string> { docOutputPath });

                InsertLogs(user.UserName, opfat.Ime, parcela.Broj, year + "" + month + "\\" + ime);
                return fullPath;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }



        public static string GenerateIzvodAerodromA3(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod_aerodrom2.docx";
                string docOutputPath = downloadDirectory + outFileName;
                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);
                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {

                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));
                    if (opfat.TipPlan == 5)
                    {
                        document.AddCustomProperty(new CustomProperty("ups_ime_plan", opfat.Ime));
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", ""));
                    }
                    else
                    {
                        document.AddCustomProperty(new CustomProperty("dup_ime_plan", opfat.Ime));
                        document.AddCustomProperty(new CustomProperty("ups_ime_plan", ""));
                    }
                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(new CustomProperty("sl_vesnik", opfat.SlVesnik));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                    document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                    document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                    document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                    document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                    document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                    document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                    document.AddCustomProperty(parcela.Povrshina != null
                        ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                        : new CustomProperty("povrsina", ""));
                    document.AddCustomProperty(parcela.PovrshinaGradenje != null
                        ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                        : new CustomProperty("povrsina_za_gradba", ""));
                    document.AddCustomProperty(parcela.BrutoPovrshina != null
                        ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                        : new CustomProperty("bruto_razviena", ""));
                    document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                    document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                        ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                        : new CustomProperty("iskoristenost", ""));
                    document.AddCustomProperty(parcela.ProcentIzgradenost != null
                        ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                        : new CustomProperty("izgradenost", ""));

                    //generate picture
                    var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                    var resolution = GetResolutionForScale(1000);
                    var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 1800, 1800);
                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "sintezen2");

                    Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                    // Create a picture (A custom view of an Image).
                    Picture picture = image.CreatePicture(1000, 1000);
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph = document.Paragraphs[24];
                    paragraph.InsertPicture(picture);
                    paragraph.Alignment = Alignment.center;
                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "komunalen");
                                    image = document.AddImage(downloadDirectory + myGuid + "komunalen");
                                    picture = image.CreatePicture(1000, 1000);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "soobrakaen");
                                    image = document.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                    picture = image.CreatePicture(1000, 1000);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 4:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "spomenici");
                                    image = document.AddImage(downloadDirectory + myGuid + "spomenici");
                                    picture = image.CreatePicture(1000, 1000);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "parking");
                                    image = document.AddImage(downloadDirectory + myGuid + "parking");
                                    picture = image.CreatePicture(1000, 1000);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 6:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "infrastrukturen");
                                    image = document.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                    picture = image.CreatePicture(1000, 1000);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 7:
                                try
                                {
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = document.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "zelenilo");
                                    image = document.AddImage(downloadDirectory + myGuid + "zelenilo");
                                    picture = image.CreatePicture(1000, 1000);
                                    paragraph.InsertPicture(picture);
                                    document.InsertSectionPageBreak();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = document.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    document.InsertSectionPageBreak();
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                        }
                    }

                    document.Save();
                } // Release this document from memory.



                //generate Zip
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var folderName = year + "" + month;
                if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                    Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                List<string> filesToAdd = new List<string>();
                filesToAdd.Add(docOutputPath);

                var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                if (parcela.OpstiUsloviId.HasValue)
                {
                    var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                        uri = usloviDirectory + opsti.Path;
                    else
                        uri = opsti.Path;
                    filesToAdd.Add(uri);
                }
                if (parcela.PosebniUsloviId.HasValue)
                {
                    var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                        uri = usloviDirectory + posebni.Path;
                    else
                        uri = posebni.Path;
                    filesToAdd.Add(uri);
                }
                var fullPath = folderName + "\\Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" +
                               Guid.NewGuid() + ".zip";

                CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                DeleteFilesFromSystem(new List<string> { docOutputPath });

                InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                return fullPath;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }



        public static string GenerateIzvodKavadarci(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                var resolution = GetResolutionForScale(1000);
                var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                var check = CheckBbox(bbox.Bottom, bbox.Left, bbox.Right, bbox.Top, parcela.Id);

                if (parcela.Id == 10750)
                {
                    var fullPath = "GotoviIzvodi\\Izvod_1_IzmenaIDopolna_UB_22K_23K.zip";

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, fullPath.ToString());

                    return fullPath;
                }
                else if (check == false)
                {
                    return GenerateIzvodKavadarciA3(coordinates);
                }
                else
                {
                    var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                    if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                    var myGuid = Guid.NewGuid();
                    var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                    //get path to template and instance output
                    string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvod.docx";
                    string docOutputPath = downloadDirectory + outFileName;
                    ////create copy of template so that we don't overwrite it
                    File.Copy(docTemplatePath, docOutputPath);
                    // Load a .docx file
                    using (DocX document = DocX.Load(docOutputPath))
                    {
                        document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                        document.AddCustomProperty(new CustomProperty("gradonacalnik",
                            ConfigurationManager.AppSettings["gradonacalnik"]));
                        document.AddCustomProperty(new CustomProperty("ime_plan", opfat.Ime));
                        document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                        document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                            ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                            : new CustomProperty("odluka_od", ""));
                        document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                        document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                        document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                        document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                        document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                        document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                        document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                        document.AddCustomProperty(parcela.Povrshina != null
                            ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                            : new CustomProperty("povrsina", ""));
                        document.AddCustomProperty(parcela.PovrshinaGradenje != null
                            ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                            : new CustomProperty("povrsina_za_gradba", ""));
                        document.AddCustomProperty(parcela.BrutoPovrshina != null
                            ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                            : new CustomProperty("bruto_razviena", ""));
                        document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                        document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                            ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                            : new CustomProperty("iskoristenost", ""));
                        document.AddCustomProperty(parcela.ProcentIzgradenost != null
                            ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                            : new CustomProperty("izgradenost", ""));

                        //generate picture
                        centroid = new ParceliDa().GetCentroidById(parcela.Id);
                        resolution = GetResolutionForScale(1000);
                        bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 600, 400);
                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "sintezen2");

                        Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                        // Create a picture (A custom view of an Image).
                        Picture picture = image.CreatePicture(420, 630);
                        // Insert an emptyParagraph into this document.
                        Paragraph paragraph = document.Paragraphs[12];
                        paragraph.InsertPicture(picture);
                        paragraph.Alignment = Alignment.center;
                        // Save changes made to this document

                        foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                        {
                            switch (legenda.TipNaPodatokId)
                            {
                                case 1:
                                    try
                                    {
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 2:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "komunalen");
                                        image = document.AddImage(downloadDirectory + myGuid + "komunalen");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 3:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "soobrakaen");
                                        image = document.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 4:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "spomenici");
                                        image = document.AddImage(downloadDirectory + myGuid + "spomenici");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 5:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "parking");
                                        image = document.AddImage(downloadDirectory + myGuid + "parking");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 6:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id,
                                            "infrastrukturen");
                                        image = document.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                                case 7:
                                    try
                                    {
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        paragraph = document.InsertParagraph("", false);
                                        DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1200, parcela.Id, "zelenilo");
                                        image = document.AddImage(downloadDirectory + myGuid + "zelenilo");
                                        picture = image.CreatePicture(420, 630);
                                        paragraph.InsertPicture(picture);
                                        document.InsertSectionPageBreak();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                        paragraph.Alignment = Alignment.center;
                                        image =
                                            document.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                              ConfigurationManager.AppSettings["ms4w_app"] +
                                                              "\\data\\" + legenda.Path);
                                        picture = image.CreatePicture();
                                        paragraph = document.InsertParagraph("", false);
                                        paragraph.InsertPicture(picture);
                                        paragraph.Alignment = Alignment.center;
                                        document.InsertSectionPageBreak();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                    }
                                    break;
                            }
                        }

                        document.Save();
                    } // Release this document from memory.

                    //generate Zip
                    var month = DateTime.Now.Month;
                    var year = DateTime.Now.Year;
                    var folderName = year + "" + month;
                    if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                        Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                    List<string> filesToAdd = new List<string>();
                    filesToAdd.Add(docOutputPath);

                    var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                    if (parcela.OpstiUsloviId.HasValue)
                    {
                        var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                        //da se vidi kako e so memorijata
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                            uri = usloviDirectory + opsti.Path;
                        else
                            uri = opsti.Path;
                        filesToAdd.Add(uri);
                    }
                    if (parcela.PosebniUsloviId.HasValue)
                    {
                        var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                        //da se vidi kako e so memorijata
                        string uri;
                        if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                            uri = usloviDirectory + posebni.Path;
                        else
                            uri = posebni.Path;
                        filesToAdd.Add(uri);
                    }
                    var ime = "Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" + Guid.NewGuid() + ".zip";
                    var fullPath = folderName + "\\" + ime;

                    CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                    DeleteFilesFromSystem(new List<string> { docOutputPath });

                    InsertLogs(user.UserName, opfat.Ime, parcela.Broj, year + "" + month + "\\" + ime);
                    return fullPath;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }


        public static string GenerateIzvodKavadarciA3(string coordinates)
        {
            try
            {
                var listParceli = new ParceliDa().GenerateList(coordinates);
                if (listParceli == null || listParceli.Count <= 0) return "";
                var id = (FormsIdentity)HttpContext.Current.User.Identity;
                var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
                var opfat = new OpfatDa().Get(listParceli[0].OpfatId);
                var parcela = listParceli[0];
                var listKatOpstini = new KatOpstiniDa().GetIntersect(parcela.Id);
                var katOpstini = listKatOpstini.Aggregate("", (current, item) => current + (item.Ime + ", "));
                //proveri sto piovi na planovi ima
                var legendi = new LegendDa().Get(opfat.Id);

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Izvodi\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var outFileName = string.Format("Izvod_{0}_{1}.docx", parcela.Id, myGuid);

                //get path to template and instance output
                string docTemplatePath = HttpRuntime.AppDomainAppPath + "Templates\\izvoda3.docx";
                string docOutputPath = downloadDirectory + outFileName;

                ////create copy of template so that we don't overwrite it
                File.Copy(docTemplatePath, docOutputPath);
                // Load a .docx file
                using (DocX document = DocX.Load(docOutputPath))
                {
                    document.AddCustomProperty(new CustomProperty("opstina", ConfigurationManager.AppSettings["opstina"]));
                    document.AddCustomProperty(new CustomProperty("gradonacalnik",
                        ConfigurationManager.AppSettings["gradonacalnik"]));
                    document.AddCustomProperty(new CustomProperty("ime_plan", opfat.Ime));
                    document.AddCustomProperty(new CustomProperty("odluka_broj", opfat.BrOdluka));
                    document.AddCustomProperty(opfat.DatumNaDonesuvanje != null
                        ? new CustomProperty("odluka_od", opfat.DatumNaDonesuvanje.Value.ToString("dd.MM.yyyy"))
                        : new CustomProperty("odluka_od", ""));
                    document.AddCustomProperty(new CustomProperty("namena", parcela.KlasaNamena));
                    document.AddCustomProperty(new CustomProperty("kat_o", katOpstini));
                    document.AddCustomProperty(new CustomProperty("br_parcela", parcela.Broj));
                    document.AddCustomProperty(new CustomProperty("izgotvil", user.FullName));

                    document.AddCustomProperty(new CustomProperty("komp_klasa", parcela.KompKlasaNamena));
                    document.AddCustomProperty(new CustomProperty("maks_visina", parcela.MaxVisina));
                    document.AddCustomProperty(new CustomProperty("maks_katnost", parcela.Katnost));
                    document.AddCustomProperty(parcela.Povrshina != null
                        ? new CustomProperty("povrsina", parcela.Povrshina.Value)
                        : new CustomProperty("povrsina", ""));
                    document.AddCustomProperty(parcela.PovrshinaGradenje != null
                        ? new CustomProperty("povrsina_za_gradba", parcela.PovrshinaGradenje.Value)
                        : new CustomProperty("povrsina_za_gradba", ""));
                    document.AddCustomProperty(parcela.BrutoPovrshina != null
                        ? new CustomProperty("bruto_razviena", parcela.BrutoPovrshina.Value)
                        : new CustomProperty("bruto_razviena", ""));
                    document.AddCustomProperty(new CustomProperty("parking", parcela.ParkingMesta));
                    document.AddCustomProperty(parcela.KoeficientIskoristenost != null
                        ? new CustomProperty("iskoristenost", parcela.KoeficientIskoristenostOpisno.ToString())
                        : new CustomProperty("iskoristenost", ""));
                    document.AddCustomProperty(parcela.ProcentIzgradenost != null
                        ? new CustomProperty("izgradenost", parcela.ProcentIzgradenostOpisno.ToString())
                        : new CustomProperty("izgradenost", ""));

                    //generate picture
                    var centroid = new ParceliDa().GetCentroidById(parcela.Id);
                    var resolution = GetResolutionForScale(1000);
                    var bbox = CalculateBounds(centroid.X, centroid.Y, resolution, 1800, 1800);
                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "sintezen2");

                    Image image = document.AddImage(downloadDirectory + myGuid + "sintezen2");

                    // Create a picture (A custom view of an Image).
                    Picture picture = image.CreatePicture(1000, 1000);
                    // Insert an emptyParagraph into this document.
                    Paragraph paragraph = document.Paragraphs[24];
                    //paragraph.InsertPicture(picture);
                    //paragraph.Alignment = Alignment.center;
                    // Save changes made to this document

                    foreach (var legenda in legendi.OrderBy(l => l.TipNaPodatokId))
                    {
                        switch (legenda.TipNaPodatokId)
                        {
                            case 1:
                                try
                                {
                                    document.InsertSectionPageBreak();
                                    DocX doc1 = DocX.Create(docOutputPath);
                                    doc1.PageHeight = 1191;
                                    doc1.PageWidth = 842;
                                    paragraph = doc1.InsertParagraph("", false);
                                    paragraph.Append("Синтезна - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc1.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1900, 1900, parcela.Id,
                                        "sintezen2");
                                    image = doc1.AddImage(downloadDirectory + myGuid + "sintezen2");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc1.InsertSectionPageBreak();


                                    paragraph = doc1.InsertParagraph("", false);
                                    paragraph.Append("Синтезна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc1.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc1.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc1.InsertSectionPageBreak();
                                    doc1.Save();
                                    document.InsertDocument(doc1);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 2:
                                try
                                {
                                    DocX doc2 = DocX.Create(docOutputPath);
                                    doc2.PageHeight = 1191;
                                    doc2.PageWidth = 842;
                                    paragraph = doc2.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc2.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "komunalen");
                                    image = doc2.AddImage(downloadDirectory + myGuid + "komunalen");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc2.InsertSectionPageBreak();
                                    paragraph = doc2.InsertParagraph("", false);
                                    paragraph.Append("Комунална инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc2.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc2.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc2.InsertSectionPageBreak();
                                    doc2.Save();
                                    document.InsertDocument(doc2);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 3:
                                try
                                {
                                    DocX doc3 = DocX.Create(docOutputPath);
                                    doc3.PageHeight = 1191;
                                    doc3.PageWidth = 842;
                                    paragraph = doc3.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc3.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1900, 1900, parcela.Id,
                                        "soobrakaen");
                                    image = doc3.AddImage(downloadDirectory + myGuid + "soobrakaen");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc3.InsertSectionPageBreak();

                                    paragraph = doc3.InsertParagraph("", false);
                                    paragraph.Append("Сообраќајна инфраструктура - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc3.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc3.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc3.InsertSectionPageBreak();
                                    doc3.Save();
                                    document.InsertDocument(doc3);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 4:
                                try
                                {
                                    DocX doc4 = DocX.Create(docOutputPath);
                                    doc4.PageHeight = 1191;
                                    doc4.PageWidth = 842;
                                    paragraph = doc4.InsertParagraph("", false);
                                    paragraph.Append("Споменици - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc4.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "spomenici");
                                    image = doc4.AddImage(downloadDirectory + myGuid + "spomenici");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc4.InsertSectionPageBreak();
                                    paragraph = doc4.InsertParagraph("", false);
                                    paragraph.Append("Споменици - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc4.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc4.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc4.InsertSectionPageBreak();
                                    doc4.Save();
                                    document.InsertDocument(doc4);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 5:
                                try
                                {
                                    DocX doc5 = DocX.Create(docOutputPath);
                                    doc5.PageHeight = 1191;
                                    doc5.PageWidth = 842;
                                    paragraph = doc5.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc5.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "parking");
                                    image = doc5.AddImage(downloadDirectory + myGuid + "parking");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc5.InsertSectionPageBreak();
                                    paragraph = doc5.InsertParagraph("", false);
                                    paragraph.Append("Подземен паркинг - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc5.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc5.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc5.InsertSectionPageBreak();
                                    doc5.Save();
                                    document.InsertDocument(doc5);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 6:
                                try
                                {
                                    DocX doc6 = DocX.Create(docOutputPath);
                                    doc6.PageHeight = 1191;
                                    doc6.PageWidth = 842;
                                    paragraph = doc6.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc6.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id,
                                        "infrastrukturen");
                                    image = doc6.AddImage(downloadDirectory + myGuid + "infrastrukturen");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc6.InsertSectionPageBreak();
                                    paragraph = doc6.InsertParagraph("", false);
                                    paragraph.Append("Инфраструктурен план - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc6.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc6.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc6.InsertSectionPageBreak();
                                    doc6.Save();
                                    document.InsertDocument(doc6);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                            case 7:
                                try
                                {
                                    DocX doc7 = DocX.Create(docOutputPath);
                                    doc7.PageHeight = 1191;
                                    doc7.PageWidth = 842;
                                    paragraph = doc7.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - мапа").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    paragraph = doc7.InsertParagraph("", false);
                                    DownloadWmsPlan(bbox, downloadDirectory + myGuid, 1800, 1800, parcela.Id, "zelenilo");
                                    image = doc7.AddImage(downloadDirectory + myGuid + "zelenilo");
                                    picture = image.CreatePicture(950, 950);
                                    paragraph.InsertPicture(picture);
                                    doc7.InsertSectionPageBreak();
                                    paragraph = doc7.InsertParagraph("", false);
                                    paragraph.Append("Зеленило - легенда").Bold().FontSize(14);
                                    paragraph.Alignment = Alignment.center;
                                    image =
                                        doc7.AddImage(ConfigurationManager.AppSettings["ms4w_apps_path"] +
                                                          ConfigurationManager.AppSettings["ms4w_app"] +
                                                          "\\data\\" + legenda.Path);
                                    picture = image.CreatePicture();
                                    paragraph = doc7.InsertParagraph("", false);
                                    paragraph.InsertPicture(picture);
                                    paragraph.Alignment = Alignment.center;
                                    doc7.InsertSectionPageBreak();
                                    doc7.Save();
                                    document.InsertDocument(doc7);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                                break;
                        }
                    }

                    document.Save();
                } // Release this document from memory.

                //generate Zip
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var folderName = year + "" + month;
                if (!Directory.Exists(downloadDirectory + "\\" + folderName))
                    Directory.CreateDirectory(downloadDirectory + "\\" + folderName);

                List<string> filesToAdd = new List<string>();
                filesToAdd.Add(docOutputPath);

                var usloviDirectory = HttpRuntime.AppDomainAppPath + "Uslovi\\";
                if (parcela.OpstiUsloviId.HasValue)
                {
                    var opsti = new OpstiUsloviDa().Get(parcela.OpstiUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(opsti.Path))
                        uri = usloviDirectory + opsti.Path;
                    else
                        uri = opsti.Path;
                    filesToAdd.Add(uri);
                }
                if (parcela.PosebniUsloviId.HasValue)
                {
                    var posebni = new PosebniUsloviDa().Get(parcela.PosebniUsloviId.Value);
                    //da se vidi kako e so memorijata
                    string uri;
                    if (!IdentityHelper.IsAbsoluteUrl(posebni.Path))
                        uri = usloviDirectory + posebni.Path;
                    else
                        uri = posebni.Path;
                    filesToAdd.Add(uri);
                }
                var ime = "Izvod_" + parcela.Broj.Replace('/', '_').Replace('\\', '_') + "_" + Guid.NewGuid() + ".zip";
                var fullPath = folderName + "\\" + ime;

                CreateZipFile(downloadDirectory + "\\" + fullPath, filesToAdd);
                DeleteFilesFromSystem(new List<string> { docOutputPath });

                InsertLogs(user.UserName, opfat.Ime, parcela.Broj, year + "" + month + "\\" + ime);
                return fullPath;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return "";
            }
        }



        public static bool CheckBbox(double bottom, double left, double right, double top, int idParcela)
        {
            var result = new ParceliDa().CheckBbox(bottom, left, right, top, idParcela);
            return result;
        }
        /// <summary>
        /// Vnesuva logovi za sekoe simnuvanje na izvod
        /// </summary>
        /// <param name="username">username na korisnikot</param>
        /// <param name="opfatIme">Ime na opfatot</param>
        /// <param name="brParcela">Broj na parcela</param>
        /// <param name="path">Pateka do izvodot</param>
        /// <returns>Dali e izvrseno vnesuvanjeto na logot ili ne</returns>
        public static bool InsertLogs(string username, string opfatIme, string brParcela, string path)
        {
            var vnesi = new IzvodLogsDa().Insert(username, opfatIme, brParcela, path);
            return vnesi;
        }

        public static bool InsertLogsUlici(string username, string opfatIme, string path)
        {
            var vnesi = new IzvodLogsDa().InsertLogsUlici(username, opfatIme, path);
            return vnesi;
        }
        /// <summary>
        /// Kreira zip file
        /// </summary>
        /// <param name="zipFullPath">Celosna pateka na zip file-ot kaj sto treba da se zapise</param>
        /// <param name="filesToAdd">Lista od pateki na fajlovi sto treba da se stavat</param>
        private static void CreateZipFile(string zipFullPath, List<string> filesToAdd)
        {
            using (ZipFile zip = new ZipFile())
            {
                foreach (var file in filesToAdd)
                {
                    zip.AddFile(file, "");
                }
                try
                {
                    zip.Save(zipFullPath);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// Brishe file-ovi od sistem
        /// </summary>
        /// <param name="filesToDelete">Lista na pateki do fajlovi sto treba da se brisat</param>
        private static void DeleteFilesFromSystem(List<string> filesToDelete)
        {
            foreach (var file in filesToDelete)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        /// <summary>
        /// Konvertor na rezolucija vo razmer
        /// </summary>
        /// <param name="scale">razmer</param>
        /// <returns>rezolucija</returns>
        public static double GetResolutionForScale(int scale)
        {
            var dpi = 25.4 / 0.28;
            //var mpu = ol.proj.METERS_PER_UNIT[units];
            var mpu = 1; // sega e smeneto vo 1.1 a bese mpu=1
            var inchesPerMeter = 39.37;
            return scale / (mpu * inchesPerMeter * dpi);
        }

        public static double GetResolutionForScaleUlica(int scale)
        {
            var dpi = 26.4 / 0.28;
            //var mpu = ol.proj.METERS_PER_UNIT[units];
            var mpu = 1; // sega e smeneto vo 1.1 a bese mpu=1
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
        public static Bbox CalculateBounds(double x, double y, double resolution, int w, int h)
        {
            var halfWDeg = (w * resolution) / 2;
            var halfHDeg = (h * resolution) / 2;
            var bbox = new Bbox { Left = x - halfWDeg, Bottom = y - halfHDeg, Right = x + halfWDeg, Top = y + halfHDeg };
            return bbox;
        }


        public static Bbox BoundsUlica(double x1, double y1, double x3, double y3)
        {
            var bbox = new Bbox { Left = x1, Bottom = y1 , Right = x3 , Top = y3  };
            return bbox;
        }

       

        


        /// <summary>
        /// Kreira slika od WMS servis
        /// </summary>
        /// <param name="bbox">Bounding box</param>
        /// <param name="argPathPng">celosna pateka kaj da se zapise slikata</param>
        /// <param name="width">shirina vo pikseli</param>
        /// <param name="height">visina vo pikseli</param>
        /// <param name="id">ID na gradeznata parcela</param>
        /// <param name="tip">Tip na plan (sintezen, komunalen...)</param>
        private static void DownloadWmsPlan(IBbox bbox, string argPathPng, int width, int height, int id, string tip)
        {
            var address =
                string.Format(
                    "http://{0}/cgi-bin/mapserv.exe?map=../../apps/{9}/htdocs/{8}.map&request=GetMap&service=WMS&version=1.1.1&layers={8}&styles=&srs=EPSG%3A6316&bbox={1},{2},{3},{4}&width={5}&height={6}&GID={7}&format=image%2Fpng",
                    ConfigurationManager.AppSettings["server"] + ":" + ConfigurationManager.AppSettings["port"],
                    bbox.Left.ToString(new CultureInfo("en-US")), bbox.Bottom.ToString(new CultureInfo("en-US")), bbox.Right.ToString(new CultureInfo("en-US")), bbox.Top.ToString(new CultureInfo("en-US")), width, height, id, tip,
                    ConfigurationManager.AppSettings["ms4w_app"]);
            WebClient webClient = new WebClient();
            webClient.DownloadFile(address, argPathPng + tip);

          
        }
        private static void DownloadWmsPlanUlicaSegmenti(IBbox bbox2, string argPathPng, int width, int height, List<int> id, string tip)
        {
            string segmentIds = string.Join(",", id.Select(x => x.ToString()).ToArray());
           
            var address =
                string.Format(
                    "http://{0}/cgi-bin/mapserv.exe?map=../../apps/{9}/htdocs/{8}.map&request=GetMap&service=WMS&version=1.1.1&layers={8}&styles=&srs=EPSG%3A6316&bbox={1},{2},{3},{4}&width={5}&height={6}&GID={7}&format=image%2Fpng",
                    ConfigurationManager.AppSettings["server"] + ":" + ConfigurationManager.AppSettings["port"],
                    bbox2.Left.ToString(new CultureInfo("en-US")), bbox2.Bottom.ToString(new CultureInfo("en-US")), bbox2.Right.ToString(new CultureInfo("en-US")), bbox2.Top.ToString(new CultureInfo("en-US")), width, height, segmentIds, tip,
                    ConfigurationManager.AppSettings["ms4w_app"]);
            WebClient webClient = new WebClient();
            webClient.DownloadFile(address, argPathPng + tip);


        }

        private static void DownloadWmsPlanUlica(IBbox bbox2, string argPathPng, int width, int height, int id, string tip)
        {
            var address =
                string.Format(
                    "http://{0}/cgi-bin/mapserv.exe?map=../../apps/{9}/htdocs/{8}.map&request=GetMap&service=WMS&version=1.1.1&layers={8}&styles=&srs=EPSG%3A6316&bbox={1},{2},{3},{4}&width={5}&height={6}&GID={7}&format=image%2Fpng",
                    ConfigurationManager.AppSettings["server"] + ":" + ConfigurationManager.AppSettings["port"],
                    bbox2.Left.ToString(new CultureInfo("en-US")), bbox2.Bottom.ToString(new CultureInfo("en-US")), bbox2.Right.ToString(new CultureInfo("en-US")), bbox2.Top.ToString(new CultureInfo("en-US")), width, height, id, tip,
                    ConfigurationManager.AppSettings["ms4w_app"]);
            WebClient webClient = new WebClient();
            webClient.DownloadFile(address, argPathPng + tip);


        }

    

        /// <summary>
        /// Prebaruvaj po site sloevi
        /// </summary>
        /// <param name="searchString">prebaruvan zbor</param>
        /// <returns>DrillDownInfo objekt</returns>
        public static IDrillDownInfo SearchAll(string searchString)
        {
            var info = new DrillDownInfoDa().SearchAll(searchString);
            return info;
        }

        /// <summary>
        /// Site tekovni opfati (koi ne se otideni vo istorija)
        /// </summary>
        /// <returns>Lista od opfati</returns>
        public static List<IOpfat> GetAllTekovniOpfati()
        {
            var opfati = new OpfatDa().GetAllTekovni();
            return opfati;
        }

        /// <summary>
        /// Site gradezni parceli na eden opfat
        /// </summary>
        /// <param name="opfatId">ID na Opfatot</param>
        /// <returns>Lista od gradezni parceli</returns>
        public static List<IGradParceli> GetGradezniParceli(int opfatId)
        {
            var parceli = new ParceliDa().GetByOpfat(opfatId);
            return parceli;
        }

        /// <summary>
        /// Dodadi posebni uslovi
        /// </summary>
        /// <param name="opfatId">ID na Opfatot</param>
        /// <param name="filePath">Celosna pateka do fajlot</param>
        /// <returns>uspesnost na rezultatot na zapisuvanje</returns>
        public static bool AddOpstiUslovi(int opfatId, string filePath)
        {
            var opstiUsloviId = new OpstiUsloviDa().Add(filePath);
            if (opstiUsloviId < 1) return false;
            var result = new OpstiUsloviDa().Add(opfatId, opstiUsloviId);
            return result;
        }

        /// <summary>
        /// Zemi gi opstite uslovi
        /// </summary>
        /// <param name="opfatId">ID na opfat</param>
        /// <returns>Uslov objekt</returns>
        public static IUslov GetOpstiUslovi(int opfatId)
        {
            var result = new OpstiUsloviDa().GetByOpfat(opfatId);
            return result;
        }

        /// <summary>
        /// Zemi gi posebnite uslovi
        /// </summary>
        /// <param name="id">ID na gradeznata parcela</param>
        /// <returns>Uslov objekt</returns>
        public static IUslov GetPosebniUslovi(int id)
        {
            var result = new PosebniUsloviDa().Get(id);
            return result;
        }
        public static IUslov GetNumerickiPokazateli(int id)
        {
            var result = new NumerickiPokazateliDa().GetNumericki(id);
            return result;
        }
        /// <summary>
        /// Dodadi posebni uslovi
        /// </summary>
        /// <param name="puIds">Lista od ID na gradeznite parceli</param>
        /// <param name="fileName">Celosna pateka do fajlot</param>
        /// <returns>uspesnost na rezultatot na zapisuvanje</returns>
        public static bool AddPosebniUslovi(List<int> puIds, string fileName)
        {
            var posebniUsloviId = new PosebniUsloviDa().Add(fileName);
            if (posebniUsloviId < 1) return false;
            var result = new PosebniUsloviDa().Add(puIds, posebniUsloviId);
            return result;
        }

        public static bool AddNumerickiPokazateli(List<int> npIds, string fileName)
        {
            var numerickiPokazatekiId = new NumerickiPokazateliDa().AddNumeric(fileName);
            if (numerickiPokazatekiId < 1) return false;
            var result = new NumerickiPokazateliDa().AddNumerickiPokazateli(npIds, numerickiPokazatekiId);
            return result;

        }

        /// <summary>
        /// Prebaruvanje po katastarski parceli
        /// </summary>
        /// <param name="searchString">Prebaruvan zbor</param>
        /// <returns>DrillDownInfo objekt</returns>
        public static IDrillDownInfo SearchKatastarskiParceli(string searchString)
        {
            var info = new DrillDownInfoDa().SearchKatastarskiParceli(searchString);
            return info;
        }

        /// <summary>
        /// Zemi gi site korisnici
        /// </summary>
        /// <returns>Lista od User objekti</returns>
        public static List<IUser> GetAllUsers()
        {
            var users = new UserDa().GetAll();
            return users;
        }
    
        /// <summary>
        /// Zemi gi site rolji
        /// </summary>
        /// <returns>Lista od Role objekti</returns>
        public static List<IRole> GetAllRoles()
        {
            var roles = new UserDa().GetAllRoles();
            return roles;
        }

        /// <summary>
        /// Vnesi nov korisnik
        /// </summary>
        /// <param name="roleId">ID na rolja</param>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <param name="fullName">Celosno ime</param>
        /// <param name="phone">Telefon</param>
        /// <param name="email">E-mail</param>
        /// <returns>uspesnost na rezultatot na zapisuvanje</returns>
        public static bool InsertUser(int roleId, string userName, string password, string fullName, string phone,
            string email)
        {
            var result = new UserDa().Insert(roleId, userName, password, fullName, phone, email);
            return result;
        }

        /// <summary>
        /// Promena na korisnik
        /// </summary>
        /// <param name="userId">ID na korisnik</param>
        /// <param name="userName">Korisnicko ime</param>
        /// <param name="fullName">Celosno ime</param>
        /// <param name="phone">Telefon</param>
        /// <param name="email">Email</param>
        /// <param name="active">Dali e aktiven</param>
        /// <returns>uspesnost na rezultatot na zapisuvanje</returns>
        public static bool UpdateUser(int userId, int roleId, string userName, string fullName, string phone, string email, bool active)
        {
            var result = new UserDa().Update(userId, roleId, userName, fullName, phone, email, active);
            return result;
        }

        /// <summary>
        /// Zemi korisnik
        /// </summary>
        /// <param name="userId">ID na korisnikot</param>
        /// <returns>User objekt</returns>
        public static IUser GetUser(int userId)
        {
            var user = new UserDa().Get(userId);
            return user;
        }

        /// <summary>
        /// Kreira reset token na za pronema na lozinka
        /// </summary>
        /// <param name="usermail">korisnicko ime ili lozinka</param>
        /// <returns>uspesnost na rezultatot na zapisuvanje</returns>
        public static bool CreateResetPasswordToken(string usermail)
        {
            var result = new UserDa().CreateResetPasswordToken(usermail);
            return result;
        }

        /// <summary>
        /// Resetira lozinka
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <param name="token">Token</param>
        /// <param name="password">Lozinka</param>
        /// <returns>uspesnost na rezultatot na zapisuvanje</returns>
        public static bool ResetPassword(string email, string token, string password)
        {
            var result = new UserDa().ResetPassword(email, token, password);
            return result;
        }
        /// <summary>
        /// Generiranje na lista na opfati koi ne se doneseni
        /// </summary>
        /// <returns>lista od nedoneseni opfati</returns>
        public static List<IOpfat> GetOpfatiNedoneseni()
        {
            var opfati = new OpfatDa().GetAllNedoneseni();
            return opfati;
        }
        /// <summary>
        /// Genriranje na objekt so site podatoci za daden opfat
        /// </summary>
        /// <param name="id">Id na opfatot koj go proveruvame</param>
        /// <returns>Vrakja objekt so site podatoci za opfatot</returns>
        public static IOpfat GetOpfat(int id)
        {
            var opfat = new OpfatDa().Get(id);
            return opfat;
        }
        /// <summary>
        /// Generiranje na lista od preklopi
        /// </summary>
        /// <param name="opfatId">Id na opfatot koj go prorvuvame</param>
        /// <returns>Lista na site opfati koi se preklopuvaat so dadeniot opfat</returns>
        public static List<IPreklop> GeneratePreklop(int opfatId)
        {
            var listPreklop = new List<IPreklop>();
            var noviParceli = new ParceliDa().GetByOpfat(opfatId);
            foreach (var novaParcela in noviParceli)
            {
                var preklop = new Preklop();
                var stariParceli = new ParceliDa().GeneratePreklop(novaParcela.Id);
                preklop.Nova = novaParcela;
                preklop.Stari = stariParceli;
                listPreklop.Add(preklop);
            }
            return listPreklop;
        }
        /// <summary>
        /// Generiranje na site statistiki na dadeniot opfat
        /// </summary>
        /// <param name="opfatId">Id na opfatot za koj ni se potrebni statistki</param>
        /// <returns>Objekt od statistiki za opfatot</returns>
        public static IStat GetProjectStat(int opfatId)
        {
            var stat = new StatDa().GetStat(opfatId);
            return stat;
        }
        /// <summary>
        /// Generiranje na lista na nameni za dadeniot opfat
        /// </summary>
        /// <param name="opfatId">Id na opfatot</param>
        /// <returns>Lista na site nameni za opfatot</returns>
        public static List<INamena> GetNamena(int opfatId)
        {
            var namena = new StatDa().GetListNamena(opfatId);
            return namena;
        }
        /// <summary>
        /// Generiranje na site statistiki za odredena namena 
        /// </summary>
        /// <param name="namena">Id na namena</param>
        /// <param name="opfatId">Id na opfatot</param>
        /// <returns>Objekt so statistiki za dadenata namena</returns>
        public static IStat GetNamenaStat(string namena, int opfatId)
        {
            var stat = new StatDa().GetStatNamena(namena, opfatId);
            return stat;
        }
        /// <summary>
        /// Vnesuvanje na podatoci za odobrenija vo baza za odredena parcela
        /// </summary>
        /// <param name="fkParcela">Id na parcelata</param>
        /// <param name="brPredmet">Broj na predmet</param>
        /// <param name="tipBaranje">Tip na baranje</param>
        /// <param name="sluzbenik">Ime na slusbenik</param>
        /// <param name="datumBaranja">Datum na baranjeto</param>
        /// <param name="datumIzdavanja">Datum na izdavanje</param>
        /// <param name="datumPravosilno">Datum na pravosilno</param>
        /// <param name="investitor">Ime na investitor</param>
        /// <param name="brKP">Broj na katastarska parcela</param>
        /// <param name="ko">Katastarska opstina</param>
        /// <param name="adresa">Adresa</param>
        /// <param name="parkingMestaParcela">Parking mesta za taa parcela</param>
        /// <param name="parkingMestaGaraza">Parking mesta vo garazata</param>
        /// <param name="katnaGaraza">Katna garaza</param>
        /// <param name="iznosKomunalii">Iznos na komunalii</param>
        /// <param name="zabeleski">Zabeleski</param>
        /// <param name="path">Pateka do dokumenti</param>
        /// <returns>true ako e napraveno vnesuvanjeto ili false ako ne e napraveno</returns>
        public static bool InsertOdobrenie(int fkParcela, string brPredmet, string tipBaranje, string sluzbenik, DateTime datumBaranja, DateTime datumIzdavanja, DateTime datumPravosilno, string investitor, string brKP, string ko, string adresa, string parkingMestaParcela, string parkingMestaGaraza, string katnaGaraza, double iznosKomunalii, string zabeleski, string path)
        {
            var result = new OdobrenieGradbaDa().Insert(fkParcela, brPredmet, tipBaranje, sluzbenik, datumBaranja, datumIzdavanja, datumPravosilno, investitor, brKP, ko, adresa, parkingMestaParcela, parkingMestaGaraza, katnaGaraza, iznosKomunalii, zabeleski, path);
            return result;
        }
        /// <summary>
        /// Vnes na odobrenija so podtip vo baza za odredena parcela
        /// </summary>
        /// <param name="fkParcela">Broj na parcela</param>
        /// <param name="brPredmet">Broj na predmet</param>
        /// <param name="tipBaranje">Tip na baranje</param>
        /// <param name="sluzbenik">Ime na sluzbenik</param>
        /// <param name="datumBaranja">Datum na baranjeto</param>
        /// <param name="datumIzdavanja">Datum na izdavanje</param>
        /// <param name="datumPravosilno">Datum na pravosilno</param>
        /// <param name="investitor">Ime na investitor</param>
        /// <param name="brKP">Broj na katastarska parcela</param>
        /// <param name="ko">Katastarska Opstina</param>
        /// <param name="adresa">Adresa</param>
        /// <param name="parkingMestaParcela">Parking mesta vo parcelata</param>
        /// <param name="parkingMestaGaraza">Parking mesta vo garaza</param>
        /// <param name="katnaGaraza">Katni garazi</param>
        /// <param name="iznosKomunalii">Iznos na komunalii</param>
        /// <param name="zabeleski">Zabeleski</param>
        /// <param name="path">Pateka</param>
        /// <param name="podtipBaranje">Pod tip baranje</param>
        /// <returns>true ako e napraveno vnesuvanjeto ili false ako ne e napraveno</returns>
        public static bool InsertOdobrenieSoPodtip(int fkParcela, string brPredmet, string tipBaranje, string sluzbenik, DateTime datumBaranja, DateTime datumIzdavanja, DateTime datumPravosilno, string investitor, string brKP, string ko, string adresa, string parkingMestaParcela, string parkingMestaGaraza, string katnaGaraza, double iznosKomunalii, string zabeleski, string path, string podtipBaranje)
        {
            var result = new OdobrenieGradbaDa().InsertOdobrenieSoPodtip(fkParcela, brPredmet, tipBaranje, sluzbenik, datumBaranja, datumIzdavanja, datumPravosilno, investitor, brKP, ko, adresa, parkingMestaParcela, parkingMestaGaraza, katnaGaraza, iznosKomunalii, zabeleski, path, podtipBaranje);
            return result;
        }


        /// <summary>
        /// Vnesuvanje na dokument za odredena parcela
        /// </summary>
        /// <param name="fkParcela">Id na pracelata za koja vnesuvame dokument</param>
        /// <param name="path">Pateka do dokumentot koj se vnesuva</param>
        /// <returns>true ako e napraveno vnesuvanjeto ili false ako ne e napraveno</returns>
        public static bool InsertDoc(int fkParcela, string path)
        {
            var result = new OdobrenieGradbaDa().InsertDocument(fkParcela, path);
            return result;
        }
        /// <summary>
        /// Generiranje lista na odobrenija za gradba
        /// </summary>
        /// <param name="coordinates">koordinati vo format x,y</param>
        /// <returns>Lista na odobrenija</returns>
        public static List<IOdobrenieGradba> GetInfoOdobrenija(string coordinates)
        {
            var info = new OdobrenieGradbaDa().GetOdobrenija(coordinates);
            return info;
        }
        /// <summary>
        /// Generiranje na lista na dokumenti koi se prikaceni za odredena parcela koja se naogja na koordinatite koi se izbrani
        /// </summary>
        /// <param name="coordinates">koordinati vo format x,y</param>
        /// <returns>Lista na dokumenti</returns>
        public static List<IOdobrenieGradba> GetInfoDoc(string coordinates)
        {
            var info = new OdobrenieGradbaDa().GetDocuments(coordinates);
            return info;
        }
        /// <summary>
        /// Generiranje na listaa na temi od interes
        /// </summary>
        /// <returns>Lista na temi</returns>
        public static List<ITema> GetAllTemi()
        {
            var temi = new NotifikaciiDa().GetListTemi();
            return temi;
        }
        /// <summary> 
        /// Generiranje na listaa na podtemi od interes za dadena tema
        /// </summary>
        /// <param name="temaId">Id na temata</param>
        /// <returns>Lista na podtemi</returns>
        public static List<IPodTema> GetAllPodTemi(int temaId)
        {
            var podTemi = new NotifikaciiDa().GetListPodTema(temaId);
            return podTemi;
        }
        /// <summary>
        /// Vnesuvanje na notifikacii za odredena podtema
        /// </summary>
        /// <param name="fkPodtema">Id na podteka</param>
        /// <param name="fkUser">Id na korisnikot koj ja vnesuva notifikacijata</param>
        /// <param name="komentar">Komentar</param>
        /// <param name="datumOd">Datum od koga vazi notifikacija</param>
        /// <param name="datumDo">Datum do koga ke vazi notifikacijata</param>
        /// <param name="coordinate">koordinati vo format x,y</param>
        /// <returns>true ako e napraveno vnesuvanjeto ili false ako ne e napraveno</returns>
        public static bool InsertNotifikacija(int fkPodtema, int fkUser, string komentar, DateTime datumOd, DateTime datumDo, string coordinate)
        {
            var result = new NotifikaciiDa().Insert(fkPodtema, fkUser, komentar, datumOd, datumDo, coordinate);
            return result;
        }
        /// <summary>
        /// Generiranje na lista na notifikacii
        /// </summary>
        /// <returns>Lista na site notifikacii</returns>
        public static List<INotifikacii> GetListIzvestuvanje()
        {
            var izvestuvanja = new NotifikaciiDa().GetAllNotifications();
            return izvestuvanja;
        }
        /// <summary>
        /// Brisenje na notifikacija od baza
        /// </summary>
        /// <param name="id">Id na notifikacijata</param>
        /// <returns>true ako e napraveno ili false ako ne e napraveno</returns>
        public static bool DeleteNotification(int id)
        {
            var result = new NotifikaciiDa().Delete(id);
            return result;
        }
        public static bool DeleteDocument(int id)
        {
            var result = new LegalizacijaDal().DeleteDocument(id);
            return result;
        }
        /// <summary>
        /// Brisenje na predmet
        /// </summary>
        /// <param name="id">Id na predmet</param>
        /// <returns>true ako e napraveno ili false ako ne e napraveno</returns>
        public static bool DeletePredmet(int id)
        {
            var result = new OdobrenieGradbaDa().Delete(id);
            return result;
        }
        /// <summary>
        /// Generiranje na pateka do dokumentot za opsti uslovi
        /// </summary>
        /// <param name="opstiId">Id na opsti uslovi</param>
        /// <returns>Pateka do dokumentot</returns>
        public static string GenerateOpsti(int opstiId)
        {
            var path = new ParceliDa().GetOpsti(opstiId);
            return path;
        }
        /// <summary>
        /// Generiranje na pateka do dokumentot za posebni uslovi
        /// </summary>
        /// <param name="posebniId">Id na posebni uslovi</param>
        /// <returns>Pateka do dokumentot</returns>
        public static string GeneratePosebni(int posebniId)
        {
            var path = new ParceliDa().GetPosebni(posebniId);
            return path;
        }

        public static string GenerateNumericki(int numerickiId)
        {
            var path = new ParceliDa().GetNumericki(numerickiId);
            return path;
        }
        public static string GenerateTehnickiIspravki(int tehnickiIspravkiId)
        {
            var path = new ParceliDa().GenerateTehnickiIspravki(tehnickiIspravkiId);
            return path;
        }
        /// <summary>
        /// Generiranje na lista na site ulici
        /// </summary>
        /// <returns>Lista so site ulici</returns>
        public static List<IStreet> GetAllStreets()
        {
            var streets = new AdresiDa().GetListStreet();
            return streets;
        }

        //public static List<ITreeShrubSeason> GetTreeSeason()
        //{
        //    var seasontree = new TreeShrubDa().GetListSeasonTree();
        //    return seasontree;
        //}

        public static List<ITreeShrubType> GetDrvoGrmushka(string search = null)
        {
            var treeshrub = new TreeShrubDa().GetDrvoGrmushka(search);
            return treeshrub;
        }

        public static List<IZelenilo> GetStreetsPolygons(string search = null)
        {
            var streets = new TreeShrubDa().GetStreets(search);
            return streets;
        }

       
        public static List<ITreeShrubTopology> GetTreeShrubName(int? type = null,int? id=null, string search = null)
        {
            var treeshrubname = new TreeShrubDa().GetTreeShrubName(type,id, search);
            return treeshrubname;
        }


        public static List<ITreeShrubSeason> GetSeason(int? type = null, string search = null)
        {
            var partner = new TreeShrubDa().GetSeason(type, search);
            return partner;
        }
        /// <summary>
        /// Generiranje na lista na site ulici
        /// </summary>
        /// <returns>Lista so site ulici</returns>
        public static List<IStreet> GetAllVStreets()
        {
            var streets = new AdresiDa().GetListVStreet();
            return streets;
        }

        public static List<IStreet> GetCentarStreets()
        {
            var streets = new AdresiDa().GetCentarStreet();
            return streets;
        }
        /// <summary>
        /// Generiranje na lista na site ulici
        /// </summary>
        /// <returns>Lista so site ulici</returns>
        public static List<IStreet> GetStreets()
        {
            var streets = new AdresiDa().GetAllStreet();
            return streets;
        }
        public static List<IOpfat2> GetAllOpfati()
        {
            var opfati = new OpfatDa().GetListOpfat();
            return opfati;
        }
        /// <summary>
        /// Generiranje na lista na site broevi za sekoja ulica
        /// </summary>
        /// <param name="ulica">Ime na ulica</param>
        /// <returns>Lista na broevi</returns>
        public static List<IStNumber> GetAllNumbers(string ulica)
        {
            var numbers = new AdresiDa().GetListNumbers(ulica);
            return numbers;
        }
        /// <summary>
        /// Generiranje na lista na site broevi za sekoja ulica
        /// </summary>
        /// <param name="ulica">Ime na ulica</param>
        /// <returns>Lista na broevi</returns>
        public static List<IStNumber> GetAllVNumbers(string ulica)
        {
            var numbers = new AdresiDa().GetListVNumbers(ulica);
            return numbers;
        }
        /// <summary>
        /// Generiranje na lista na site broevi za sekoja ulica
        /// </summary>
        /// <param name="ulica">Ime na ulica</param>
        /// <returns>Lista na broevi</returns>
        public static List<IStNumber> GetAllStreetNumbers(string ulica)
        {
            var numbers = new AdresiDa().GetListStreetNumbers(ulica);
            return numbers;
        }
        public static List<IStNumber> GetAllCStreetNumbers(string ulica)
        {
            var numbers = new AdresiDa().GetListCStreetNumbers(ulica);
            return numbers;
        }
        /// <summary>
        /// Generiranje na lista na gradezni parceli
        /// </summary>
        /// <param name="opfat">Ime na opfat</param>
        /// <returns>Lista na gradezni parceli</returns>
        public static List<IGradParceli2> GetAllGParceli(string opfat)
        {
            var parceli = new ParceliDa().GetListGParceli(opfat);
            return parceli;
        }

        //public static List<ITreeShrubTopology> GetTreesByName(int treeseasonid)
        //{
        //    var treesName = new TreeShrubDa().GetTreesByName(treeseasonid);
        //    return treesName;
        //}

        /// <summary>
        /// Prebaruvanje po adresa
        /// </summary>
        /// <param name="ulica">Ime na ulica</param>
        /// <param name="broj">Broj na ulica</param>
        /// <returns>Adresa objekt</returns>
        public static IAdresi SearchAdresa(string ulica, string broj)
        {
            var info = new AdresiDa().SearchAdress(ulica, broj);
            return info;
        }

        public static List<ITreeShrubInventory> SearchTreeShrub(int treeShrub , int season , string name )
        {
            var info = new TreeShrubDa().SearchTreeShrub(treeShrub, season, name);
            return info;
        }
        /// <summary>
        /// Prebaruvanje po adresa
        /// </summary>
        /// <param name="ulica">Ime na ulica</param>
        /// <param name="broj">Broj na ulica</param>
        /// <returns>Adresa objekt</returns>
        public static IAdresi SearchVAdresa(string ulica, string broj)
        {
            var info = new AdresiDa().SearchVAdress(ulica, broj);
            return info;
        }
        /// <summary>
        /// Prebaruvanje po adresa
        /// </summary>
        /// <param name="ulica">Ime na ulica</param>
        /// <param name="broj">Broj na ulica</param>
        /// <returns>Adresa objekt</returns>
        public static IAdresi SearchAdress(string ulica, string broj)
        {
            var info = new AdresiDa().SearchAdressPoint(ulica, broj);
            return info;
        }

        public static IAdresi SearchCAdress(string ulica, string broj)
        {
            var info = new AdresiDa().SearchCAdress(ulica, broj);
            return info;
        }

        public static List<IStreet> GetAdresi(string poligon)
        {
            var info = new AdresiDa().GetAdresi(poligon);
            return info;
        }

        /// <summary>
        /// Prebaruvanje na gradezna parcela 
        /// </summary>
        /// <param name="id">id na parcela</param>
        /// <returns>Geografija na parcelata</returns>
        public static IGradezniParceli SearchParcela(int id)
        {
            var info = new ParceliDa().SearchParcela(id);
            return info;
        }
        /// <summary>
        /// Generiranje na dxf so okolnite objekti
        /// </summary>
        /// <param name="coordinates">koordinati vo format x,y</param>
        /// <returns>pateka do dxf</returns>
        public static string GenerateDxfNearby(string coordinates)
        {
            string dbName = ConfigurationManager.AppSettings["dbName"];
            string dbAddress = ConfigurationManager.AppSettings["dbServer"];
            string dbPassword = ConfigurationManager.AppSettings["dbPass"];
            string dbPort = ConfigurationManager.AppSettings["dbPort"];

            var downloadDirectory = HttpRuntime.AppDomainAppPath + "Dxf\\";
            if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
            var myGuid = Guid.NewGuid();
            var argPathDxf = string.Format("Dxf_{0}.dxf", myGuid);
            var cmd =
                string.Format(
                    "-f DXF \"{0}\" PG:\"dbname='{1}' host='{2}' port='{3}' user='postgres' password='{4}'\" -nlt LINESTRING -sql \"select '' as layer, query1.mikro_povrsina_za_gradba_id as id, query1.geom from mikro_povrsini_za_gradba as query1, (select distinct b.geom from mikro_gradezni_parceli a inner join mikro_gradezni_parceli as b ON a.mikro_gradezna_parcela_id!=b.mikro_gradezna_parcela_id where (ST_Touches(st_makevalid(a.geom), st_makevalid(b.geom)) or ST_Intersects(st_makevalid(a.geom), st_makevalid(b.geom))) and st_isvalid(a.geom)=true and st_isvalid(b.geom)=true and a.active=true and a.valid_to='infinity' and a.produkcija=true and b.active=true and b.valid_to='infinity' and b.produkcija=true and a.mikro_gradezna_parcela_id=(SELECT mikro_gradezna_parcela_id FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and st_isvalid(geom)=true and active=true and valid_to='infinity' and produkcija=true)  union SELECT  geom FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and active=true and valid_to='infinity' and produkcija=true ) as query2 WHERE ST_Contains(ST_Buffer(query2.geom, 1), query1.geom) and query1.active=true union select distinct b.broj as layer, 0 as id, b.geom from mikro_gradezni_parceli a inner join mikro_gradezni_parceli as b ON a.mikro_gradezna_parcela_id!=b.mikro_gradezna_parcela_id where (ST_Touches(st_makevalid(a.geom), st_makevalid(b.geom)) or ST_Intersects(st_makevalid(a.geom), st_makevalid(b.geom))) and st_isvalid(a.geom)=true and st_isvalid(b.geom)=true and a.active=true and a.valid_to='infinity' and a.produkcija=true and b.active=true and b.valid_to='infinity' and b.produkcija=true and a.mikro_gradezna_parcela_id=(SELECT mikro_gradezna_parcela_id FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and st_isvalid(geom)=true and active=true and valid_to='infinity' and produkcija=true) union SELECT broj as layer,0 as id, geom FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and active=true and valid_to='infinity' and produkcija=true \"",
                    downloadDirectory + argPathDxf, dbName, dbAddress, dbPort, dbPassword, coordinates);


            //generate DXF

            var fullPath = ConfigurationManager.AppSettings["ogrPath"];
            ProcessStartInfo start = new ProcessStartInfo(fullPath);
            start.Arguments = cmd;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string result;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();
                }
            }
            return argPathDxf;
        }

        public static string GenerateDxfNearbyKumanovo(string coordinates)
        {


            string dbName = ConfigurationManager.AppSettings["dbName"];
            string dbAddress = ConfigurationManager.AppSettings["dbServer"];
            string dbPassword = ConfigurationManager.AppSettings["dbPass"];
            string dbPort = ConfigurationManager.AppSettings["dbPort"];

            var downloadDirectory = HttpRuntime.AppDomainAppPath + "Dxf\\";
            if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
            var myGuid = Guid.NewGuid();
            var argPathDxf = string.Format("Dxf_{0}.dxf", myGuid);
            var cmd =
                string.Format(
                    "-f DXF \"{0}\" PG:\"dbname='{1}' host='{2}' port='{3}' user='postgres' password='{4}'\" -nlt LINESTRING -sql \"select '' as layer, query1.mikro_povrsina_za_gradba_id as id, query1.geom from mikro_povrsini_za_gradba as query1, (select distinct b.geom from mikro_gradezni_parceli a inner join mikro_gradezni_parceli as b ON a.mikro_gradezna_parcela_id!=b.mikro_gradezna_parcela_id where (ST_Touches(st_makevalid(a.geom), st_makevalid(b.geom)) or ST_Intersects(st_makevalid(a.geom), st_makevalid(b.geom))) and st_isvalid(a.geom)=true and st_isvalid(b.geom)=true and a.active=true and a.valid_to='infinity' and a.produkcija=true and b.active=true and b.valid_to='infinity' and b.produkcija=true and a.mikro_gradezna_parcela_id=(SELECT mikro_gradezna_parcela_id FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and st_isvalid(geom)=true and active=true and valid_to='infinity' and produkcija=true)  union SELECT  geom FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and active=true and valid_to='infinity' and produkcija=true ) as query2 WHERE ST_Contains(ST_Buffer(query2.geom, 1), query1.geom) and query1.active=true union select distinct b.broj as layer, 0 as id, b.geom from mikro_gradezni_parceli a inner join mikro_gradezni_parceli as b ON a.mikro_gradezna_parcela_id!=b.mikro_gradezna_parcela_id where (ST_Touches(st_makevalid(a.geom), st_makevalid(b.geom)) or ST_Intersects(st_makevalid(a.geom), st_makevalid(b.geom))) and st_isvalid(a.geom)=true and st_isvalid(b.geom)=true and a.active=true and a.valid_to='infinity' and a.produkcija=true and b.active=true and b.valid_to='infinity' and b.produkcija=true and a.mikro_gradezna_parcela_id=(SELECT mikro_gradezna_parcela_id FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and st_isvalid(geom)=true and active=true and valid_to='infinity' and produkcija=true) union SELECT broj as layer,0 as id, geom FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and active=true and valid_to='infinity' and produkcija=true \"",
                    downloadDirectory + argPathDxf, dbName, dbAddress, dbPort, dbPassword, coordinates);


            //generate DXF

            var fullPath = ConfigurationManager.AppSettings["ogrPath"];
            ProcessStartInfo start = new ProcessStartInfo(fullPath);
            start.Arguments = cmd;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string result;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();
                }
            }
            return argPathDxf;
        }
        /// <summary>
        /// Vnesuvanje na dokument vo baza
        /// </summary>
        /// <param name="fkParcela">Id na parcela</param>
        /// <param name="path">Pateka do dokuemntot</param>
        /// <returns>true ako e uspesnp ili false ako ne e uspesno</returns>
        public static bool InsertGeneralDoc(int fkParcela, string path)
        {
            var result = new GeneralDocDa().InsertGeneralDocument(fkParcela, path);
            return result;
        }
        /// <summary>
        /// Generiranje na lista na dokumenti 
        /// </summary>
        /// <param name="coordinates">koordinati vo format x,y</param>
        /// <returns>Lista na dokumenti</returns>
        public static List<IGeneralDoc> GetGeneralDoc(string coordinates)
        {
            var info = new GeneralDocDa().GetGeneralDocuments(coordinates);
            return info;
        }
        /// <summary>
        /// Vnesuvanje ime na investitor vo baza
        /// </summary>
        /// <param name="fkParcela">Id na parcela</param>
        /// <param name="investitor">Ime na investitor</param>
        /// <returns>true ako e uspesnp ili false ako ne e uspesno</returns>
        public static bool InsertInestitor(int fkParcela, string investitor)
        {
            var result = new ParceliDa().InsertImeInvestitor(fkParcela, investitor);
            return result;
        }
        /// <summary>
        /// Dobivanje na lista, logovi na aktivnosti
        /// </summary>
        /// <returns>Lista od logovoi</returns>
        public static List<IIzvodLogs> GetAllLogs()
        {
            var logs = new IzvodLogsDa().GetAll();
            return logs;
        }
        /// <summary>
        /// Generiranje lista na site katni garazi
        /// </summary>
        /// <returns>Lista na katni garazi</returns>
        public static List<IKatniGarazi> GetAllKatniGarazi()
        {
            var garazi = new OdobrenieGradbaDa().GetAllKatniGarazi();
            return garazi;
        }
        /// <summary>
        /// Generiranje lista na site baranja
        /// </summary>
        /// <returns>Lista na site baranja</returns>
        public static List<ITipBaranje> GetAllTipBaranja()
        {
            var baranja = new OdobrenieGradbaDa().GetAllTipBaranja();
            return baranja;
        }
        /// <summary>
        /// Generiranje lista na site podtipovi na baranja
        /// </summary>
        /// <returns>Lista na site podtipovi na baranja</returns>
        public static List<IPodTipBaranje> GetAllPodTipBaranja()
        {
            var baranja = new OdobrenieGradbaDa().GetAllPodTipBaranja();
            return baranja;
        }

        public static bool InsertDoc(string path, string filename,int legalizacija_id)
        {

            var result = new LegalizacijaDal().InsertDoc(path, filename, legalizacija_id);
            return result;
        }

        public static List<ILegalizacija> GetInfoLegalizacija(string coordinates)
        {
            var info = new LegalizacijaDal().GetLegalizacija(coordinates);
            return info;
        }

       
        public static List<ILegalizacija> Count()
        {
            var info = new LegalizacijaDal().Count();
            return info;
        }

        public static int InsertLegalizacija(string katastarskaOpstina, string katastarskaParcela, string broj, string namenaobjekt, string tipLegalizacija, string polygon, int? brojObjekt)
        {

            var result = new LegalizacijaDal().InsertLegalizacija(katastarskaOpstina, katastarskaParcela, broj, namenaobjekt, tipLegalizacija, polygon, brojObjekt);
            return result;

        }
       
        public static bool UpdateStatusGradba(int gradbaId)
        {

            var result = new LegalizacijaDal().UpdateStatusGradba(gradbaId);
            return result;

        }    

        public static List<ILegalizacija> SearchKatParceliLegalizacija(string searchString)
        {
            var info = new LegalizacijaDal().GetKatParceliLegalizacija(searchString);
            return info;
        }
        public static List<IZelenilo> SearchUliciZelenilo(string searchString)
        {
            var info = new TreeShrubDa().GetUlica(searchString);
            return info;
        }
        public static List<ILegalizacija> SearchBrPredmetLegalizacija(string searchString)
        {
            var info = new LegalizacijaDal().GetBrPremetLegalizacija(searchString);
            return info;
        }
        public static List<ITema> GetAllMunicipalities()
        {
            var temi = new LegalizacijaDal().GetListMunicipalities();
            return temi;
        }
        public static List<ITema> GetNamena()
        {
            var temi = new LegalizacijaDal().GetListNamena();
            return temi;
        }

        public static bool DeleteObjekt(int id)
        {
            var result = new LegalizacijaDal().Delete(id);
            return result;
        }

        public static List<ILegalizacija> ListDokumenti(int id)
        {
            var listadokumenti = new LegalizacijaDal().ListDokumenti(id);
            return listadokumenti;
        }

        //public static List<IDrva> GetAllTrees()
        //{
        //    var institucii = new InstituciiDa().GetListInstitucii();
        //    return institucii;
        //}


        public static ITreeShrubInventory GetTreeShrubCount(int? treeShrub = null)
        {
            var count = new TreeShrubDa().GetTreeShrubCount(treeShrub);
            return count;
        }

    }
}