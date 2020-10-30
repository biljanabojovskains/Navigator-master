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
    public class OdobrenieGradbaDa : IOdobrenieGradbaDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public bool Insert(int fkParcela, string brPredmet, string tipBaranje, string sluzbenik, DateTime datumBaranja, DateTime datumIzdavanja, DateTime datumPravosilno, string investitor, string brKP, string ko, string adresa, string parkingMestaParcela, string parkingMestaGaraza, string katnaGaraza, double iznosKomunalii, string zabeleski, string path)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                 "insert into odobrenie_za_gradba(fk_parcela, br_predmet, tip_baranje, sluzbenik, datum_baranje, datum_izdavanje, datum_pravosilno, investitor, br_kp, ko, adresa, parking_mesta_pacela, parking_mesta_garaza, katna_garaza, iznos_komunalii, zabeleski, dup, odluka_dup, datum_odluka, napemna_gp, br_gp, geom)  select :fk_par, :br_pre, :tip_bara, :sluz, :dat_bara, :dat_izda, :dat_pravo, :inve, :brkp, :ko, :adr, :parmpar, :parkmgar, :katgar, :izkom, :zab, mo.opfat_ime, mo.br_odluka, mo.datum_donesuvanje, mg.klasa_namena, mg.broj, mg.geom from mikro_opfat as mo left join mikro_gradezni_parceli as mg on mo.mikro_opfat_id = mg.fk_mikro_opfat where mg.mikro_gradezna_parcela_id=:fk_par ;";
                Db.CreateParameterFunc(cmd, "@fk_par", fkParcela, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@br_pre", brPredmet, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@tip_bara", tipBaranje, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@sluz", sluzbenik, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@dat_bara", datumBaranja.Date, NpgsqlDbType.Date);
                Db.CreateParameterFunc(cmd, "@dat_izda", datumIzdavanja.Date, NpgsqlDbType.Date);
                Db.CreateParameterFunc(cmd, "@dat_pravo", datumPravosilno.Date, NpgsqlDbType.Date);
                Db.CreateParameterFunc(cmd, "@inve", investitor, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@brkp", brKP, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@ko", ko, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@adr", adresa, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@parmpar", parkingMestaParcela, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@parkmgar", parkingMestaGaraza, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@katgar", katnaGaraza, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@izkom", iznosKomunalii, NpgsqlDbType.Numeric);
                Db.CreateParameterFunc(cmd, "@zab", zabeleski, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@pat", path, NpgsqlDbType.Text);
                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        public bool InsertOdobrenieSoPodtip(int fkParcela, string brPredmet, string tipBaranje, string sluzbenik, DateTime datumBaranja, DateTime datumIzdavanja, DateTime datumPravosilno, string investitor, string brKP, string ko, string adresa, string parkingMestaParcela, string parkingMestaGaraza, string katnaGaraza, double iznosKomunalii, string zabeleski, string path, string podtipBaranje)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                 "insert into odobrenie_za_gradba(fk_parcela, br_predmet, tip_baranje, podtip_baranje, sluzbenik, datum_baranje, datum_izdavanje, datum_pravosilno, investitor, br_kp, ko, adresa, parking_mesta_pacela, parking_mesta_garaza, katna_garaza, iznos_komunalii, zabeleski, dup, odluka_dup, datum_odluka, napemna_gp, br_gp, geom)  select :fk_par, :br_pre, :tip_bara, :podtip, :sluz, :dat_bara, :dat_izda, :dat_pravo, :inve, :brkp, :ko, :adr, :parmpar, :parkmgar, :katgar, :izkom, :zab, mo.opfat_ime, mo.br_odluka, mo.datum_donesuvanje, mg.klasa_namena, mg.broj, mg.geom from mikro_opfat as mo left join mikro_gradezni_parceli as mg on mo.mikro_opfat_id = mg.fk_mikro_opfat where mg.mikro_gradezna_parcela_id=:fk_par ;";
                Db.CreateParameterFunc(cmd, "@fk_par", fkParcela, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@br_pre", brPredmet, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@tip_bara", tipBaranje, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@sluz", sluzbenik, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@dat_bara", datumBaranja.Date, NpgsqlDbType.Date);
                Db.CreateParameterFunc(cmd, "@dat_izda", datumIzdavanja.Date, NpgsqlDbType.Date);
                Db.CreateParameterFunc(cmd, "@dat_pravo", datumPravosilno.Date, NpgsqlDbType.Date);
                Db.CreateParameterFunc(cmd, "@inve", investitor, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@brkp", brKP, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@ko", ko, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@adr", adresa, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@parmpar", parkingMestaParcela, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@parkmgar", parkingMestaGaraza, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@katgar", katnaGaraza, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@izkom", iznosKomunalii, NpgsqlDbType.Numeric);
                Db.CreateParameterFunc(cmd, "@zab", zabeleski, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@pat", path, NpgsqlDbType.Text);
                Db.CreateParameterFunc(cmd, "@podtip", podtipBaranje, NpgsqlDbType.Text);

                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        public bool InsertDocument(int fkParcela, string path)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                 "insert into odobrenie_predmet_doc(fk_parcela, path, dup, geom)  select :fk_par, :pat, mo.opfat_ime, mg.geom from mikro_opfat as mo left join mikro_gradezni_parceli as mg on mo.mikro_opfat_id = mg.fk_mikro_opfat where mg.mikro_gradezna_parcela_id=:fk_par ;";
                Db.CreateParameterFunc(cmd, "@fk_par", fkParcela, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@pat", path, NpgsqlDbType.Text);

                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        public List<IOdobrenieGradba> GetOdobrenija(string coordinates)
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
                     "select *, ST_AsGeoJSON(geom) as geojson from odobrenie_za_gradba g where ST_Intersects(g.geom, (select geom as geo from mikro_gradezni_parceli where active=true and valid_to='infinity' and produkcija=true and ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),geom))) and st_area(st_intersection(g.geom, (select geom as geo from mikro_gradezni_parceli where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),geom))))>7";
                   //"select *, ST_AsGeoJSON(geom) as geojson from odobrenie_za_gradba g where ST_Intersects(g.geom, (select geom as geo from mikro_gradezni_parceli where active=true and valid_to='infinity' and produkcija=true and ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),geom))) and st_area(st_intersection(g.geom, (select geom as geo from mikro_gradezni_parceli where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),geom))))>36";
                   //"select *, ST_AsGeoJSON(geom) as geojson from odobrenie_za_gradba g where ST_Intersects(g.geom, (select geom as geo from mikro_gradezni_parceli where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),geom)))";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOdobrenieGradba> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private IOdobrenieGradba CreateObject(DataRow dr)
        {
            var odobrenie = new OdobrenieGradba
            {
                Id = int.Parse(dr["odobrenie_id"].ToString()),
                FkParcela = int.Parse(dr["fk_parcela"].ToString()),
                BrPredmet = dr["br_predmet"].ToString(),
                TipBaranje = dr["tip_baranje"].ToString(),
                PodTipBaranje = dr["podtip_baranje"].ToString(),
                Sluzbenik = dr["sluzbenik"].ToString(),
                DatumBaranja = DateTime.Parse(dr["datum_baranje"].ToString()),
                DatumIzdavanja = DateTime.Parse(dr["datum_izdavanje"].ToString()),
                DatumPravosilno = DateTime.Parse(dr["datum_pravosilno"].ToString()),
                Investitor = dr["investitor"].ToString(),
                BrKP = dr["br_kp"].ToString(),
                KO = dr["ko"].ToString(),
                adresa = dr["adresa"].ToString(),
                ParkingMestaPacela = dr["parking_mesta_pacela"].ToString(),
                ParkingMestaGaraza = dr["parking_mesta_garaza"].ToString(),
                KatnaGaraza = dr["katna_garaza"].ToString(),
                IznosKomunalii = double.Parse(dr["iznos_komunalii"].ToString()),
                Zabeleski = dr["zabeleski"].ToString(),
                Path = dr["path"].ToString(),
                Dup = dr["dup"].ToString(),
                OdlukaDup = dr["odluka_dup"].ToString(),
                DonesuvanjeOdlukaDup = DateTime.Parse(dr["datum_odluka"].ToString()),
                Namena = dr["napemna_gp"].ToString(),
                BrNamena = dr["br_gp"].ToString(),
                GeoJson = dr["geojson"].ToString()
            };
            return odobrenie;
        }
        public List<IOdobrenieGradba> GetDocuments(string coordinates)
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

                   "select *, ST_AsGeoJSON(geom) as geojson from odobrenie_predmet_doc g where ST_Intersects(g.geom, (select geom as geo from mikro_gradezni_parceli where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),geom)))";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IOdobrenieGradba> list = (from DataRow dr in dt.Rows select CreateDocObject(dr)).ToList();
            return list;
        }

        private IOdobrenieGradba CreateDocObject(DataRow dr)
        {
            var documents = new OdobrenieGradba
            {
                Id = int.Parse(dr["doc_id"].ToString()),
                FkParcela = int.Parse(dr["fk_parcela"].ToString()),              
                Path = dr["path"].ToString(),
                Dup = dr["dup"].ToString(),
                GeoJson = dr["geojson"].ToString(),
            };
            return documents;
        }
        public bool Delete(int id)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                 "delete from odobrenie_za_gradba where odobrenie_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);

                var rowsAffected = Db.ExecuteNonQuery(cmd);
                return rowsAffected == 1;
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
        }
        public List<IKatniGarazi> GetAllKatniGarazi()
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
                    "SELECT * FROM katna_garaza;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IKatniGarazi> list = (from DataRow dr in dt.Rows select CreateKatnaGarazaObject(dr)).ToList();

            return list;
        }
        private static IKatniGarazi CreateKatnaGarazaObject(DataRow dr)
        {
            var garaza = new KatniGarazi
            {
                KatniGaraziId = int.Parse(dr["katna_garaza_id"].ToString()),
                KatniGaraziName = dr["garaza_ime"].ToString()
            };
            return garaza;
        }
        public List<ITipBaranje> GetAllTipBaranja()
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
                    "SELECT * FROM tip_baranja;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<ITipBaranje> list = (from DataRow dr in dt.Rows select CreateTipBaranjaObject(dr)).ToList();

            return list;
        }
        private static ITipBaranje CreateTipBaranjaObject(DataRow dr)
        {
            var baranje = new TipBaranje
            {
                TipBaranjeId = int.Parse(dr["tip_baranje_id"].ToString()),
                TipBaranjeName = dr["tip_baranje_ime"].ToString()
            };
            return baranje;
        }
        public List<IPodTipBaranje> GetAllPodTipBaranja()
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
                    "SELECT * FROM podtip_baranja;";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<IPodTipBaranje> list = (from DataRow dr in dt.Rows select CreatePodTipBaranjaObject(dr)).ToList();

            return list;
        }

        private static IPodTipBaranje CreatePodTipBaranjaObject(DataRow dr)
        {
            var baranje = new PodTipBaranje
            {
                id = int.Parse(dr["podtip_baranje_id"].ToString()),
                text = dr["podtip_baranje_ime"].ToString()
            };
            return baranje;
        }
    }
}