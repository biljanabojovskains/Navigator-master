using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Dal.Abstract;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NLog;
using NpgsqlTypes;

namespace Navigator.Dal.Concrete
{  
    public class BlokDa : IBaseDal<IBlok>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
       
        public List<IBlok> GenerateList(string coordinates)
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
                    "select * from mikro_blok where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" +
                    coordinates +
                    "),6316),geom);";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IBlok> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }
       
        private IBlok CreateObject(DataRow dr)
        {
            var item = new Blok
            {
                Id = int.Parse(dr["mikro_blok_id"].ToString()),
                Ime = dr["blok_ime"].ToString(),
                Namena = dr["namena"].ToString(),
                Povrshina = (dr["povrsina"] == null || dr["povrsina"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["povrsina"].ToString()),
                PovrshinaPresmetana = (dr["povrsina_presmetana"] == null || dr["povrsina_presmetana"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["povrsina_presmetana"].ToString())
            };
            try
            {
                item.GeoJson = dr["geojson"].ToString();
            }
            catch
            {
                // ignored
            }
            return item;
        }
       
        public List<IBlok> Search(string keyword)
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
                    "select *, ST_AsGeoJSON(geom) as geojson from mikro_blok where active=true and valid_to='infinity' and blok_ime like :kw;";
                Db.CreateParameterFunc(cmd, "@kw", '%' + keyword + '%', NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IBlok> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }
       
        public IVertex GetCentroidById(int id)
        {
            throw new NotImplementedException();
        }


        public IBlok Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}