using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Domain.FileDoc
{
    [DataContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public class OutputResponseMarca
    {
        //TO DO Struttura OutputResponseMarca

        /// <summary>
        /// descrizione dell'eventuale errore nella verifica e/o rischiesta della marca temporale
        /// </summary>
        [DataMember]
        public string descrizioneErrore
        {
            get;
            set;
        }
        
        /// <summary>
        /// stringa rappresentante il timestamp
        /// </summary>
        [DataMember]
        public string docm
        {
            get;
            set;
        }

        /// <summary>
        /// timestamp in formato data
        /// </summary>
        [DataMember]
        public string docm_date
        {
            get;
            set;
        }
        
        /// <summary>
        /// data scadenza marca
        /// </summary>
        [DataMember]
        public string dsm
        {
            get;
            set;
        }
        
        /// <summary>
        /// data a partire dalla quale la marca è valida
        /// </summary>
        [DataMember]
        public string fromDate
        {
            get;
            set;
        }
        
        /// <summary>
        /// esito della richiesta della marca
        /// </summary>
        [DataMember]
        public string esito
        {
            get;
            set;
        }
        
        /// <summary>
        /// stringa esadecimale rappresentante l'hash del file al quale la marca è associata
        /// </summary>
        [DataMember]
        public string fhash
        {
            get;
            set;
        }
        
        /// <summary>
        /// stringa in base64 rappresentante la marca temporale restituita dalla TSA
        /// </summary>
        [DataMember]
        public string marca
        {
            get;
            set;
        }
        
        /// <summary>
        /// numero seriale della marca temporale
        /// </summary>
        [DataMember]
        public string sernum
        {
            get;
            set;
        }
        
        /// <summary>
        /// oggetto TSA in formato RFC2253
        /// </summary>
        [DataMember]
        public TSARFC2253 TSA
        {
            get;
            set;
        }
        
        /// <summary>
        /// oggetto contenente le informazioni in chiaro della marca
        /// </summary>
        [DataMember]
        public Marca DecryptedTSR
        {
            get;
            set;
        }
        
        /// <summary>
        /// numero del documento marcato temporalmente
        /// </summary>
        [DataMember]
        public string timestampedDoc
        {
            get;
            set;
        }
        
        /// <summary>
        /// numero seriale del certificato di firma
        /// </summary>
        [DataMember]
        public string snCertificato
        {
            get;
            set;
        }
        
        /// <summary>
        /// algoritmo di firma del certificato della TSA
        /// </summary>
        [DataMember]
        public string algCertificato
        {
            get;
            set;
        }
        
        /// <summary>
        /// algoritmo di hash
        /// </summary>
        [DataMember]
        public string algHash
        {
            get;
            set;
        }
        
    }
}