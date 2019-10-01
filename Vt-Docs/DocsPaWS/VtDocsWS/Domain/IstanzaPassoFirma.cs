using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain
{
    /// <summary>
    /// IstanzaPassoFirma
    /// </summary>
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class IstanzaPassoFirma
    {
        /// <summary>
        /// System id dell'istanza di passo
        /// </summary>
        [DataMember]
        public string idIstanzaPasso
        {
            get;
            set;
        }
        /// <summary>
        /// System id dell'istanza di processo di firma
        /// </summary>
        [DataMember]
        public string idIstanzaProcesso
        {
            get;
            set;
        }

        /// <summary>
        /// System id del passo
        /// </summary>
        [DataMember]
        public string idPasso
        {
            get;
            set;
        }

        /// <summary>
        /// System id dello stato passo
        /// </summary>
        [DataMember]
        public string statoPasso
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione dello stato passo
        /// </summary>
        [DataMember]
        public string descrizioneStatoPasso
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo firma (Elettronica/Digitale/Pades)
        /// </summary>
        [DataMember]
        public string TipoFirma
        {
            get;
            set;
        }

        /// <summary>
        /// system id del tipo evento associato al passo
        /// </summary>
        [DataMember]
        public string IdTipoEvento
        {
            get;
            set;
        }

        /// <summary>
        /// Codice tipo evento presente anche in DPA_LOG
        /// </summary>
        [DataMember]
        public string CodiceTipoEvento
        {
            get;
            set;
        }

        /// <summary>
        /// Data in cui è stato eseguito il passo
        /// </summary>
        [DataMember]
        public string dataEsecuzione
        {
            get;
            set;
        }

        /// <summary>
        /// Data entro cui poter eseguito il passo
        /// </summary>
        [DataMember]
        public string dataScadenza
        {
            get;
            set;
        }
        /// <summary>
        /// Descriznoe motivo del respingimento
        /// </summary>
        [DataMember]
        public string motivoRespingimento
        {
            get;
            set;
        }

        /// <summary>
        /// Ruolo titolare del passo
        /// </summary>
        [DataMember]
        public string IdRuoloCoinvolto
        {
            get;
            set;
        }

        /// <summary>
        /// Utente titolare del passo
        /// </summary>
        [DataMember]
        public string IdUtenteCoinvolto
        {
            get;
            set;
        }

        /// <summary>
        /// System id della notifica relativa all'avanzamento di passo
        /// </summary>
        [DataMember]
        public string idNotificaEffettuata
        {
            get;
            set;
        }

        /// <summary>
        /// Numero relativo all'ordine del passo
        /// </summary>
        [DataMember]
        public int numeroSequenza
        {
            get;
            set;
        }

        /// <summary>
        /// Note relative al passo
        /// </summary>
        [DataMember]
        public string Note
        {
            get;
            set;
        }
    }
}