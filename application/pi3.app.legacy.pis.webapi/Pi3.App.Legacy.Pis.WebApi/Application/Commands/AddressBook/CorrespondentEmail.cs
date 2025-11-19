// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook
{
    public class CorrespondentEmail
    {
        /// <summary>
        /// Indirizzo email della casella
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Note associate alla casella
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Flag mail principale: 1 mail principale, 0 mail secondaria
        /// </summary>
        public string Main { get; set; }

        /// <summary>
        /// Lista di informazioni aggiuntive
        /// </summary>
        public List<FieldLite> OtherInfos
        {
            get;
            set;
        }
    }
}
