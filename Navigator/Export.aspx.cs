using Navigator.Bll;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Navigator
{
    public partial class Export : System.Web.UI.Page
    {
        public int tip = 0;

        public string result;
        public int broj;
        public int zimzeleni;
        public int listopadni;
        public int bolni;
        public int zdravi;
        public string tipIme;

        public DateTime dateTime;

       // public string codeAttributeTypeId = ConfigurationManager.AppSettings["CodeSvfAttributeTypeId"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params.Count > 0)
            {
                tipIme = tip == 1 ? "дрва" : "грмушки";
                tip = int.Parse(Request.Params["tip"]);
                var item = Bl.GetTreeShrubCount(tip);
                result = JsonConvert.SerializeObject(item);
                broj = item.CountTreeShrub;
                zimzeleni = item.CountZimzeleni;
                listopadni = item.CountListopadni;
                bolni = item.CountBolni;
                zdravi = item.CountZdravi;
                dateTime = DateTime.Now;
                
            }
        }

        }
    }
