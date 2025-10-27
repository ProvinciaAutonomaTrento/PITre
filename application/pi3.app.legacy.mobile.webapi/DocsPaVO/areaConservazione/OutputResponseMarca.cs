// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DocsPaVO.areaConservazione
{
    [Serializable()]
    [DataContract]
    public class OutputResponseMarca
    {
        /// <summary>
        /// descrizione dell'eventuale errore nella verifica e/o rischiesta della marca temporale
        /// </summary>
        [DataMember]
        public string descrizioneErrore { get; set; } = string.Empty;
        /// <summary>
        /// stringa rappresentante il timestamp
        /// </summary>
        [DataMember]
        public string docm { get; set; } = string.Empty;
        /// <summary>
        /// timestamp in formato data
        /// </summary>
        [DataMember]
        public string docm_date { get; set; } = string.Empty;
        /// <summary>
        /// data scadenza marca
        /// </summary>
        [DataMember]
        public string dsm { get; set; } = string.Empty;
        /// <summary>
        /// data a partire dalla quale la marca è valida
        /// </summary>
        [DataMember]
        public string fromDate { get; set; } = string.Empty;
        /// <summary>
        /// esito della richiesta della marca
        /// </summary>
        [DataMember]
        public string esito { get; set; } = string.Empty;
        /// <summary>
        /// stringa esadecimale rappresentante l'hash del file al quale la marca è associata
        /// </summary>
        [DataMember]
        public string fhash { get; set; } = string.Empty;
        /// <summary>
        /// stringa in base64 rappresentante la marca temporale restituita dalla TSA
        /// </summary>
        [DataMember]
        public string marca { get; set; } = string.Empty;
        /// <summary>
        /// numero seriale della marca temporale
        /// </summary>
        [DataMember]
        public string sernum { get; set; } = string.Empty;
        /// <summary>
        /// oggetto TSA in formato RFC2253
        /// </summary>
        [DataMember]
        public TSARFC2253 TSA { get; set; }
        /// <summary>
        /// oggetto contenente le informazioni in chiaro della marca
        /// </summary>
        [DataMember]
        public Marca DecryptedTSR { get; set; }
        /// <summary>
        /// numero del documento marcato temporalmente
        /// </summary>
        [DataMember]
        public string timestampedDoc { get; set; } = string.Empty;
        /// <summary>
        /// numero seriale del certificato di firma
        /// </summary>
        [DataMember]
        public string snCertificato { get; set; } = string.Empty;
        /// <summary>
        /// algoritmo di firma del certificato della TSA
        /// </summary>
        [DataMember]
        public string algCertificato { get; set; } = string.Empty;
        /// <summary>
        /// algoritmo di hash
        /// </summary>
        [DataMember]
        public string algHash { get; set; } = string.Empty;
    }

    /// <summary>
    /// oggetto TSA in formato RFC2253 navigabile nei singoli campi
    /// </summary>
    [DataContract]
    public class TSARFC2253
    {
        /// <summary>
        /// nome della TSA in formato RFC2253
        /// </summary>
        [DataMember]
        public string TSARFC2253Name { get; set; } = string.Empty;
        /// <summary>
        /// Nome comune
        /// </summary>
        [DataMember]
        public string CN { get; set; } = string.Empty;
        /// <summary>
        /// nome dell'unità organizzativa
        /// </summary>
        [DataMember]
        public string OU { get; set; } = string.Empty;
        /// <summary>
        /// nome dell'organizzazione
        /// </summary>
        [DataMember]
        public string O { get; set; } = string.Empty;
        /// <summary>
        /// sigla della nazione
        /// </summary>
        [DataMember]
        public string C { get; set; } = string.Empty;
    }

    /// <summary>
    /// contenuto in chiaro della marca temporale
    /// </summary>
    [DataContract]
    public class Marca
    {
        /// <summary>
        /// contenuto binario della marca, tipicamente sarà rappresentato come un HTML
        /// e conterrà come informazioni il timestamp e l'hash del file al quale è associata
        /// la marca
        /// </summary>
        [DataMember]
        public byte[] content { get; set; }
        /// <summary>
        /// dimensione dell'array di byte contenente la marca
        /// </summary>
        [DataMember]
        public int length { get; set; }
        /// <summary>
        /// MIME type del contenuto della marca che sarà tipicamente un HTML
        /// </summary>
        [DataMember]
        public string contentType { get; set; }
    }
}
