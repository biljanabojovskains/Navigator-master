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
    public class KatOpstiniDa : IKatOpstiniDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<IKatOpstina> GetIntersect(int parcelaId)
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
                    "select kat_opstina_id, kat_opstina_ime from kat_opstini where st_intersects(geom,(select geom from mikro_gradezni_parceli where mikro_gradezna_parcela_id=:id));";
                Db.CreateParameterFunc(cmd, "@id", parcelaId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IKatOpstina> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private IKatOpstina CreateObject(DataRow dr)
        {
            var item = new KatOpstina
            {
                Id = int.Parse(dr["kat_opstina_id"].ToString()),
                Ime = dr["kat_opstina_ime"].ToString()
            };
            return item;
        }
    }
}