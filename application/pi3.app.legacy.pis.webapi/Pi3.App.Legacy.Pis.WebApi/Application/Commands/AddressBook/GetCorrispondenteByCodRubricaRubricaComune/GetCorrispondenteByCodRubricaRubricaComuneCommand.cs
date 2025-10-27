// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.GetCorrispondenteByCodRubricaRubricaComune
{
    public class GetCorrispondenteByCodRubricaRubricaComuneCommand : IRequest<GetCorrispondenteByCodRubricaRubricaComuneCommandResponse>
    {
        public InfoUtente InfoUtente{ get; set; }
        public string Codice{ get; set; }
    }

    public class GetCorrispondenteByCodRubricaRubricaComuneCommandResponse
    {
        public DocsPaVO.utente.Corrispondente Output{ get; set; }
    }
}
