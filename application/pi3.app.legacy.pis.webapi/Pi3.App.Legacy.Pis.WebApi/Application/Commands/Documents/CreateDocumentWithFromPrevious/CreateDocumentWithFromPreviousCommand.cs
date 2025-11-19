// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithFromPrevious
{
    public class CreateDocumentWithFromPreviousCommand : IRequest<CreateDocumentWithFromPreviousCommandResponse>
    {
        /// <summary>
        /// Nel caso di protocollo specificare il registro
        /// </summary>
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Documento che si vuole creare
        /// </summary>
        public Document Document
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'RF in cui si vuole protocollare (opzionale)
        /// </summary>
        public string CodeRF
        {
            get;
            set;
        }

        public bool FromPregressi 
        { 
            get; 
            set; 
        }
    }

    public class CreateDocumentWithFromPreviousCommandResponse
    {
        public Document Document { get; set; }

        public CreateDocumentWithFromPreviousResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Ex{ get; set; }
    }
    public enum CreateDocumentWithFromPreviousResponseCode { OK, SYSTEM_ERROR , PIS_ERROR}


}
