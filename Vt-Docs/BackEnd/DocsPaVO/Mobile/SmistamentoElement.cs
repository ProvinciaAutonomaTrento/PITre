using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.trasmissione;
using System.Globalization;

namespace DocsPaVO.Mobile
{
    public class SmistamentoElement
    {
        public string Id
        {
            get;
            set;
        }

        public string Segnatura
        {
            get;
            set;
        }

        public DateTime DataDoc
        {
            get;
            set;
        }

        public string Mittente
        {
            get;
            set;
        }

        public string Destinatario
        {
            get;
            set;
        }

        public string RagioneTrasmissione
        {
            get;
            set;
        }

        public string Oggetto
        {
            get;
            set;
        }

        public string NoteGenerali
        {
            get;
            set;
        }

        public string NoteIndividuali
        {
            get;
            set;
        }

        public string IdTrasmissione
        {
            get;
            set;
        }

        public string IdTrasmissioneUtente
        {
            get;
            set;
        }

        public string IdTrasmissioneSingola
        {
            get;
            set;
        }

        public string Extension
        {
            get;
            set;
        }

        public bool HasWorkflow
        {
            get;
            set;
        }

        public static SmistamentoElement BuildInstance(infoToDoList input, Trasmissione trasm, UserInfo userInfo)
        {
            SmistamentoElement res = new SmistamentoElement();
            res.DataDoc = toDate(input.dataDoc);
            res.Id = input.sysIdDoc;
            res.Mittente = input.utenteMittente;
            res.NoteGenerali = trasm.noteGenerali;
            res.Oggetto = input.oggetto;
            res.IdTrasmissione = trasm.systemId;
            res.Extension = input.cha_img;

            foreach (TrasmissioneSingola temp in trasm.trasmissioniSingole)
            {
                res.IdTrasmissioneSingola = temp.systemId;
                foreach (TrasmissioneUtente tempUt in temp.trasmissioneUtente)
                {
                    if (userInfo.UserId.Equals(tempUt.utente.userId))
                    {
                        res.RagioneTrasmissione = temp.ragione.descrizione;
                        res.NoteIndividuali = temp.noteSingole;
                        res.IdTrasmissioneUtente = tempUt.systemId;
                        if ("W".Equals(temp.ragione.tipo)) res.HasWorkflow = true;
                    }
                }
            }
            res.Segnatura = input.infoDoc;
            return res;
        }

        private static DateTime toDate(string date)
        {
            string[] formats = {"dd/MM/yyyy",
                                "dd/MM/yyyy HH:mm:ss",
								"dd/MM/yyyy h:mm:ss",
								"dd/MM/yyyy h.mm.ss",
								"dd/MM/yyyy HH.mm.ss"};
            return DateTime.ParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        }

    }
}
