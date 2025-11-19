// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook
{
    public class CorrespondentAdvanced : Correspondent
    {

        /// <summary>
        /// Lista delle email associate al corrispondente con il dettaglio della nota
        /// </summary>
        public List<CorrespondentEmail> EmailsDetailed
        {
            get;
            set;
        }

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
