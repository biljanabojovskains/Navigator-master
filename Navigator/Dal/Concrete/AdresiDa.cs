using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Navigator.Dal.Abstract;
using NLog;
using System.Data;
using NpgsqlTypes;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using System.Globalization;

namespace Navigator.Dal.Concrete
{
    public class AdresiDa : IAdresiDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        //Adresi za Opstina Aerodrom
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
                GeoJson = dr["geom"].ToString()

            };
            return p;
        }

        //Adresi za Opstina Veles
        public List<IStreet> GetListVStreet()
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
                    "select distinct mk_street from adresi_veles order by mk_street;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IStreet> list = (from DataRow dr in dt.Rows select CreateStreetVObject(dr)).ToList();
            return list;
        }
        private IStreet CreateStreetVObject(DataRow dr)
        {
            var street = new Street
            {
                id = dr["mk_street"].ToString(),
                text = dr["mk_street"].ToString(),
            };
            return street;
        }
        public List<IStreet> GetAllStreet()
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
                    "select distinct mk_street from adresi order by mk_street;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IStreet> list = (from DataRow dr in dt.Rows select CreateAllStreetObject(dr)).ToList();
            return list;
        }
        private IStreet CreateAllStreetObject(DataRow dr)
        {
            var street = new Street
            {
                id = dr["mk_street"].ToString(),
                text = dr["mk_street"].ToString(),
            };
            return street;
        }


     

        public List<IStNumber> GetListVNumbers(string ulica)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select distinct mk_street,number from adresi_veles where mk_street =:ulica order by number;";
                Db.CreateParameterFunc(cmd, "@ulica", ulica, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IStNumber> list = (from DataRow dr in dt.Rows select CreateVNumberObject(dr)).ToList();

            return list;
        }
        private static IStNumber CreateVNumberObject(DataRow dr)
        {
            var number = new StNumber
            {
                text = dr["number"].ToString(),
                id = dr["number"].ToString(),
            };
            return number;
        }
        public List<IStNumber> GetListStreetNumbers(string ulica)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select distinct mk_street,mk_number from adresi where mk_street =:ulica order by mk_number;";
                Db.CreateParameterFunc(cmd, "@ulica", ulica, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IStNumber> list = (from DataRow dr in dt.Rows select CreateStreetNumberObject(dr)).ToList();

            return list;
        }
        private static IStNumber CreateStreetNumberObject(DataRow dr)
        {
            var number = new StNumber
            {
                text = dr["mk_number"].ToString(),
                id = dr["mk_number"].ToString(),
            };
            return number;
        }
        public IAdresi SearchVAdress(string ulica, string broj)
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
                    @"select distinct gid, ST_AsGeoJSON(geom) as geom from adresi_veles where mk_street LIKE :ulica and number=:broj limit 1;";

                Db.CreateParameterFunc(cmd, "@ulica", '%' + ulica + '%', NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@broj", broj, NpgsqlDbType.Numeric);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            IAdresi list = CreateVObject(dt.Rows[0]);

            return list;
        }
        private static IAdresi CreateVObject(DataRow dr)
        {
            var p = new Adresi
            {
                Id = int.Parse(dr["gid"].ToString()),
                GeoJson = dr["geom"].ToString()
            };
            return p;
        }

        public IAdresi SearchAdressPoint(string ulica, string broj)
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
                    @"select distinct gid, ST_AsGeoJSON(geom) as geom from adresi where mk_street LIKE :ulica and mk_number=:broj limit 1;";

                Db.CreateParameterFunc(cmd, "@ulica", '%' + ulica + '%', NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@broj", broj, NpgsqlDbType.Numeric);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            IAdresi list = CreateAdressObject(dt.Rows[0]);

            return list;
        }
        private static IAdresi CreateAdressObject(DataRow dr)
        {
            var p = new Adresi
            {
                Id = int.Parse(dr["gid"].ToString()),
                GeoJson = dr["geom"].ToString()
            };
            return p;
        }

        //Centar adresi
        public List<IStreet> GetCentarStreet()
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
                    "select distinct en_street from adresi order by en_street;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IStreet> list = (from DataRow dr in dt.Rows select CreateCentarStreetObject(dr)).ToList();
            return list;
        }


     
        
        private IStreet CreateCentarStreetObject(DataRow dr)
        {
            var street = new Street
            {
                id = dr["ulica_id"].ToString(),
                text = dr["ime_ulica"].ToString(),
            };
            return street;
        }

        public IAdresi SearchCAdressPoint(string ulica, string broj,string coordinates)
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
                    @"select distinct gid, ST_AsGeoJSON(geom) as geom from adresi where en_street LIKE :ulica and broj=:broj limit 1;";

                Db.CreateParameterFunc(cmd, "@ulica", '%' + ulica + '%', NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@broj", broj, NpgsqlDbType.Numeric);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            IAdresi list = CreateCAdressObject(dt.Rows[0]);

            return list;
        }
        private static IAdresi CreateCAdressObject(DataRow dr)
        {
            var p = new Adresi
            {
                Id = int.Parse(dr["gid"].ToString()),
                GeoJson = dr["geom"].ToString()
            };
            return p;
        }

        public List<IStNumber> GetListCStreetNumbers(string ulica)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select distinct en_street,broj from adresi where en_street =:ulica order by broj;";
                Db.CreateParameterFunc(cmd, "@ulica", ulica, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IStNumber> list = (from DataRow dr in dt.Rows select CreateStreetCNumberObject(dr)).ToList();

            return list;
        }
        private static IStNumber CreateStreetCNumberObject(DataRow dr)
        {
            var number = new StNumber
            {
                text = dr["broj"].ToString(),
                id = dr["broj"].ToString(),
            };
            return number;
        }

        public IAdresi SearchCAdress(string ulica, string broj)
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
                    @"select distinct id, ST_AsGeoJSON(geom) as geom from adresi where en_street LIKE :ulica and broj=:broj limit 1;";

                Db.CreateParameterFunc(cmd, "@ulica", '%' + ulica + '%', NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@broj", broj, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            IAdresi list = CreateCObject(dt.Rows[0]);

            return list;
        }
        private static IAdresi CreateCObject(DataRow dr)
        {
            var p = new Adresi
            {
                Id = int.Parse(dr["id"].ToString()),
                GeoJson = dr["geom"].ToString()
            };
            return p;
        }

        public List<IStreet> GetAdresi(string poligon)
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
                    "select distinct ulica_id, ime_ulica from ulica_info as text where ST_Intersects(geom, ST_GeomFromText('POLYGON(( " + poligon + "))', 6316)) order by ime_ulica;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            List<IStreet> list = (from DataRow dr in dt.Rows select CreateCentarStreetObject(dr)).ToList();
            return list;
        }

       
    }
}