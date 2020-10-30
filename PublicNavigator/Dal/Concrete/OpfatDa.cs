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
    public class OpfatDa : IBaseDal<IOpfat>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<IOpfat> GenerateList(string coordinates)
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
                    "select * from mikro_opfat where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" +
                    coordinates +
                    "),6316),geom);";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private IOpfat CreateObject(DataRow dr)
        {
            var item = new Opfat
            {
                Id = int.Parse(dr["mikro_opfat_id"].ToString()),
                Ime = dr["opfat_ime"].ToString(),
                TehnickiBroj = dr["tehnicki_broj"].ToString(),
                Povrshina = (dr["povrsina"] == null || dr["povrsina"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["povrsina"].ToString()),
                PovrshinaPresmetana = (dr["povrsina_presmetana"] == null || dr["povrsina_presmetana"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["povrsina_presmetana"].ToString()),
                PlanskiPeriod = dr["planski_period"].ToString(),
                ZakonskaRegulativa = dr["zakonska_regulativa"].ToString(),
                Izrabotuva = dr["izrabotuva"].ToString(),
                BrOdluka = dr["br_odluka"].ToString(),
                DatumNaDonesuvanje = (dr["datum_donesuvanje"] == null || dr["datum_donesuvanje"].ToString() == "")
                    ? (DateTime?) null
                    : DateTime.Parse(dr["datum_donesuvanje"].ToString())
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


        public IVertex GetCentroidById(int id)
        {
            throw new NotImplementedException();
        }

        public List<IOpfat> Search(string keyword)
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
                    "select *, ST_AsGeoJSON(geom) as geojson from mikro_opfat where active=true and valid_to='infinity' and opfat_ime like :kw;";
                Db.CreateParameterFunc(cmd, "@kw", '%' + keyword + '%', NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public List<IOpfat> GetAllTekovni()
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
                    "select * from mikro_opfat where active=true and valid_to='infinity' and produkcija=true;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public List<IOpfat> GetAllNedoneseni()
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select * from mikro_opfat where produkcija=false;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOpfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public IOpfat Get(int id)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select *, ST_AsGeoJSON(geom) as geojson from mikro_opfat where mikro_opfat_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return CreateObject(dt.Rows[0]);
        }
    }
}