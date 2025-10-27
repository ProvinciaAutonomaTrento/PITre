// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class Note
    {
        /// <summary>
        /// System id della nota
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione della nota
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Utente proprietario della nota
        /// </summary>
        public Roles.User User
        {
            get;
            set;
        }
    }
}
