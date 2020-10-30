using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PublicNavigator.Dal.Abstract;
using PublicNavigator.Models.Abstract;
using PublicNavigator.Models.Concrete;
using NLog;
using NpgsqlTypes;

namespace PublicNavigator.Dal.Concrete
{
    public class BiznisInfoDa : IBiznisInfoDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<IBiznisInfo> GetListBiznisInfo(string coordinates)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "select *, ST_AsGeoJSON(geom) as geojson from poi_biznis where ST_Contains(ST_Buffer(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316), 50), geom);";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IBiznisInfo> list = (from DataRow dr in dt.Rows select CreateDocObject(dr)).ToList();
            return list;
        }

        private IBiznisInfo CreateDocObject(DataRow dr)
        {
            var info = new BiznisInfo
            {
                Id = int.Parse(dr["biznis_id"].ToString()),
                Ime = dr["ime"].ToString(),
                KontaktLice = dr["kontakt_lice"].ToString(),
                Telefon = dr["telefon"].ToString(),
                Email = dr["email"].ToString(),
                Adresa = dr["adresa"].ToString(),
                Web = dr["web"].ToString(),
                GeoJson = dr["geojson"].ToString(),        
            };
            return info;
        }
    }
}