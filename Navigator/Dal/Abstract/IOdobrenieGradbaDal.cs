using Navigator.Models.Abstract;
using System;
using System.Collections.Generic;

namespace Navigator.Dal.Abstract
{
    public interface IOdobrenieGradbaDal
    {
        bool Insert(int fkParcela, string brPredmet, string tipBaranje, string sluzbenik, DateTime datumBaranja, DateTime datumIzdavanja, DateTime datumPravosilno, string investitor, string brKP, string ko, string adresa, string parkingMestaParcela, string parkingMestaGaraza, string katnaGaraza,double iznosKomunalii, string zabeleski, string path );
        bool InsertDocument(int fkParcela, string path);
        List<IOdobrenieGradba> GetOdobrenija(string coordinates);
        List<IOdobrenieGradba> GetDocuments(string coordinates);
    }
}