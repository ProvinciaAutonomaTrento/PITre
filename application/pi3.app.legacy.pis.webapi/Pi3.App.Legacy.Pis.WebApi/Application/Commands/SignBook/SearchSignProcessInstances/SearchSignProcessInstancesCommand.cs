// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.SearchSignProcessInstances
{
    public class SearchSignProcessInstancesCommand: SearchRequest, IRequest<SearchSignProcessInstancesCommandResponse>
    {        
    }

    public class SearchSignProcessInstancesCommandResponse
    {
        public SignatureProcessInstance[] SignatureProcessInstances
        {
            get;
            set;
        }
        public int TotalNumber
        {
            get;
            set;
        }
        public string ErrorMessage { get; set; }
        public GetSignProcessInstancesResponseCode Code { get; set; }
    }
    public enum GetSignProcessInstancesResponseCode { OK, SYSTEM_ERROR }
}