﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Navigator.Dal.Abstract;
using Navigator.Models.Abstract;
using Navigator.Models.Concrete;
using NpgsqlTypes;

namespace Navigator.Dal.Concrete
{
    public class KatParceliDa : IKatParceliDal
    {
        public List<IKatastarskaParcela> Get(string searchedText)
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
                    @"SELECT gid, text, lokacija, ST_AsGeoJSON(geom) as geojson FROM kat_tocki WHERE text LIKE :searchedText";
                Db.CreateParameterFunc(cmd, "@searchedText", '%' + searchedText + '%', NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw new Exception(ex.Message);
            }

            List<IKatastarskaParcela> list = (from DataRow dr in dt.Rows select CreateObject(dr)).ToList();

            return list;
        }

        private static IKatastarskaParcela CreateObject(DataRow dr)
        {
            var p = new KatastarskaParcela
            {
                Id = int.Parse(dr["gid"].ToString()),
                GeoJson = dr["geojson"].ToString(),
                Name = dr["text"].ToString(),
                Location = dr["lokacija"].ToString()
            };
            return p;
        }
    }
}