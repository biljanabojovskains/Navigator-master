using System;
using System.Web.Services;
using System.Web.UI;
using Navigator.Bll;
using Newtonsoft.Json;

namespace Navigator.Modules.Notifikacii
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        [WebMethod]
        public static string GetList()
        {
            var item = Bl.GetListIzvestuvanje();
            var result = JsonConvert.SerializeObject(item);
            return result;
        }
        [WebMethod]
        public static void Izbrisi(int id)
        {
            var item = Bl.DeleteNotification(id);
        }
    }
}