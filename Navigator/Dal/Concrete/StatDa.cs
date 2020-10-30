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

namespace Navigator.Dal.Concrete
{
    public class StatDa : IStatDal
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public IStat GetStat(int opfatId)
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
                    "select sum(bruto_povrsina) as bruto_povrsina, sum(povrsina) as povrsina,sum(povrsina_presmetana) as povrsina_presmetana,avg(koef_iskoristenost) as koef_iskoristenost, SUM(CASE WHEN klasa_namena ILIKE 'Д%' THEN povrsina ELSE 0 END) AS zeleni, SUM(CASE WHEN klasa_namena ILIKE 'Д%' THEN povrsina_presmetana ELSE 0 END) AS zeleni_prezetani, SUM(CASE WHEN klasa_namena NOT ILIKE 'Д%' THEN povrsina ELSE 0 END) AS gradezni, SUM(CASE WHEN klasa_namena NOT ILIKE 'Д%' THEN povrsina_presmetana ELSE 0 END) AS gradezni_presmetana from mikro_gradezni_parceli where fk_mikro_opfat=:id;";

                Db.CreateParameterFunc(cmd, "@id", opfatId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            IStat list = CreateStatObject(dt.Rows[0]);

            return list;
        }
        private static IStat CreateStatObject(DataRow dr)
        {
            var stat = new Stat
            {
                BrutoPovrsina = (dr["bruto_povrsina"] == null || dr["bruto_povrsina"].ToString() == "")
                                   ? (double?)null
                          : double.Parse(dr["bruto_povrsina"].ToString()),
                PovrsinaGradezniParceli = (dr["povrsina"] == null || dr["povrsina"].ToString() == "")
                                    ? (double?)null
                           : double.Parse(dr["povrsina"].ToString()),
                PovrsinaPresmetana = (dr["povrsina_presmetana"] == null || dr["povrsina_presmetana"].ToString() == "")
                                    ? (double?)null
                           : double.Parse(dr["povrsina_presmetana"].ToString()),
                KoeficientIskeristenost = (dr["koef_iskoristenost"] == null || dr["koef_iskoristenost"].ToString() == "")
                                    ? (double?)null
                           : Math.Round(double.Parse(dr["koef_iskoristenost"].ToString()), 2),
                ZelenaPovrsina = (dr["zeleni"] == null || dr["zeleni"].ToString() == "")
                                ? (double?)null
                           : double.Parse(dr["zeleni"].ToString()),
                ZelenaPovrsinaPresmetana = (dr["zeleni_prezetani"] == null || dr["zeleni_prezetani"].ToString() == "")
                                ? (double?)null
                           : double.Parse(dr["zeleni_prezetani"].ToString()),
                GradeznaPovrsina = (dr["gradezni"] == null || dr["gradezni"].ToString() == "")
                                ? (double?)null
                           : double.Parse(dr["gradezni"].ToString()),
                GradeznaPovrsinaPresmetana = (dr["gradezni_presmetana"] == null || dr["gradezni_presmetana"].ToString() == "")
                                ? (double?)null
                           : double.Parse(dr["gradezni_presmetana"].ToString()),
            };
            return stat;
        }
        public List<INamena> GetListNamena(int opfatId)
        {
            DataTable dt;

            try
            {
                var cmd = Db.CreateCommand();
                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }
                cmd.CommandText = " select distinct klasa_namena as klasa_namena from mikro_gradezni_parceli where fk_mikro_opfat=:id;";

                Db.CreateParameterFunc(cmd, "@id", opfatId, NpgsqlDbType.Integer);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            List<INamena> list = (from DataRow dr in dt.Rows select CreateNamenaObject(dr)).ToList();

            return list;
        }
        private static INamena CreateNamenaObject(DataRow dr)
        {
            var namena = new Namena
            {
                id = dr["klasa_namena"].ToString(),
                text = dr["klasa_namena"].ToString()

            };
            return namena;
        }
        public IStat GetStatNamena(string namena, int opfatId)
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

                    "select sum(bruto_povrsina) as bruto_povrsina, sum(povrsina) as povrsina,sum(povrsina_presmetana) as povrsina_presmetana,avg(koef_iskoristenost) as koef_iskoristenost, SUM(CASE WHEN klasa_namena ILIKE 'Д%' THEN povrsina ELSE 0 END) AS zeleni, SUM(CASE WHEN klasa_namena ILIKE 'Д%' THEN povrsina_presmetana ELSE 0 END) AS zeleni_prezetani,SUM(CASE WHEN klasa_namena NOT ILIKE 'Д%' THEN povrsina ELSE 0 END) AS gradezni, SUM(CASE WHEN klasa_namena NOT ILIKE 'Д%' THEN povrsina_presmetana ELSE 0 END) AS gradezni_presmetana from mikro_gradezni_parceli where fk_mikro_opfat=:id and klasa_namena=:namena";

                Db.CreateParameterFunc(cmd, "@id", opfatId, NpgsqlDbType.Integer);
                Db.CreateParameterFunc(cmd, "@namena", namena, NpgsqlDbType.Text);
                dt = Db.ExecuteSelectCommand(cmd);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception(ex.Message);
            }

            IStat list = CreateStatNamenaObject(dt.Rows[0]);

            return list;
        }
        private static IStat CreateStatNamenaObject(DataRow dr)
        {
            var stat = new Stat
            {

                BrutoPovrsina = (dr["bruto_povrsina"] == null || dr["bruto_povrsina"].ToString() == "")
                                    ? (double?)null
                           : double.Parse(dr["bruto_povrsina"].ToString()),
                PovrsinaGradezniParceli = (dr["povrsina"] == null || dr["povrsina"].ToString() == "")
                                    ? (double?)null
                           : double.Parse(dr["povrsina"].ToString()),
                PovrsinaPresmetana = (dr["povrsina_presmetana"] == null || dr["povrsina_presmetana"].ToString() == "")
                                    ? (double?)null
                           : double.Parse(dr["povrsina_presmetana"].ToString()),
                KoeficientIskeristenost = (dr["koef_iskoristenost"] == null || dr["koef_iskoristenost"].ToString() == "")
                                    ? (double?)null
                           : Math.Round(double.Parse(dr["koef_iskoristenost"].ToString()), 2),
                ZelenaPovrsina = (dr["zeleni"] == null || dr["zeleni"].ToString() == "")
                                    ? (double?)null
                            : double.Parse(dr["zeleni"].ToString()),
                ZelenaPovrsinaPresmetana = (dr["zeleni_prezetani"] == null || dr["zeleni_prezetani"].ToString() == "")
                                ? (double?)null
                           : double.Parse(dr["zeleni_prezetani"].ToString()),
                GradeznaPovrsina = (dr["gradezni"] == null || dr["gradezni"].ToString() == "")
                                ? (double?)null
                           : double.Parse(dr["gradezni"].ToString()),
                GradeznaPovrsinaPresmetana = (dr["gradezni_presmetana"] == null || dr["gradezni_presmetana"].ToString() == "")
                                ? (double?)null
                           : double.Parse(dr["gradezni_presmetana"].ToString())
            };
            return stat;
        }
    }
}