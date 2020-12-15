using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;
using DocsPaVO.Note;
using System.Globalization;
using DocsPaVO.utente;
using DocsPaVO.fascicolazione;

namespace DocsPaVO.Mobile
{
    public class DocInfo
    {
        public DocInfo()
        {
            Destinatari = new List<string>();
            Fascicoli = new List<List<string>>();
        }

        public string IdDoc
        {
            get; 
            set;
        }

        public string OriginalFileName
        {
            get;
            set;
        }

        public string Oggetto
        {
            get; 
            set;
        }

        public string Note
        {
            get; 
            set; 
        }


        public bool IsAcquisito
        {
            get; 
            set;
        }

        public bool HasPreview
        {
            get; 
            set; 
        }

        public DateTime DataDoc
        {
            get;
            set;
        }

        public string TipoProto
        {
            get;
            set;
        }

        public string Segnatura
        {
            get;
            set;
        }

        public DateTime DataProto
        {
            get;
            set;
        }

        public string Mittente
        {
            get;
            set;
        }

        public List<string> Destinatari
        {
            get;
            set;
        }

        public List<List<string>> Fascicoli
        {
            get;
            set;
        }

        public List<string> DescrFasc
        {
            get;
            set;
        }

        public bool IsProtocollato
        {
            get;
            set;
        }

        public bool CanTransmit
        {
            get;
            set;
        }

        public string AccessRights
        {
            get;
            set;
        }

        public string IdDocPrincipale
        { get; set; }

        public static DocInfo buildInstance(SchedaDocumento input, String OriginalFileName, IEnumerable<Fascicolo> fascicoli, bool isAcquisito, bool hasPreview)
        {
            DocInfo res = new DocInfo();
            res.IdDoc = input.systemId;
            res.OriginalFileName = OriginalFileName;
            if (input.noteDocumento != null && input.noteDocumento.Count > 0)
            {
                Array.Sort(input.noteDocumento.ToArray(), new InfoNoteComparer());
                res.Note = input.noteDocumento[input.noteDocumento.Count - 1].Testo;
            }
            if(input.oggetto!=null){
                res.Oggetto = input.oggetto.descrizione;
            }
            res.DataDoc = toDate(input.dataCreazione);
            res.HasPreview = hasPreview;
            res.IsAcquisito = isAcquisito;
            res.TipoProto = input.tipoProto;
            res.AccessRights = input.accessRights;
            int AccInt = 0;
            Int32.TryParse(res.AccessRights, out AccInt);
            if (AccInt>45) res.CanTransmit = true;

            if (input.documentoPrincipale != null) res.IdDocPrincipale = input.documentoPrincipale.docNumber;

            if(input.protocollo!=null){
                res.IsProtocollato = false;
                res.Segnatura = input.protocollo.segnatura;
                if (input.protocollo.daProtocollare == "0")
                {
                    res.IsProtocollato = true;
                    res.DataProto = toDate(input.protocollo.dataProtocollazione);
                }
                if (res.TipoProto.Equals("P"))
                {
                    ProtocolloUscita pu = (ProtocolloUscita)input.protocollo;
                    res.Mittente = formatCorrispondente(pu.mittente);
                    foreach (object temp in pu.destinatari)
                    {
                        res.Destinatari.Add(formatCorrispondente((Corrispondente)temp));
                    }
                }
                if (res.TipoProto.Equals("A"))
                {
                    ProtocolloEntrata pe = (ProtocolloEntrata)input.protocollo;
                    res.Mittente = formatCorrispondente(pe.mittente);
                }
            }
            if (fascicoli != null)
            {
                foreach (Fascicolo fasc in fascicoli)
                {
                    res.Fascicoli.Add(new List<string> { fasc.codice, fasc.descrizione });
                }
            }
            return res;
        }

        private static DateTime toDate(string date)
        {
            string[] formats = {"dd/MM/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
								"dd/MM/yyyy h:mm:ss",
								"dd/MM/yyyy h.mm.ss",
								"dd/MM/yyyy HH.mm.ss"};
            if (date.Length == 0)
                return DateTime.MinValue ;
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        }

        private static string formatCorrispondente(Corrispondente corr)
        {
            return corr.descrizione;
        }
    }

}
