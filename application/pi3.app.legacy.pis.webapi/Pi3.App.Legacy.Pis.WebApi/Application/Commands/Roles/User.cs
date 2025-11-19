// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles
{
    public class User
    {
        /// <summary>
        /// Id dell'utente (idPeople)
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        ///Codice dell'utente
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione con nome e cognome
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Nome 
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Cognome
        /// </summary>
        public string Surname
        {
            get;
            set;
        }

        /// <summary>
        /// Codice fiscale/p.iva
        /// </summary>
        public string NationalIdentificationNumber
        {
            get;
            set;
        }
    }
}
