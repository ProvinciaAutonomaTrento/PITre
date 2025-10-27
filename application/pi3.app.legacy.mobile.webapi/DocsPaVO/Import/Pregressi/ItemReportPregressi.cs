// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace DocsPaVO.Import.Pregressi
{
    [Serializable()]
    [DataContract]
    public class ItemReportPregressi
    {
        [DataMember]
        public string systemId { get; set; } = string.Empty;
        [DataMember]
        public string idPregresso { get; set; } = string.Empty;
        [DataMember]
        public string idRegistro { get; set; } = string.Empty;
        [DataMember]
        public string codRegistro { get; set; } = string.Empty;
        [DataMember]
        public string idDocumento { get; set; } = string.Empty;
        [DataMember]
        public string idUtente { get; set; } = string.Empty;
        [DataMember]
        public string idRuolo { get; set; } = string.Empty;
        [DataMember]
        public string tipoOperazione { get; set; } = string.Empty;
        [DataMember]
        public string data { get; set; } = string.Empty;
        [DataMember]
        public string errore { get; set; } = string.Empty;
        //L'esito può essere S: successo; W: warning; E: error
        [DataMember]
        public string esito { get; set; } = string.Empty;
        //public bool esito = false;
        //Nel caso in cui si stia facendo l'import di documenti non protocollati il campo idNumProtocolloExcel viene utilizzato per id del vecchio documento
        [DataMember]
        public string idNumProtocolloExcel { get; set; } = string.Empty;
        [DataMember]
        public string ordinale { get; set; } = string.Empty;

        [DataMember]
        public string rigaExcel { get; set; } = string.Empty;

        [DataMember]
        public string tipoProtocollo { get; set; } = string.Empty;

        //Campi non inclusi nel processo di controllo/validazione
        [DataMember]
        public string cod_rf { get; set; } = string.Empty;
        [DataMember]
        public string cod_oggetto { get; set; } = string.Empty;
        [DataMember]
        public string oggetto { get; set; } = string.Empty;
        [DataMember]
        public string cod_corrispondenti { get; set; } = string.Empty;
        [DataMember]
        public string corrispondenti { get; set; } = string.Empty;
        [DataMember]
        public string pathname { get; set; } = string.Empty;
        [DataMember]
        public string adl { get; set; } = string.Empty;
        [DataMember]
        public string note { get; set; } = string.Empty;
        [DataMember]
        public string cod_modello_trasm { get; set; } = string.Empty;
        //Nome della tipologia
        [DataMember]
        public string tipo_documento { get; set; } = string.Empty;

        //PROVA ANDREA
        //Campi per fascicolazione
        [DataMember]
        public string[] ProjectCodes { get; set; } = null;
        [DataMember]
        public string ProjectDescription { get; set; } = string.Empty;
        //Sottofascicolo
        [DataMember]
        public string FolderDescrition { get; set; } = string.Empty;
        [DataMember]
        public string NodeCode { get; set; } = string.Empty;
        //Nome del titolario
        [DataMember]
        public string Titolario { get; set; } = string.Empty;
        //Codice del registro del fascicolo
        [DataMember]
        public string codiceRegistroFascicolo { get; set; } = string.Empty;
        //END PROVA ANDREA

        [XmlArray()]
        [XmlArrayItem(typeof(Allegati))]
        [DataMember]
        public List<Allegati> Allegati { get; set; }

        public ItemReportPregressi()
        { this.Allegati = new List<Allegati>(); }

        [DataMember]
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
