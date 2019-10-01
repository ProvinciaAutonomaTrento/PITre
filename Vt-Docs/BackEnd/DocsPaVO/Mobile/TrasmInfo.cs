using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.trasmissione;
using System.Globalization;

namespace DocsPaVO.Mobile
{
    public class TrasmInfo
    {
        public string IdTrasm
        {
            get;
            set; 
        }

        public string IdTrasmUtente
        {
            get;
            set;
        }

        public string NoteGenerali
        {
            get; 
            set;
        }

        public string Mittente
        {
            get; 
            set; 
        }

        public DateTime Data
        {
            get; 
            set; 
        }

        public string Ragione
        {
            get; 
            set; 
        }

        public string NoteIndividuali
        {
            get; 
            set;
        }

        public bool HasWorkflow
        {
            get;
            set;
        }

        public bool Accettata
        {
            get;
            set;
        }

        public bool Rifiutata
        {
            get;
            set;
        }

        public static TrasmInfo buildInstance(Trasmissione input,UserInfo userInfo,DocsPaVO.utente.Utente delegato)
        {
            TrasmInfo trasmInfo = new TrasmInfo();
            trasmInfo.IdTrasm = input.systemId;
            trasmInfo.NoteGenerali = input.noteGenerali;
            trasmInfo.Mittente = input.utente.descrizione;
            if (delegato != null)
            {
                trasmInfo.Mittente = delegato.descrizione + " sostituto di " + input.utente.descrizione;
            }
            foreach (TrasmissioneSingola temp in input.trasmissioniSingole)
            {
                foreach(TrasmissioneUtente tempUt in temp.trasmissioneUtente){
                    if(userInfo.UserId.Equals(tempUt.utente.userId)){
                        trasmInfo.Ragione=temp.ragione.descrizione;
                        trasmInfo.NoteIndividuali = temp.noteSingole;
                        trasmInfo.IdTrasmUtente = tempUt.systemId;
                        if (!String.IsNullOrEmpty(tempUt.dataAccettata)) trasmInfo.Accettata=true;
                        if (!String.IsNullOrEmpty(tempUt.dataRifiutata)) trasmInfo.Rifiutata = true;                            
                            
                        if ("W".Equals(temp.ragione.tipo)) trasmInfo.HasWorkflow = true;
                    }
                }
            }
            trasmInfo.Data = toDate(input.dataInvio);
            return trasmInfo;
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
