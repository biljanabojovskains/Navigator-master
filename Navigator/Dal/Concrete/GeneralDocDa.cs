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
    public class GeneralDocDa :IGeneralDocDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public bool InsertGeneralDocument(int fkParcela, string path)
        {
            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                cmd.CommandText =
                 "insert into general_doc(fk_parcela, path, dup, geom)  select :fk_par, :pat, mo.opfat_ime, mg.geom from mikro_opfat as mo left join mikro_gradezni_parceli as mg on mo.mikro_opfat_id = mg.fk_mikro_opfat where mg.mikro_gradezna_parcela_id=:fk_par ;";
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
        public List<IGeneralDoc> GetGeneralDocuments(string coordinates)
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

                   "select *, ST_AsGeoJSON(geom) as geojson from general_doc g where ST_Intersects(g.geom, (select geom as geo from mikro_gradezni_parceli where active=true and valid_to='infinity' and ST_Intersects(ST_SetSRID(ST_MakePoint(" + coordinates + "),6316),geom)))";
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }
            List<IGeneralDoc> list = (from DataRow dr in dt.Rows select CreateDocObject(dr)).ToList();
            return list;
        }

        private IGeneralDoc CreateDocObject(DataRow dr)
        {
            var documents = new GeneralDoc
            {
                Id = int.Parse(dr["doc_id"].ToString()),
                FkParcela = int.Parse(dr["fk_parcela"].ToString()),
                Path = dr["path"].ToString(),
                Dup = dr["dup"].ToString(),
                GeoJson = dr["geojson"].ToString(),
            };
            return documents;
        }
    }
}