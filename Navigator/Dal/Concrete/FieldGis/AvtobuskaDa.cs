using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Models.Concrete.FileldGis;
using NLog;

namespace Navigator.Dal.Concrete.FieldGis
{
    public class AvtobuskaDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<AvtobuskaStanica> GenerateList(string coordinates)
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
                    "from avtobuska_stanica " +
                    " where ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),ST_Buffer(geom,5));";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<AvtobuskaStanica> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private AvtobuskaStanica CreateObject(DataRow dr)
        {
            var item = new AvtobuskaStanica
            {
                Id = int.Parse(dr["avtobuska_stanica_id"].ToString()),
                Ime = dr["ime"].ToString()
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