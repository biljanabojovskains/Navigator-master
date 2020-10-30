using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Models.Concrete.FileldGis;
using NLog;

namespace Navigator.Dal.Concrete.FieldGis
{
    public class DefektDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<Defekt> GenerateList(string coordinates)
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
                    "from defekt d left join sif_defekt s on d.sif_defekt_id_fk = s.sif_defekt_id " +
                    " where ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),ST_Buffer(d.geom,5));";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<Defekt> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private Defekt CreateObject(DataRow dr)
        {
            var item = new Defekt
            {
                Id = int.Parse(dr["defekt_id"].ToString()),
                Text = dr["tekst"].ToString(),
                Slika = dr["slika"].ToString(),
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