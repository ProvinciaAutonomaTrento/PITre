// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SearchDocEvents
{
    public class SearchDocEventsCommand: IRequest<SearchDocEventsCommandResponse>
    {
        /// <summary>
        /// Data di inizio ricerca. Obbligatorio
        /// </summary>
        public string FromDate
        {
            get; set;
        }

        /// <summary>
        /// Data fine ricerca. Opzionale
        /// </summary>
        public string ToDate { get; set; }

        /// <summary>
        /// Lista degli eventi da ricercare.
        /// </summary>
        public string[] Events { get; set; }

        /// <summary>
        /// Lista delle tipologie sulle quali effettuare la ricerca. Opzionale
        /// </summary>
        public Template[] Templates { get; set; }

        /// <summary>
        /// Lista di parametri vari ed eventuali si vorranno aggiungere alla ricerca. Opzionale
        /// </summary>
        public FieldLite[] OtherParams { get; set; }
    }

    public class SearchDocEventsCommandResponse
    {
        public DocEventInfo[] DocEvents
        {
            get;
            set;
        }
        public string TotalEvents { get; set; }
        public string ErrorMessage { get; set; }
        public SearchDocEventsResponseCode Code { get; set; }
    }

    public enum SearchDocEventsResponseCode { OK, SYSTEM_ERROR }
}