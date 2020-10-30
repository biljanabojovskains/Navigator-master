using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Models.Concrete.FileldGis;
using NLog;

namespace Navigator.Dal.Concrete.FieldGis
{
    public class JavnaPovrsinaDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<JavnaPovrsina> GenerateList(string coordinates)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand("FieldGisConnection");
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "select * " +
                    "from javna_povrsina d left join tip_javna_povrsina s on d.tip_javna_povrsina_id_fk = s.tip_javna_povrsina_id " +
                    " where ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),ST_Buffer(d.geom,5));";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<JavnaPovrsina> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private JavnaPovrsina CreateObject(DataRow dr)
        {
            var item = new JavnaPovrsina
            {
                Id = int.Parse(dr["javna_povrsina_id"].ToString()),
                Komentar = dr["komentar"].ToString(),
                Tip = dr["ime"].ToString()
            };
            //try
            //{
            //    item.GeoJson = dr["geojson"].ToString();
            //}
            //catch
            //{
            //    // ignored
            //}
            return item;
        }
    }
}