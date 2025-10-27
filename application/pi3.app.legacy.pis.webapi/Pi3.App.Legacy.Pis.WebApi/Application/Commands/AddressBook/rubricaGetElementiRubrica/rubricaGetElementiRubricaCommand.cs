// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.rubrica;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.rubricaGetElementiRubrica
{
    public class rubricaGetElementiRubricaCommand :IRequest<rubricaGetElementiRubricaCommandResponse>
    {
        public ParametriRicercaRubrica Qc { get; set; }
        public InfoUtente U { get; set; }
        public SmistamentoRubrica SmistamentoRubrica { get; set; }
    }

    public class rubricaGetElementiRubricaCommandResponse
    {
        public ElementoRubrica[] Output { get; set; }
    }
}
