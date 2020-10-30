using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Web;
using NLog;
using PublicNavigator.Dal.Abstract;
using PublicNavigator.Dal.Concrete;
using PublicNavigator.Models.Abstract;

namespace PublicNavigator.Bll
{
    public class Bl
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static List<ITema> GetAllTemi()
        {
            var temi = new NotifikaciiDa().GetListTemi();
            return temi;
        }

        public static List<IPodTema> GetAllPodTemi(int temaId)
        {
            var podTemi = new NotifikaciiDa().GetListPodTema(temaId);
            return podTemi;
        }

        public static bool InsertPoint(int podtema, int userId, string coordinates,string email)
        {
            int counter = new NotifikaciiDa().CheckCounerPoi(userId);
            if (counter >= 3) return false;
            var result = new NotifikaciiDa().Insert(podtema, userId, coordinates,email);
            return result;
        }

        public static IDrillDownInfo SearchAll(string searchString)
        {
            var info = new DrillDownInfoDa().SearchAll(searchString);
            return info;
        }

        public static IDrillDownInfo SearchKatastarskiParceli(string searchString)
        {
            var info = new DrillDownInfoDa().SearchKatastarskiParceli(searchString);
            return info;
        }

        public static bool InsertUser(string userName, string password, string fullName, string phone, string email)
        {
            var result = new UserDa().Insert(userName, password, fullName, phone, email);
            return result;
        }

        public static bool CreateResetPasswordToken(string usermail)
        {
            var result = new UserDa().CreateResetPasswordToken(usermail);
            return result;
        }

        public static bool ResetPassword(string email, string token, string password)
        {
            var result = new UserDa().ResetPassword(email, token, password);
            return result;
        }

        public static string GenerateDxf(string coordinates, int userId)
        {
            int counterDxf = new UserDa().CheckCounerDxf(userId);

            if (counterDxf > 0)
            {
                string dbName = ConfigurationManager.AppSettings["dbName"];
                string dbAddress = ConfigurationManager.AppSettings["dbServer"];
                string dbPassword = ConfigurationManager.AppSettings["dbPass"];
                string dbPort = ConfigurationManager.AppSettings["dbPort"];

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Dxf\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var argPathDxf = string.Format("Dxf_{0}.dxf", myGuid);
                var cmd =
                    string.Format(
                        "-f DXF \"{0}\" PG:\"dbname='{1}' host='{2}' port='{3}' user='postgres' password='{4}'\" -nlt LINESTRING -sql \"select broj as layer, geom from mikro_gradezni_parceli where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom)\"",
                        downloadDirectory + argPathDxf, dbName, dbAddress, dbPort, dbPassword, coordinates);
                //generate DXF

                var fullPath = ConfigurationManager.AppSettings["ogrPath"];
                ProcessStartInfo start = new ProcessStartInfo(fullPath);
                start.Arguments = cmd;
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                string result;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();
                    }
                }
                var result2 = new UserDa().UpdateCounterDxf(userId);
                return !result2 ? "ne" : argPathDxf;
            }
            return "ne";
        }
        public static string GenerateDxfFile(string coordinates)
        {
                string dbName = ConfigurationManager.AppSettings["dbName"];
                string dbAddress = ConfigurationManager.AppSettings["dbServer"];
                string dbPassword = ConfigurationManager.AppSettings["dbPass"];
                string dbPort = ConfigurationManager.AppSettings["dbPort"];

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Dxf\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var argPathDxf = string.Format("Dxf_{0}.dxf", myGuid);
                var cmd =
                    string.Format(
                        "-f DXF \"{0}\" PG:\"dbname='{1}' host='{2}' port='{3}' user='postgres' password='{4}'\" -nlt LINESTRING -sql \"select broj as layer, geom from mikro_gradezni_parceli where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom)\"",
                        downloadDirectory + argPathDxf, dbName, dbAddress, dbPort, dbPassword, coordinates);
                //generate DXF

                var fullPath = ConfigurationManager.AppSettings["ogrPath"];
                ProcessStartInfo start = new ProcessStartInfo(fullPath);
                start.Arguments = cmd;
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                string result;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();
                    }
                }
            
              return  argPathDxf;
  
        }
        public static string GenerateDxfNearby(string coordinates)
        {
            //int counterDxf = new UserDa().CheckCounerDxf(userId);

            //if ( counterDxf > 0)
            //{
                string dbName = ConfigurationManager.AppSettings["dbName"];
                string dbAddress = ConfigurationManager.AppSettings["dbServer"];
                string dbPassword = ConfigurationManager.AppSettings["dbPass"];
                string dbPort = ConfigurationManager.AppSettings["dbPort"];

                var downloadDirectory = HttpRuntime.AppDomainAppPath + "Dxf\\";
                if (!Directory.Exists(downloadDirectory)) Directory.CreateDirectory(downloadDirectory);
                var myGuid = Guid.NewGuid();
                var argPathDxf = string.Format("Dxf_{0}.dxf", myGuid);
                var cmd =
                    string.Format(
                        //"-f DXF \"{0}\" PG:\"dbname='{1}' host='{2}' port='{3}' user='postgres' password='{4}'\" -nlt LINESTRING -sql \"select distinct b.broj as layer, b.geom from mikro_gradezni_parceli a inner join mikro_gradezni_parceli as b ON a.mikro_gradezna_parcela_id!=b.mikro_gradezna_parcela_id where (ST_Touches(st_makevalid(a.geom), st_makevalid(b.geom)) or ST_Intersects(st_makevalid(a.geom), st_makevalid(b.geom))) and st_isvalid(a.geom)=true and st_isvalid(b.geom)=true and a.active=true and a.valid_to='infinity' and a.produkcija=true and b.active=true and b.valid_to='infinity' and b.produkcija=true and a.mikro_gradezna_parcela_id=(SELECT mikro_gradezna_parcela_id FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and st_isvalid(geom)=true and active=true and valid_to='infinity' and produkcija=true) union SELECT broj as layer, geom FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and active=true and valid_to='infinity' and produkcija=true \"",
                       "-f DXF \"{0}\" PG:\"dbname='{1}' host='{2}' port='{3}' user='postgres' password='{4}'\" -nlt LINESTRING -sql \"select '' as layer, query1.mikro_povrsina_za_gradba_id as id, query1.geom from mikro_povrsini_za_gradba as query1, (select distinct b.geom from mikro_gradezni_parceli a inner join mikro_gradezni_parceli as b ON a.mikro_gradezna_parcela_id!=b.mikro_gradezna_parcela_id where (ST_Touches(st_makevalid(a.geom), st_makevalid(b.geom)) or ST_Intersects(st_makevalid(a.geom), st_makevalid(b.geom))) and st_isvalid(a.geom)=true and st_isvalid(b.geom)=true and a.active=true and a.valid_to='infinity' and a.produkcija=true and b.active=true and b.valid_to='infinity' and b.produkcija=true and a.mikro_gradezna_parcela_id=(SELECT mikro_gradezna_parcela_id FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and st_isvalid(geom)=true and active=true and valid_to='infinity' and produkcija=true)  union SELECT  geom FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and active=true and valid_to='infinity' and produkcija=true ) as query2 WHERE ST_Contains(ST_Buffer(query2.geom, 1), query1.geom) and query1.active=true union select distinct b.broj as layer, 0 as id, b.geom from mikro_gradezni_parceli a inner join mikro_gradezni_parceli as b ON a.mikro_gradezna_parcela_id!=b.mikro_gradezna_parcela_id where (ST_Touches(st_makevalid(a.geom), st_makevalid(b.geom)) or ST_Intersects(st_makevalid(a.geom), st_makevalid(b.geom))) and st_isvalid(a.geom)=true and st_isvalid(b.geom)=true and a.active=true and a.valid_to='infinity' and a.produkcija=true and b.active=true and b.valid_to='infinity' and b.produkcija=true and a.mikro_gradezna_parcela_id=(SELECT mikro_gradezna_parcela_id FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and st_isvalid(geom)=true and active=true and valid_to='infinity' and produkcija=true) union SELECT broj as layer,0 as id, geom FROM mikro_gradezni_parceli WHERE ST_Intersects(ST_SetSRID(ST_MakePoint({5}),6316),geom) and active=true and valid_to='infinity' and produkcija=true \"",
                        downloadDirectory + argPathDxf, dbName, dbAddress, dbPort, dbPassword, coordinates);
                //generate DXF

                var fullPath = ConfigurationManager.AppSettings["ogrPath"];
                ProcessStartInfo start = new ProcessStartInfo(fullPath);
                start.Arguments = cmd;
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                string result;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        result = reader.ReadToEnd();
                    }
                }
                return argPathDxf;
                //var result2 = new UserDa().UpdateCounterDxf(userId);
                //return !result2 ? "ne" : argPathDxf;
            //}
            //return "ne";
        }
        public static IDrillDownInfo GetInfo(string coordinates)
        {
            var info = new DrillDownInfoDa().GetInfo(coordinates);
            return info;
        }

        public static string GenerateOpsti(int opstiId)
        {
            var path = new ParceliDa().GetOpsti(opstiId);
            return path;
        }

        public static string GeneratePosebni(int posebniId)
        {
            var path = new ParceliDa().GetPosebni(posebniId);
            return path;
        }

        public static List<ITockiOdInteres> GetPoiList(int userId)
        {
            return new NotifikaciiDa().GetPoiByUser(userId);
        }

        public static bool DeletePoi(int id)
        {
            return new NotifikaciiDa().DeletePoi(id);
        }
        public static List<IStreet> GetAllStreets()
        {
            var streets = new AdresiDa().GetListStreet();
            return streets;
        }
        public static List<IStNumber> GetAllNumbers(string ulica)
        {
            var numbers = new AdresiDa().GetListNumbers(ulica);
            return numbers;
        }
        public static IAdresi SearchAdresa(string ulica, string broj)
        {
            var info = new AdresiDa().SearchAdress(ulica, broj);
            return info;
        }
        public static List<IBiznisInfo> GetBiznisInfo(string coordinates)
        {
            var info = new BiznisInfoDa().GetListBiznisInfo(coordinates);
            return info;
        }
    }
}