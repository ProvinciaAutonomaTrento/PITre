// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Invoices.NuovoLotto
{
    public class NuovoLottoCommand : NuovaFatturaRequest, IRequest<NuovoLottoCommandResponse>
    {   

    }

    public class NuovoLottoCommandResponse: GetDocumentResponse
    {
	}
}