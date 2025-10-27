// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class SearchRequest
    {
        /// <summary>
        /// Filtri di ricerca
        /// </summary>
        public Filter[] Filters
        {
            get;
            set;
        }

        /// <summary>
        /// Pagina che si desidera visualizzare
        /// </summary>
        public int? PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Quanti elementi sono presenti nella pagina se la ricerca è paginata
        /// </summary>
        public int? ElementsInPage
        {
            get;
            set;
        }

    }
}
