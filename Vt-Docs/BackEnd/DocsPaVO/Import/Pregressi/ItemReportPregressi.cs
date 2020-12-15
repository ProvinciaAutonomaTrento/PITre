using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Import.Pregressi
{
    [Serializable()]
    public class ItemReportPregressi
    {
        public string systemId = string.Empty;
        public string idPregresso = string.Empty;
        public string idRegistro = string.Empty;
        public string codRegistro = string.Empty;
        public string idDocumento = string.Empty;
        public string idUtente = string.Empty;
        public string idRuolo = string.Empty;
        public string tipoOperazione = string.Empty;
        public string data = string.Empty;
        public string errore = string.Empty;
        //L'esito può essere S: successo; W: warning; E: error
        public string esito = string.Empty;
        //public bool esito = false;
        //Nel caso in cui si stia facendo l'import di documenti non protocollati il campo idNumProtocolloExcel viene utilizzato per id del vecchio documento
        public string idNumProtocolloExcel = string.Empty;
        public string ordinale = string.Empty;

        public string rigaExcel = string.Empty;

        public string tipoProtocollo = string.Empty;

        //Campi non inclusi nel processo di controllo/validazione
        public string cod_rf = string.Empty;
        public string cod_oggetto = string.Empty;
        public string oggetto = string.Empty;
        public string cod_corrispondenti = string.Empty;
        public string corrispondenti = string.Empty;
        public string pathname = string.Empty;
        public string adl = string.Empty;
        public string note = string.Empty;
        public string cod_modello_trasm = string.Empty;
        //Nome della tipologia
        public string tipo_documento = string.Empty;

        //PROVA ANDREA
        //Campi per fascicolazione
        public string[] ProjectCodes = null;
        public string ProjectDescription = string.Empty;
        //Sottofascicolo
        public string FolderDescrition = string.Empty;
        public string NodeCode = string.Empty;
        //Nome del titolario
        public string Titolario = string.Empty;
        //Codice del registro del fascicolo
        public string codiceRegistroFascicolo = string.Empty;
        //END PROVA ANDREA

        [XmlArray()]
        [XmlArrayItem(typeof(Allegati))]
        public List<Allegati> Allegati { get; set; }

        public ItemReportPregressi()
        { this.Allegati = new List<Allegati>(); }

        public List<String> valoriProfilati { get; set; }

        /// <summary>
        /// Funzione per l'aggiunta di un allegatoItemReportPregressi alla lista
        /// Il campo verrà aggiunto solo se non ne esiste già uno uguale
        /// </summary>
        /// <param name="fieldSettings"></param>
        public void AddAllegatoItemReportPregressi(Allegati allegatoItemReportPreg)
        {
            if (!this.Allegati.Contains(allegatoItemReportPreg))
                this.Allegati.Add(allegatoItemReportPreg);
        }

        /// <summary>
        /// Funzione per la rimozione di un allegatoItemReportPregressi dalla lista
        /// Il campo verrà eliminato solo se esiste nella lista
        /// </summary>
        /// <param name="fieldSettings"></param>
        public void DeleteAllegatoItemReportPregressi(Allegati allegatoItemReportPreg)
        {
            if (this.Allegati.Contains(allegatoItemReportPreg))
                this.Allegati.Remove(allegatoItemReportPreg);
        }


    }
}
