using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Models.Concrete.FileldGis;
using NLog;

namespace Navigator.Dal.Concrete.FieldGis
{
    public class VodovodDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<Vodovod> GenerateList(string coordinates)
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
                    "select d.vodovod_id, tipo.ime as tipo_ime, s.ime as sif_ime " +
                    "from vodovod d left join sif_sahta_tip s on d.sif_sahta_tip_id_fk = s.sif_sahta_tip_id " +
                    "left join tip_vodovod_kanalizacija tipo on d.tip_vodovod_kanalizacija_id_fk = tipo.tip_vodovod_kanalizacija_id " +
                    " where ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),ST_Buffer(d.geom,5));";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<Vodovod> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private Vodovod CreateObject(DataRow dr)
        {
            var item = new Vodovod
            {
                Id = int.Parse(dr["vodovod_id"].ToString()),
                Sifra = dr["sif_ime"].ToString(),
                Tip = dr["tipo_ime"].ToString()
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