using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    public class OutputResponseMarca
    {
        /// <summary>
        /// descrizione dell'eventuale errore nella verifica e/o rischiesta della marca temporale
        /// </summary>
        public string descrizioneErrore = string.Empty;
        /// <summary>
        /// stringa rappresentante il timestamp
        /// </summary>
        public string docm = string.Empty;
        /// <summary>
        /// timestamp in formato data
        /// </summary>
        public string docm_date = string.Empty;
        /// <summary>
        /// data scadenza marca
        /// </summary>
        public string dsm = string.Empty;
        /// <summary>
        /// data a partire dalla quale la marca è valida
        /// </summary>
        public string fromDate = string.Empty;
        /// <summary>
        /// esito della richiesta della marca
        /// </summary>
        public string esito = string.Empty;
        /// <summary>
        /// stringa esadecimale rappresentante l'hash del file al quale la marca è associata
        /// </summary>
        public string fhash = string.Empty;
        /// <summary>
        /// stringa in base64 rappresentante la marca temporale restituita dalla TSA
        /// </summary>
        public string marca = string.Empty;
        /// <summary>
        /// numero seriale della marca temporale
        /// </summary>
        public string sernum = string.Empty;
        /// <summary>
        /// oggetto TSA in formato RFC2253
        /// </summary>
        public TSARFC2253 TSA;
        /// <summary>
        /// oggetto contenente le informazioni in chiaro della marca
        /// </summary>
        public Marca DecryptedTSR;
        /// <summary>
        /// numero del documento marcato temporalmente
        /// </summary>
        public string timestampedDoc = string.Empty;
        /// <summary>
        /// numero seriale del certificato di firma
        /// </summary>
        public string snCertificato = string.Empty;
        /// <summary>
        /// algoritmo di firma del certificato della TSA
        /// </summary>
        public string algCertificato = string.Empty;
        /// <summary>
        /// algoritmo di hash
        /// </summary>
        public string algHash = string.Empty;
    }

    /// <summary>
    /// oggetto TSA in formato RFC2253 navigabile nei singoli campi
    /// </summary>
    public class TSARFC2253
    {
        /// <summary>
        /// nome della TSA in formato RFC2253
        /// </summary>
        public string TSARFC2253Name = string.Empty;
        /// <summary>
        /// Nome comune
        /// </summary>
        public string CN = string.Empty;
        /// <summary>
        /// nome dell'unità organizzativa
        /// </summary>
        public string OU = string.Empty;
        /// <summary>
        /// nome dell'organizzazione
        /// </summary>
        public string O = string.Empty;
        /// <summary>
        /// sigla della nazione
        /// </summary>
        public string C = string.Empty;
    }

    /// <summary>
    /// contenuto in chiaro della marca temporale
    /// </summary>
    public class Marca
    {
        /// <summary>
        /// contenuto binario della marca, tipicamente sarà rappresentato come un HTML
        /// e conterrà come informazioni il timestamp e l'hash del file al quale è associata
        /// la marca
        /// </summary>
        public byte[] content;
        /// <summary>
        /// dimensione dell'array di byte contenente la marca
        /// </summary>
        public int length;
        /// <summary>
        /// MIME type del contenuto della marca che sarà tipicamente un HTML
        /// </summary>
        public string contentType;
    }
}
