using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Models.Concrete.FileldGis;
using NLog;

namespace Navigator.Dal.Concrete.FieldGis
{
    public class OsvetluvanjeDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<Osvetluvanje> GenerateList(string coordinates)
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
                    "select o.osvetluvanje_id, sp.ime as sp_ime, sot.ime as sot_ime, ss.ime as ss_ime, tipo.ime as tipo_ime " +
                    "from osvetluvanje o left join sif_osvetluvanje_tip sot on o.sif_osvetluvanje_tip_id_fk = sot.sif_osvetluvanje_tip_id " +
                    "left join sif_postavenost sp on o.sif_postavenost_id_fk = sp.sif_postavenost_id " +
                    "left join sif_sostojba ss on o.sif_sostojba_id_fk = ss.sif_sostojba_id " +
                    "left join tip_osvetluvanje tipo on o.tip_osvetluvanje_id_fk = tipo.tip_osvetluvanje_id " +
                    " where ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),ST_Buffer(o.geom,5));";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<Osvetluvanje> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private Osvetluvanje CreateObject(DataRow dr)
        {
            var item = new Osvetluvanje
            {
                Id = int.Parse(dr["osvetluvanje_id"].ToString()),
                Postavenost = dr["sp_ime"].ToString(),
                Sostojba = dr["ss_ime"].ToString(),
                TipOsvetluvanje = dr["tipo_ime"].ToString(),
                TipSijalica = dr["sot_ime"].ToString()
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