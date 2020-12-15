using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.fascicolazione;
using DocsPaVO.documento;

namespace DocsPaVO.Mobile
{
    public class RicercaElement
    {
        public string Id
        {
            get;
            set;
        }

        public ElementType Tipo
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public string Oggetto
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

        public string Extension
        {
            get;
            set;
        }
                
        public string Data
        { get; set; }

        public static RicercaElement buildInstance(Fascicolo input)
        {
            RicercaElement res = new RicercaElement();
            res.Id = input.systemID;
            res.Tipo = ElementType.FASCICOLO;
            res.Oggetto = input.descrizione;
            res.Data = input.apertura;
            if (input.noteFascicolo != null && input.noteFascicolo.Length > 0)
            {
                Array.Sort(input.noteFascicolo, new InfoNoteComparer());
                res.Note = input.noteFascicolo[input.noteFascicolo.Length - 1].Testo;
            }
            if (!string.IsNullOrWhiteSpace(input.codice))
                res.Segnatura = input.codice;

            return res;
        }

        public static RicercaElement buildInstance(Folder input)
        {
            RicercaElement res = new RicercaElement();
            res.Id = input.systemID;
            res.Tipo = ElementType.FOLDER;
            res.Oggetto = input.descrizione;
            res.Note = null;
            res.Data = input.dtaApertura;
            if (!string.IsNullOrWhiteSpace(input.codicelivello))
                res.Segnatura = input.codicelivello;
            return res;
        }

        public static RicercaElement buildInstance(InfoDocumento input)
        {
            RicercaElement res = new RicercaElement();
            res.Id = input.idProfile;
            res.Tipo = ElementType.DOCUMENTO;
            res.Oggetto = input.oggetto;
            res.TipoProto= input.tipoProto;
            res.Segnatura = input.segnatura;
            res.Extension = input.acquisitaImmagine;
            res.Data = input.dataApertura;
            if (!string.IsNullOrWhiteSpace(input.noteCestino))
                res.Note = input.noteCestino;
            return res;
        }
    }
}
