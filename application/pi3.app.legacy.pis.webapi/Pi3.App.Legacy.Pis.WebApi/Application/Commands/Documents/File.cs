// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class File
    {
        /// <summary>
        /// System id del documento
        /// </summary>
        public string Id
        {
            get;
            set;
        }
        /// <summary>
        /// Descrizione del file
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Contenuto del file
        /// </summary>
        public byte[] Content
        {
            get;
            set;
        }

        /// <summary>
        /// Mime del file
        /// </summary>
        public string MimeType
        {
            get;
            set;
        }

        /// <summary>
        /// Id della versione del file
        /// </summary>
        public string VersionId
        {
            get;
            set;
        }

        /// <summary>
        /// Nome del file
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
