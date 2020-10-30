using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Api.Dal.Abstract;
using Api.ViewModels.Concrete;
using Navigator.Dal;
using NLog;

namespace Api.Dal.Concrete
{
    public class OpfatDa : IOpfatDa
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<Opfat> GetAll()
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
                        "select mikro_opfat_id, opfat_ime from mikro_opfat where active=true and valid_to='infinity';";

                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<Opfat> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();
            return list;
        }
        private Opfat CreateObject(DataRow dr)
        {
            var item = new Opfat
            {
                Id = int.Parse(dr["mikro_opfat_id"].ToString()),
                Ime = dr["opfat_ime"].ToString()
            };
            return item;
        }
    }
}