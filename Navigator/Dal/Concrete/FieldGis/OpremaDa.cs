using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Models.Concrete.FileldGis;
using NLog;

namespace Navigator.Dal.Concrete.FieldGis
{
    public class OpremaDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<Oprema> GenerateList(string coordinates)
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
                    "select o.oprema_id, s.ime as s_ime, tipo.ime as tipo_ime " +
                    "from oprema o left join sif_sostojba s on o.sif_sostojba_id_fk = s.sif_sostojba_id " +
                    "left join tip_oprema tipo on o.tip_oprema_id_fk = tipo.tip_oprema_id " +
                    " where ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),ST_Buffer(o.geom,5));";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<Oprema> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private Oprema CreateObject(DataRow dr)
        {
            var item = new Oprema
            {
                Id = int.Parse(dr["oprema_id"].ToString()),
                Tip = dr["s_ime"].ToString(),
                Sostojba = dr["tipo_ime"].ToString()
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