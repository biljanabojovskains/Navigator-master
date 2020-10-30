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
    public class UliciDa : IUliciDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<ISegment> GetListSegmenti(string poligon)
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
                      "select * ,ST_AsGeoJSON(geom) as geojson from segment where  ST_Intersects(geom, ST_GeomFromText('POLYGON(( " + poligon + "))', 6316)) ;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ISegment> list = (from DataRow dr in dt.Rows select CreateSegmentObject(dr)).ToList();
            return list;
        }

     
        public List<IUlicaInfo> GetListUlici(string poligon)
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
                      "select * ,ST_AsGeoJSON(geom) as geojson from ulica_info where  ST_Intersects(geom, ST_GeomFromText('POLYGON(( " + poligon + "))', 6316)) ;";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IUlicaInfo> list = (from DataRow dr in dt.Rows select CreateUliciInfoObject(dr)).ToList();
            return list;
        }

        public List<ISegment> GetSegmentiUlica(string poligon, int id)
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
                      @"select  i.ulica_id, se.* from ulica_info i left join ulici_segmenti s on i.ulica_id= s.fk_ulica_id
                left join segment se on s.fk_segment_ulica_id= se.segment_ulica_id
                where i.ulica_id=:id and ST_Intersects(se.geom, ST_GeomFromText('POLYGON(( " + poligon + "))', 6316));";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ISegment> list = (from DataRow dr in dt.Rows select CreateSegmentObject(dr)).ToList();
            return list;
        }



        public IUlicaInfo Get(int id)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select *, ST_AsGeoJSON(geom) as geojson from ulica_info where ulica_id=:id;";
                Db.CreateParameterFunc(cmd, "@id", id, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            return CreateUliciInfoObject(dt.Rows[0]);
        }





        private static IUlicaInfo CreateUliciInfoObject(DataRow dr)
        {
            var uliciInfo = new UlicaInfo
            {
                Id = int.Parse(dr["ulica_id"].ToString()),
                Ime_ulica = dr["ime_ulica"].ToString(),
                GeoJson = dr["geom"].ToString()

            };
            return uliciInfo;
        }

        private static ISegment CreateSegmentObject(DataRow dr)
        {
            var segment = new Segment
            {
                Id = int.Parse(dr["segment_ulica_id"].ToString()),
                Ime_ulica = dr["ime_ulica"].ToString(),
                Tip_Ulica = dr["tip_ulica"].ToString(),
                SegmentBr = int.Parse(dr["segment"].ToString()),
                Shirina = double.Parse(dr["shirina"].ToString()),
                Trotoari = bool.Parse(dr["trotoari"].ToString()),
                Velosipedska_pateka = bool.Parse(dr["velosipedska_pateka"].ToString()),
                Zelenilo = bool.Parse(dr["zelenilo"].ToString()),
                Atmosferska_planirana = bool.Parse(dr["atmosferska_kanalizacija_planirana"].ToString()),
                Atmosferska_postojna = bool.Parse(dr["atmosferska_kanalizacija_postojna"].ToString()),
                Vodovodna_planirana = bool.Parse(dr["vodovodna_infrastruktura_planirana"].ToString()),
                Vodovodna_postojna = bool.Parse(dr["vodovodna_infrastruktura_postojna"].ToString()),
                Gasovodna_planirana = bool.Parse(dr["gasovodna_infrastruktura_planirana"].ToString()),
                Gasovodna_postojna = bool.Parse(dr["gasovodna_infrastruktura_postojna"].ToString()),
                Telekomunikaciska_planirana = bool.Parse(dr["telekomunikaciska_infrastruktura_planirana"].ToString()),
                Telekomunikaciska_postojna = bool.Parse(dr["telekomunikaciska_infrastruktura_postojna"].ToString()),
                Elektrika_planirana = bool.Parse(dr["elektrika_planirana"].ToString()),
                Elektrika_postojna = bool.Parse(dr["elektrika_postojna"].ToString()),
                Fekalna_planirana = bool.Parse(dr["fekalna_infrastruktura_planirana"].ToString()),
                Fekalna_postojna = bool.Parse(dr["fekalna_infrastruktura_postojna"].ToString()),
                Toplifikacija_planirana = bool.Parse(dr["toplifikacija_planirana"].ToString()),
                Toplifikacija_postojna = bool.Parse(dr["toplifikacija_postojna"].ToString()),
                GeoJson = dr["geom"].ToString(),
                Parking = bool.Parse(dr["parking"].ToString()),
                Pesacka_pateka = bool.Parse(dr["pesacka_pateka"].ToString())
                
                
            };
            return segment;
        }

        private static IUlicaSegment CreateUlicaSegmentObject(DataRow dr)
        {
            var ulicaSegment = new UlicaSegment
            {
                Id = int.Parse(dr["ulici_segmenti_id"].ToString()),
                Fk_ulica_id = int.Parse(dr["fk_ulica_id"].ToString()),
                Fk_segment_ulica_id = int.Parse(dr["fk_segment_ulica_id"].ToString())

            };
            return ulicaSegment;
        }




    }
}