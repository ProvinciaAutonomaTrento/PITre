using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.utente.RegistroConservazione
{
    /// <summary>
    /// Contiene le informazioni necessarie alla creazione del
    /// summary per le stampe del registro di conservazione,
    /// sia per istanze che per documenti
    /// </summary>
    [Serializable()]
    public class RegistroConservazioneSummary
    {

        public string descrizione { get; set; }     //I=descrizione, D=oggetto
        public DateTime creationDate { get; set; }  //I=data apertura, D=data creazione
        public DateTime invioDate { get; set; }     //I=data invio CS
        public string codiceFascicolo { get; set; } //D=codice fascicolo
        public string tipoFile { get; set; }        //D=tipo file
        public string numDoc { get; set; }          //I=numero documenti, D=numero allegati
        public string fileDim { get; set; }         //I=dim.complessive, D=dim.file

    }
}
