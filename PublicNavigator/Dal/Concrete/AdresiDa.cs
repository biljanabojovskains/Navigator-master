using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PublicNavigator.Dal.Abstract;
using NLog;
using System.Data;
using NpgsqlTypes;
using PublicNavigator.Models.Abstract;
using PublicNavigator.Models.Concrete;
using System.Globalization;

namespace PublicNavigator.Dal.Concrete
{
    public class AdresiDa : IAdresiDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<IStreet> GetListStreet()
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
                    "select distinct ime_adresa from adresi_aerodrom order by ime_adresa;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IStreet> list = (from DataRow dr in dt.Rows select CreateStreetObject(dr)).ToList();
            return list;
        }
        private IStreet CreateStreetObject(DataRow dr)
        {
            var street = new Street
            {
                id = dr["ime_adresa"].ToString(),
                text = dr["ime_adresa"].ToString(),
            };
            return street;
        }
        public List<IStNumber> GetListNumbers(string ulica)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select distinct ime_adresa,broj_adresa from adresi_aerodrom where ime_adresa =:ulica order by broj_adresa;";
                Db.CreateParameterFunc(cmd, "@ulica", ulica, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IStNumber> list = (from DataRow dr in dt.Rows select CreateNumberObject(dr)).ToList();

            return list;
        }
        private static IStNumber CreateNumberObject(DataRow dr)
        {
            var number = new StNumber
            {
                text = dr["broj_adresa"].ToString(),
                id = dr["broj_adresa"].ToString(),
            };
            return number;
        }
        public IAdresi SearchAdress(string ulica, string broj)
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
                    @"select distinct adresa_id, ST_AsGeoJSON(geom) as geom from adresi_aerodrom where ime_adresa LIKE :ulica and broj_adresa=:broj limit 1;";

                Db.CreateParameterFunc(cmd, "@ulica", '%' + ulica + '%', NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@broj", broj, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            IAdresi list =  CreateObject(dt.Rows[0]);

            return list;
        }

        private static IAdresi CreateObject(DataRow dr)
        {
            var p = new Adresi
            {
                Id = int.Parse(dr["adresa_id"].ToString()),
                GeoJson = dr["geom"].ToString(),

            };
            return p;
        }
        
    }
}