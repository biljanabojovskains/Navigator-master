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
    public class LegendDa : ILegendDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<ILegenda> Get(int opfatId)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select * from legendi where fk_opfat=:id;";
                Db.CreateParameterFunc(cmd, "@id", opfatId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ILegenda> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        public List<ILegenda> GetLegends(List<int> ids)
        {
            DataTable dt;
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = "select * from legendi where fk_opfat = ANY(:id);";
                Db.CreateParameterFunc(cmd, "@id", ids, NpgsqlDbType.Array | NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<ILegenda> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }

        private ILegenda CreateObject(DataRow dr)
        {
            var item = new Legenda
            {
                Id = int.Parse(dr["legenda_id"].ToString()),
                OpfatId = int.Parse(dr["fk_opfat"].ToString()),
                TipNaPodatokId = int.Parse(dr["fk_tip_na_podatok"].ToString()),
                Path = dr["path"].ToString()
            };
            return item;
        }
    }
}