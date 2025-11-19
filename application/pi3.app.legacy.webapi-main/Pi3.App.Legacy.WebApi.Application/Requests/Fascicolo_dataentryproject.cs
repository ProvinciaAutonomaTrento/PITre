// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.rubrica;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record rubricaGetElementoRubricaResult(ElementoRubrica output);

    public record rubricaGetElementoRubrica(string cod, InfoUtente u, SmistamentoRubrica smistamentoRubrica, string condRegistri) : IRequest<rubricaGetElementoRubricaResult>;
}
