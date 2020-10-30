using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Navigator
{
    public class GetFile : IHttpHandler
    {

      public void ProcessRequest(HttpContext context)
        {   
            string imgName = context.Request.QueryString["img"];
            string folderName = context.Request.QueryString["folder"];
            var ext = Path.GetExtension(imgName).ToUpper();
            context.Response.Clear();
            if (ext == ".PNG")
            {
                context.Response.AddHeader("content-disposition", "attachment;filename=" + imgName);
                context.Response.ContentType = "image/png";

                var img = Image.FromFile(context.Server.MapPath("~/" + folderName + "/" + imgName));
                img.Save(context.Response.OutputStream, ImageFormat.Png);
            }
            else if (ext == ".JPG")
            {
                context.Response.AddHeader("content-disposition", "attachment;filename=" + imgName);
                context.Response.ContentType = "image/jpeg";

                var img = Image.FromFile(context.Server.MapPath("~/" + folderName + "/" + imgName));
                img.Save(context.Response.OutputStream, ImageFormat.Jpeg);
            }
            else if (ext == ".PDF")
            {
                context.Response.AddHeader("content-disposition", "attachment;filename=" + imgName);
                context.Response.ContentType = "application/pdf";
                context.Response.WriteFile(context.Server.MapPath("~/" + folderName + "/" + imgName));
            }
            else if (ext == ".DXF")
            {
                context.Response.AddHeader("content-disposition", "attachment;filename=" + imgName);
                context.Response.ContentType = "application/dxf";
                context.Response.WriteFile(context.Server.MapPath("~/" + folderName + "/" + imgName));
            }
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}
