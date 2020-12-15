using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.amministrazione
{
    [Serializable()]
    public class AlertConservazione
    {
        public string idAmm;

        public string chaLeggibilitaScadenza;
        public string scadenzaTermine;
        public string scadenzaTolleranza;

        public string chaLeggibilitaMaxDoc;
        public string percentualeMaxDoc;

        public string chaSingoloDoc;
        public string maxOperSingoloDoc;
        public string periodoSingoloDoc;

        public string chaDownload;
        public string maxOperDownload;
        public string periodoDownload;

        public string chaSfoglia;
        public string maxOperSfoglia;
        public string periodoSfoglia;

        public string serverSMTP;
        public string portaSMTP;
        public string chaSSL;
        public string userID;
        public string pwd;
        public string fromField;
        public string toField;

    }
}
