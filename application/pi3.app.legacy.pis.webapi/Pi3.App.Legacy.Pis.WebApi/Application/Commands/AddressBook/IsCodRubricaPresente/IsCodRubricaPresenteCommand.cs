// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.IsCodRubricaPresente
{
    public class IsCodRubricaPresenteCommand : IRequest<IsCodRubricaPresenteCommandResponse>
    {
        public string CodRubrica { get; set; }
        public string TipoCorr { get; set; }
        public string IdAmm { get; set; }
        public string IdReg { get; set; }
        public bool InRubricaComune { get; set; }
    }

    public class IsCodRubricaPresenteCommandResponse
    {
        public bool Output { get; set; }
    }
}
