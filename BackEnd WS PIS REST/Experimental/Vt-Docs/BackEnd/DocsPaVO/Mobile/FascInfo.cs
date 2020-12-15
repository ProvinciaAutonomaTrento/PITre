using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.fascicolazione;
using System.Globalization;

namespace DocsPaVO.Mobile
{
    public class FascInfo
    {
        public string IdFasc
        {
            get;
            set;
        }

        public string Codice
        {
            get;
            set;
        }

        public string Descrizione
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public DateTime DataApertura
        {
            get;
            set;
        }

        public DateTime DataChiusura
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

        public static FascInfo buildInstance(Fascicolo input)
        {
            FascInfo res = new FascInfo();
            res.IdFasc = input.systemID;
            res.AccessRights = input.accessRights;
            int AccInt = 0;
            Int32.TryParse(res.AccessRights, out AccInt);
            if (AccInt > 45) res.CanTransmit = true;

            if (input.noteFascicolo != null && input.noteFascicolo.Length > 0)
            {
                Array.Sort(input.noteFascicolo, new InfoNoteComparer());
                res.Note = input.noteFascicolo[input.noteFascicolo.Length - 1].Testo;
            }
            res.Descrizione = input.descrizione;
            if (!string.IsNullOrEmpty(input.apertura))
            {
                res.DataApertura = toDate(input.apertura);
            }
            if (!string.IsNullOrEmpty(input.chiusura))
            {
                res.DataChiusura = toDate(input.chiusura);
            }
            res.Codice = input.codice;
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
