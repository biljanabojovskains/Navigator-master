using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Navigator.Dal.Abstract;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NLog;
using NpgsqlTypes;

namespace Navigator.Dal.Concrete
{
    public class ParceliDa : IBaseDal<IGradParceli>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<IGradParceli> GenerateList(string coordinates)
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
                    "select *, ST_AsGeoJSON(geom) as geojson from mikro_gradezni_parceli where active=true and valid_to='infinity' and produkcija=true and ST_Intersects(ST_SetSRID(ST_MakePoint(" +
                    coordinates +
                    "),6316),geom);";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IGradParceli> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public IVertex GetCentroidById(int id)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "SELECT ST_AsText(ST_Centroid(geom)) FROM mikro_gradezni_parceli WHERE mikro_gradezna_parcela_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                var result = Db.ExecuteScalar(cmd);
                result = result.Remove(0, 6); // remove POINT((
                result = result.Remove(result.Length - 1); // remove ))
                return CreateVertex(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        private static IVertex CreateVertex(string item)
        {
            var pair = item.Split(' ');
            var v = new Vertex
            {
                X = Double.Parse(pair[0], new CultureInfo("en-US")),
                Y = Double.Parse(pair[1], new CultureInfo("en-US"))
            };
            return v;
        }

        private IGradParceli CreateObject(DataRow dr)
        {
            var item = new GradParceli
            {
                Id = int.Parse(dr["mikro_gradezna_parcela_id"].ToString()),
                Broj = dr["broj"].ToString(),
                KlasaNamena = dr["klasa_namena"].ToString(),
                KompKlasaNamena = dr["komp_klasa_namena"].ToString(),
                MaxVisina = dr["max_visina"].ToString(),
                Katnost = dr["katnost"].ToString(),
                Extrude = (dr["extrude"] == null || dr["extrude"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["extrude"].ToString()),
                ParkingMesta = dr["parking_mesta"].ToString(),
                ProcentIzgradenost = (dr["procent_izgradenost"] == null || dr["procent_izgradenost"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["procent_izgradenost"].ToString()),
                KoeficientIskoristenost =
                    (dr["koef_iskoristenost"] == null || dr["koef_iskoristenost"].ToString() == "")
                        ? (double?) null
                        : double.Parse(dr["koef_iskoristenost"].ToString()),
                PovrshinaGradenje = (dr["povrsina_gradenje"] == null || dr["povrsina_gradenje"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["povrsina_gradenje"].ToString()),
                BrutoPovrshina = (dr["bruto_povrsina"] == null || dr["bruto_povrsina"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["bruto_povrsina"].ToString()),
                Povrshina = (dr["povrsina"] == null || dr["povrsina"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["povrsina"].ToString()),
                PovrshinaPresmetana = (dr["povrsina_presmetana"] == null || dr["povrsina_presmetana"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["povrsina_presmetana"].ToString()),
                OpstiUsloviId = (dr["fk_opsti_uslovi"] == null || dr["fk_opsti_uslovi"].ToString() == "")
                    ? (int?) null
                    : int.Parse(dr["fk_opsti_uslovi"].ToString()),
                PosebniUsloviId = (dr["fk_posebni_uslovi"] == null || dr["fk_posebni_uslovi"].ToString() == "")
                    ? (int?) null
                    : int.Parse(dr["fk_posebni_uslovi"].ToString()),
                OpfatId = int.Parse(dr["fk_mikro_opfat"].ToString()),
                NumerickiPokazateliId = (dr["fk_numericki_pokazateli"] == null || dr["fk_numericki_pokazateli"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_numericki_pokazateli"].ToString())
                
            };
            try
            {
                item.GeoJson = dr["geojson"].ToString();
            }
            catch
            {
                // ignored
            }
            try
            {
                item.Investitor = dr["investitor"].ToString();
            }

            catch
            {
                // ignored
            }
            try {
                item.BrTehIspravka = dr["br_teh_ispravka"].ToString();
                item.DatumTehIspravka = (dr["datum_teh_ispravka"] == null || dr["datum_teh_ispravka"].ToString() == "")
                    ? (DateTime?)null
                    : DateTime.Parse(dr["datum_teh_ispravka"].ToString());
            }
            catch {
                // ignored
            }
            try
            {
                item.TehnickiIspravkiId = (dr["fk_tehnicki_ispravki"] == null || dr["fk_tehnicki_ispravki"].ToString() == "")
                    ? (int?)null
                    : int.Parse(dr["fk_tehnicki_ispravki"].ToString());
            }

            catch
            {
                // ignored
            }
            try
            {
                item.Presek = (dr["presek"] == null || dr["presek"].ToString() == "")
                    ? (double?) null
                    : double.Parse(dr["presek"].ToString());
            }
            catch
            {
                // ignored
            }
            return item;
        }

        public List<IGradParceli> Search(string keyword)
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
                    "select *, ST_AsGeoJSON(geom) as geojson from mikro_gradezni_parceli where active=true and valid_to='infinity' and broj like :kw;";
                Db.CreateParameterFunc(cmd, "@kw", '%' + keyword + '%', NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IGradParceli> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public List<IGradParceli> GetByOpfat(int opfatId)
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
                    "select *, ST_AsGeoJSON(geom) as geojson from mikro_gradezni_parceli where fk_mikro_opfat = :id;";
                Db.CreateParameterFunc(cmd, "@id", opfatId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IGradParceli> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }
        
        public List<IGradParceli> GeneratePreklop(int gradParcPlaniranaId)
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
                    "select stari.*, round(cast(st_area(st_intersection(ST_SetSRID(ST_MakeValid(stari.geom), 6316), ST_SetSRID(ST_MakeValid(novi.geom), 6316))) as numeric),2) as presek " +
                    "from mikro_gradezni_parceli as stari, mikro_gradezni_parceli as novi " +
                    "where stari.produkcija = true and stari.active=true and stari.valid_to='infinity' and novi.mikro_gradezna_parcela_id=:id and st_intersects(ST_SetSRID(ST_MakeValid(stari.geom), 6316), ST_SetSRID(ST_MakeValid(novi.geom), 6316));";
                Db.CreateParameterFunc(cmd, "@id", gradParcPlaniranaId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IGradParceli> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }
        
        public IGradParceli Get(int id)
        {
            throw new NotImplementedException();
        }
        public string GetOpsti(int id)
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
                    "select path from opsti_uslovi where opsti_uslovi_id = :id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);

                string path = Db.ExecuteScalar(cmd).ToString();
                return path;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public string GetPosebni(int id)
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
                    "select path from posebni_uslovi where posebni_uslovi_id = :id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);

                string path = Db.ExecuteScalar(cmd).ToString();
                return path;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public string GetNumericki(int id)
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
                    "select path from numericki_pokazateli where numericki_pokazateli_id = :id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);

                string path = Db.ExecuteScalar(cmd).ToString();
                return path;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }

        public string GenerateTehnickiIspravki(int id)
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
                    "select path from tehnicki_ispravki where tehnicki_ispravki_id = :id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);

                string path = Db.ExecuteScalar(cmd).ToString();
                return path;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        public bool InsertImeInvestitor(int fkParcela, string investitor)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =   
                 "update mikro_gradezni_parceli set investitor=:inv where mikro_gradezna_parcela_id = :fk_par;";
                Db.CreateParameterFunc(cmd, "@fk_par", fkParcela, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@inv", investitor, NpgsqlDbType.Text);

                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        public List<IGradParceli2> GetListGParceli(string opfat)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select distinct broj,mikro_gradezna_parcela_id from mikro_gradezni_parceli left join mikro_opfat on mikro_gradezni_parceli.fk_mikro_opfat=mikro_opfat.mikro_opfat_id where mikro_opfat.opfat_ime=:opfat and mikro_gradezni_parceli.active=true and mikro_gradezni_parceli.valid_to='infinity';";
                Db.CreateParameterFunc(cmd, "@opfat", opfat, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IGradParceli2> list = (from DataRow dr in dt.Rows select CreateGParceliObject(dr)).ToList();

            return list;
        }
        private static IGradParceli2 CreateGParceliObject(DataRow dr)
        {
            var parceli = new GradParceli2
            {
                text = dr["broj"].ToString(),
                id = dr["mikro_gradezna_parcela_id"].ToString(),

            };
            return parceli;
        }
        public IGradezniParceli SearchParcela(int id)
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
                    "select *, ST_AsGeoJSON(geom) as geojson from mikro_gradezni_parceli where active=true and valid_to='infinity' and mikro_gradezna_parcela_id = :id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
            
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            IGradezniParceli list = CreateObjectParcela(dt.Rows[0]);

            return list;
        }
        private static IGradezniParceli CreateObjectParcela(DataRow dr)
        {
            var item = new GradezniParceli
            {
                Id = int.Parse(dr["mikro_gradezna_parcela_id"].ToString()),

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
        public bool CheckBbox(double bottom, double left, double right, double top, int idParcela)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                 "select st_contains(ST_MakeEnvelope(:l::double precision,:b::double precision,:r::double precision,:t::double precision, 6316), geom) from mikro_gradezni_parceli where mikro_gradezna_parcela_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", idParcela, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@b", bottom, NpgsqlDbType.Double);
                Db.CreateParameterFunc(cmd, "@l", left, NpgsqlDbType.Double);
                Db.CreateParameterFunc(cmd, "@r", right, NpgsqlDbType.Double);
                Db.CreateParameterFunc(cmd, "@t", top, NpgsqlDbType.Double);
                var salt = Db.ExecuteScalar(cmd);
                return   Convert.ToBoolean(salt);
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
    }
}