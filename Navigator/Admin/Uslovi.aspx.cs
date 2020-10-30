using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Navigator.Bll;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;

namespace Navigator.Admin
{
    public partial class Uslovi : Page
    {
     
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var osnoven = new Opfat
                {
                    Id = 0,
                    Ime = "Изберете опфат"
                };
                var tekovniOpfati = Bl.GetAllTekovniOpfati();
                tekovniOpfati.Add(osnoven);
                DdlProekti.DataSource = tekovniOpfati.OrderBy(o => o.Id);
                DdlProekti.DataBind();
            }
            ListPosebniUslovi.ItemDataBound += ListPosebniUslovi2_ItemDataBound;
            ListPosebniUslovi.ItemDataBound += ListNumerickiPokazateli_ItemDataBound;
            
            
            if(IsPostBack)
            {
                var uslov = Bl.GetOpstiUslovi(int.Parse(DdlProekti.SelectedItem.Value));
                if (uslov != null)
                {
                    BtnOpstiPrevzemi.Visible = true;
                }
                else
                {
                    BtnOpstiPrevzemi.Visible = false;
                }
            }
                       
        }

        protected void BtnOpsti_OnClick(object sender, EventArgs e)
        {
            string ext = Path.GetExtension(FuOpsti.PostedFile.FileName).ToLower();
            if (FuOpsti.HasFile && FuOpsti.PostedFile.ContentLength > 0)
            {
                var outFileName = "OpstiUslovi_" + Guid.NewGuid();
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var foldername = year + "" + month;

                var uploadFolder = HttpRuntime.AppDomainAppPath + "Uslovi\\" + foldername;
                if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                var fileName = uploadFolder + "\\" + outFileName + ext;
                FuOpsti.SaveAs(fileName);
                Bl.AddOpstiUslovi(int.Parse(DdlProekti.SelectedItem.Value), fileName);
                BtnOpstiPrevzemi.Visible = true;
            }
            
        }

        protected void BtnOpstiPrevzemi_OnClick(object sender, EventArgs e)
        {
            var uslov = Bl.GetOpstiUslovi(int.Parse(DdlProekti.SelectedItem.Value));
            if (uslov != null) DownloadFile(uslov.Path);
        }
        
        public void DownloadFile(string path)
        {
            string ext = Path.GetExtension(path);
            if (ext == "") return;

            Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            Response.AppendHeader("Content-Disposition",
                string.Format("attachment; filename={0}", Path.GetFileName(path)));
            Response.TransmitFile(path);
            Response.End();
        }

        protected void DdlProekti_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (DdlProekti.SelectedItem.Value == "0")
            {
                PnlUslovi.Visible = false;
            }
            else
            {
                PnlUslovi.Visible = true;
                var gradezniParceli = Bl.GetGradezniParceli(int.Parse(DdlProekti.SelectedItem.Value));
                if (gradezniParceli == null || gradezniParceli.Count == 0)
                {
                    PnlUslovi.Visible = false;
                }
                else
                {
                    FillPosebniUslovi(gradezniParceli);
                  
                }
            }
        }

        private void FillPosebniUslovi(List<IGradParceli> list)
        {
            if (list == null) return;
            ListPosebniUslovi.DataSource = list.OrderBy(p=>p.Broj).ToList();
            ViewState["ListPosebniUslovi"] = list.OrderBy(p => p.Broj).ToList();
            ListPosebniUslovi.DataBind();

            var listKatnost = list.Select(l => l.Katnost).Distinct().OrderBy(p => p).ToList();
            listKatnost.Insert(0, "Сите");
            DropDownList ddlKatnost = (DropDownList) (ListPosebniUslovi.FindControl("DdlKatnost"));
            ddlKatnost.DataSource = listKatnost;
            ddlKatnost.DataBind();

            var listKlasa = list.Select(l => l.KlasaNamena).Distinct().OrderBy(p => p).ToList();
            listKlasa.Insert(0, "Сите");
            DropDownList ddlKlasa = (DropDownList) (ListPosebniUslovi.FindControl("DdlKlasa"));
            ddlKlasa.DataSource = listKlasa;
            ddlKlasa.DataBind();
        }

    

        private void ListPosebniUslovi2_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType != ListViewItemType.DataItem) return;
            var item = (IGradParceli) e.Item.DataItem;
            Button btnPrevzemi = (Button) e.Item.FindControl("BtnPosebniPrevzemi");
            btnPrevzemi.Visible = item.PosebniUsloviId != null;
            var sm = ScriptManager.GetCurrent(this);
            sm.RegisterPostBackControl(btnPrevzemi);
        }

        private void ListNumerickiPokazateli_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType != ListViewItemType.DataItem) return;
            var item = (IGradParceli)e.Item.DataItem;
            Button btnPrevzemi = (Button)e.Item.FindControl("BtnNumerickiPrevzemi");
            btnPrevzemi.Visible = item.NumerickiPokazateliId != null;
            var sm = ScriptManager.GetCurrent(this);
            sm.RegisterPostBackControl(btnPrevzemi);
        }

        protected void DdlKlasaKatnost_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlKlasa = (DropDownList) ListPosebniUslovi.FindControl("DdlKlasa");
            DropDownList ddlKatnost = (DropDownList) ListPosebniUslovi.FindControl("DdlKatnost");
            string selKlasa = ddlKlasa.SelectedValue;
            string selKatnost = ddlKatnost.SelectedValue;

            var listParceli = (List<IGradParceli>) ViewState["ListPosebniUslovi"];
            IQueryable<IGradParceli> listParceliNovi = null;
            if (selKlasa != "Сите" && selKatnost != "Сите")
            {
                listParceliNovi =
                    listParceli.Where(r => (r.KlasaNamena == selKlasa) && (r.Katnost == selKatnost)).AsQueryable();
            }
            if (selKlasa != "Сите" && selKatnost == "Сите")
            {
                listParceliNovi = listParceli.Where(r => r.KlasaNamena == selKlasa).AsQueryable();
            }
            if (selKlasa == "Сите" && selKatnost != "Сите")
            {
                listParceliNovi =
                    listParceli.Where(r => r.Katnost == selKatnost).AsQueryable();
            }
            if (selKlasa == "Сите" && selKatnost == "Сите")
            {
                listParceliNovi = listParceli.AsQueryable();
            }
            if (listParceliNovi != null && listParceliNovi.Any())
                ListPosebniUslovi.DataSource = listParceliNovi;
            else
            {
                IGradParceli item = new GradParceli();
                item.Broj = "Нема податоци";
                var tmpList = new List<IGradParceli> {item};
                ListPosebniUslovi.DataSource = tmpList;
            }
            ListPosebniUslovi.DataBind();
        }

        protected void BtnPosebniPrevzemi_OnCommand(object sender, CommandEventArgs e)
        {
            var uslov = Bl.GetPosebniUslovi(int.Parse(e.CommandArgument.ToString()));
            if (uslov != null) DownloadFile(uslov.Path);
        }

        protected void BtnNumerickiPrevzemi_OnCommand(object sender, CommandEventArgs e)
        {
            var uslov = Bl.GetNumerickiPokazateli(int.Parse(e.CommandArgument.ToString()));
            if (uslov != null) DownloadFile(uslov.Path);
        }

        protected void BtnPosebni_OnClick(object sender, EventArgs e)
        {
            List<int> puIds = new List<int>();
            foreach (var item in ListPosebniUslovi.Items)
            {
                var cb = (CheckBox)item.FindControl("CbAdd");
                if (!cb.Checked) continue;
                var key = ListPosebniUslovi.DataKeys[item.DataItemIndex];
                if (key != null && !key.Value.ToString().Equals("0"))
                    puIds.Add(int.Parse(key.Value.ToString()));
            }

            string ext = Path.GetExtension(FuPosebni.PostedFile.FileName).ToLower();
            if (FuPosebni.HasFile && FuPosebni.PostedFile.ContentLength > 0)
            {
                var outFileName = "PosebniUsovi_" + Guid.NewGuid();
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var foldername = year + "" + month;



                var uploadFolder = HttpRuntime.AppDomainAppPath + "Uslovi\\" + foldername;
                if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                var fileName = uploadFolder + "\\" + outFileName + ext;
                FuPosebni.SaveAs(fileName);

                Bl.AddPosebniUslovi(puIds, fileName);
            }
        }

        protected void BtnNumericki_OnClick(object sender, EventArgs e)
        {
            List<int> npIds = new List<int>();
            foreach (var item in ListPosebniUslovi.Items)
            {
                var cb = (CheckBox)item.FindControl("CbAdd");
                if (!cb.Checked) continue;
                var key = ListPosebniUslovi.DataKeys[item.DataItemIndex];
                if (key != null && !key.Value.ToString().Equals("0"))
                    npIds.Add(int.Parse(key.Value.ToString()));
            }
            string ext = Path.GetExtension(FuNumericki.PostedFile.FileName).ToLower();
            if (FuNumericki.HasFile && FuNumericki.PostedFile.ContentLength > 0)
            {
                var outFileName = "NumerickiPokazateli_" + Guid.NewGuid();
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var foldername = year + "" + month;



                var uploadFolder = HttpRuntime.AppDomainAppPath + "Uslovi\\" + foldername;
                if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                var fileName = uploadFolder + "\\" + outFileName + ext;
                FuNumericki.SaveAs(fileName);

                Bl.AddNumerickiPokazateli(npIds, fileName);
            }
         
        }
    }
}