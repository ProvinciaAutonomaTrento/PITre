﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile
{
    public class LibroFirmaElement
    {

        public string IdElemento
        {
            get;
            set;
        }

        public string StatoFirma
        {
            get;
            set;
        }

        public string TipoFirma
        {
            get;
            set;
        }

        public string Modalita
        {
            get;
            set;
        }

        public string DataInserimento
        {
            get;
            set;
        }

        public string RuoloProponente
        {
            get;
            set;
        }

        public string UtenteProponente
        {
            get;
            set;
        }

        public DocInfo InfoDocumento
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public string IdIstanzaProcesso
        {
            get;
            set;
        }

        public string MotivoRespingimento
        {
            get;
            set;
        }

        public string IdIstanzaPasso
        {
            get;
            set;
        }

        public string IdTrasmSingola
        {
            get;
            set;
        }

        public string IdUtenteTitolare
        {
            get;
            set;
        }

        public string IdRuoloTitolare
        {
            get;
            set;
        }

        public string DataAccettazione
        {
            get;
            set;
        }

        public long FileSize
        {
            get;
            set;
        }

        public string ErroreFirma
        {
            get;
            set;
        }

        public bool IsAllegato
        {
            get;
            set;
        }

        public ElementType Tipo
        {
            get;
            set;
        }

        //Id del documento
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo di Firma se già firmato
        /// </summary>
        public TipoFirma TipoFirmaFileOriginale
        {
            get;
            set;
        }

        public static LibroFirmaElement BuildInstance(DocsPaVO.LibroFirma.ElementoInLibroFirma element)
        {
            LibroFirmaElement el = new LibroFirmaElement();

            el.IdElemento = element.IdElemento;
            el.StatoFirma = element.StatoFirma.ToString();
            el.TipoFirma = element.TipoFirma;
            el.Modalita = element.Modalita;
            el.DataInserimento = element.DataInserimento;
            el.RuoloProponente = element.RuoloProponente.descrizione;
            el.UtenteProponente = element.UtenteProponente.descrizione;
            el.IdUtenteTitolare = !string.IsNullOrEmpty(element.IdUtenteTitolare) ? element.IdUtenteTitolare : string.Empty;
            el.IdRuoloTitolare = !string.IsNullOrEmpty(element.IdRuoloTitolare) ? element.IdRuoloTitolare : string.Empty;
            el.InfoDocumento = new DocInfo() { IdDoc = element.InfoDocumento.Docnumber, Oggetto = element.InfoDocumento.Oggetto, IdDocPrincipale = element.InfoDocumento.IdDocumentoPrincipale, DataDoc= toDate(element.InfoDocumento.DataCreazione), DataProto= toDate(element.InfoDocumento.DataProtocollo) };
            
            el.Note = element.Note;
            el.IdIstanzaProcesso = element.IdIstanzaProcesso;
            el.MotivoRespingimento = element.MotivoRespingimento;
            el.IdIstanzaPasso = element.IdIstanzaPasso;
            el.IdTrasmSingola = element.IdTrasmSingola;
            el.DataAccettazione = element.DataAccettazione;
            el.FileSize = element.FileSize;
            el.ErroreFirma = element.ErroreFirma;
            el.IsAllegato = !string.IsNullOrEmpty(element.InfoDocumento.IdDocumentoPrincipale) ? true : false;
            el.Tipo = ElementType.DOCUMENTO;
            el.Id = element.InfoDocumento.Docnumber;

            switch(element.TipoFirmaFile)
            {
                case "N":
                case "E":
                case "TSD":
                case "XADES":
                case "TE":
                case "XE":
                    el.TipoFirmaFileOriginale = Mobile.TipoFirma.NESSUNA_FIRMA;
                    break;
                case "P":
                case "PE":
                    el.TipoFirmaFileOriginale = Mobile.TipoFirma.PADES;
                    break;
                case "C":
                case "CE":
                    el.TipoFirmaFileOriginale = Mobile.TipoFirma.CADES;
                    break;
            }

            return el;
        }

        private static DateTime toDate(string date)
        {
            string[] formats = {"dd/MM/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
								"dd/MM/yyyy h:mm:ss",
								"dd/MM/yyyy h.mm.ss",
								"dd/MM/yyyy HH.mm.ss"};
            if (date.Length == 0)
                return DateTime.MinValue;
            return DateTime.ParseExact(date, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
        }
    }
}