using System;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using Newtonsoft.Json;
using PublicNavigator.Bll;
using PublicNavigator.Models.Concrete;

namespace PublicNavigator.Modules.Notifikacii
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static string GetList()
        {

            var id = (FormsIdentity)HttpContext.Current.User.Identity;
            var user = JsonConvert.DeserializeObject<User>(id.Ticket.UserData);
            var item = Bl.GetPoiList(user.UserId);
            var result = JsonConvert.SerializeObject(item);
            return result;
        }

        [WebMethod]
        public static void Izbrisi(int id)
        {
            var item = Bl.DeletePoi(id);
        }
    }
}