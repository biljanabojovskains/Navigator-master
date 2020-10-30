using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Api.Dal.Abstract;
using Api.ViewModels.Concrete;
using Navigator.Dal;
using NLog;
using NpgsqlTypes;

namespace Api.Dal.Concrete
{
    public class GradParceliDa : IGradParceliDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<GradParceli> Get(double lon, double lat)
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
                    string.Format(
                        "select p.mikro_gradezna_parcela_id, p.broj, p.klasa_namena, p.komp_klasa_namena, p.max_visina, p.katnost, p.parking_mesta, p.procent_izgradenost, p.koef_iskoristenost, p.povrsina_gradenje, p.bruto_povrsina, p.povrsina, p.povrsina_presmetana, p.fk_mikro_opfat, o.opfat_ime from mikro_gradezni_parceli as p inner join mikro_opfat as o on p.fk_mikro_opfat=o.mikro_opfat_id where p.active=true and p.valid_to='infinity' and ST_Intersects(ST_Buffer(ST_Transform(ST_SetSRID(ST_MakePoint({0},{1}),4326),6316),50),p.geom);",
                        lon, lat);

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<GradParceli> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public List<GradParceli> GetByOpfat(int opfatId)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select p.mikro_gradezna_parcela_id, p.broj, p.klasa_namena, p.komp_klasa_namena, p.max_visina, p.katnost, p.parking_mesta, p.procent_izgradenost, p.koef_iskoristenost, p.povrsina_gradenje, p.bruto_povrsina, p.povrsina, p.povrsina_presmetana, p.fk_mikro_opfat, o.opfat_ime from mikro_gradezni_parceli as p inner join mikro_opfat as o on p.fk_mikro_opfat=o.mikro_opfat_id where p.fk_mikro_opfat=:id;";
                Db.CreateParameterFunc(cmd, "@id", opfatId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<GradParceli> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }
        
        private GradParceli CreateObject(DataRow dr)
        {
            var item = new GradParceli
            {
                Id = int.Parse(dr["mikro_gradezna_parcela_id"].ToString()),
                Broj = dr["broj"].ToString(),
                KlasaNamena = dr["klasa_namena"].ToString(),
                KompKlasaNamena = dr["komp_klasa_namena"].ToString(),
                MaxVisina = dr["max_visina"].ToString(),
                Katnost = dr["katnost"].ToString(),
                ParkingMesta = dr["parking_mesta"].ToString(),
                ProcentIzgradenost = (dr["procent_izgradenost"] == null || dr["procent_izgradenost"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["procent_izgradenost"].ToString()),
                KoeficientIskoristenost =
                    (dr["koef_iskoristenost"] == null || dr["koef_iskoristenost"].ToString() == "")
                        ? (double?)null
                        : double.Parse(dr["koef_iskoristenost"].ToString()),
                PovrshinaGradenje = (dr["povrsina_gradenje"] == null || dr["povrsina_gradenje"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["povrsina_gradenje"].ToString()),
                BrutoPovrshina = (dr["bruto_povrsina"] == null || dr["bruto_povrsina"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["bruto_povrsina"].ToString()),
                Povrshina = (dr["povrsina"] == null || dr["povrsina"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["povrsina"].ToString()),
                PovrshinaPresmetana = (dr["povrsina_presmetana"] == null || dr["povrsina_presmetana"].ToString() == "")
                    ? (double?)null
                    : double.Parse(dr["povrsina_presmetana"].ToString()),
                OpfatId = int.Parse(dr["fk_mikro_opfat"].ToString()),
                OpfatIme = dr["opfat_ime"].ToString()
            };
            return item;
        }

        public GradParceli Get(int id)
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
                    "select p.mikro_gradezna_parcela_id, p.broj, p.klasa_namena, p.komp_klasa_namena, p.max_visina, p.katnost, p.parking_mesta, p.procent_izgradenost, p.koef_iskoristenost, p.povrsina_gradenje, p.bruto_povrsina, p.povrsina, p.povrsina_presmetana, p.fk_mikro_opfat, o.opfat_ime from mikro_gradezni_parceli as p inner join mikro_opfat as o on p.fk_mikro_opfat=o.mikro_opfat_id where p.mikro_gradezna_parcela_id=:id;";
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

        public string GetGeom(int id)
        {
            string result;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText =
                    "select st_astext(ST_Transform(ST_SetSRID(geom,6316),4326)) from mikro_gradezni_parceli where mikro_gradezna_parcela_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                result = Db.ExecuteScalar(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return result;
        }


        public List<GradParceli> Get(List<int> ids)
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
                    "select p.mikro_gradezna_parcela_id, p.broj, p.klasa_namena, p.komp_klasa_namena, p.max_visina, p.katnost, p.parking_mesta, p.procent_izgradenost, p.koef_iskoristenost, p.povrsina_gradenje, p.bruto_povrsina, p.povrsina, p.povrsina_presmetana, p.fk_mikro_opfat, o.opfat_ime from mikro_gradezni_parceli as p inner join mikro_opfat as o on p.fk_mikro_opfat=o.mikro_opfat_id where p.mikro_gradezna_parcela_id=ANY(:id_list);";
                Db.CreateParameterFunc(cmd, "@id_list", ids, NpgsqlDbType.Array | NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<GradParceli> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
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
    }
}