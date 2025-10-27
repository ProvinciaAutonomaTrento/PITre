// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.NuovoLottoAttivo
{
    public class NuovoLottoAttivoCommand: NuovaFatturaRequest, IRequest<NuovoLottoAttivoCommandResponse>
    {
        
    }

    public class NuovoLottoAttivoCommandResponse: GetDocumentResponse
    {
	}
}