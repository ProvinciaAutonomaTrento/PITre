// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Deposito;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record GetAllARCHIVE_TransferFilterForSearchResult(ARCHIVE_TransferForSearch[] output);

    public record GetAllARCHIVE_TransferFilterForSearch(string st_indefinizione, string st_analisicompletata, string st_proposto, string st_approvato, string st_inesecuzione, string st_effettuato, string st_inerrore, int Finger) : IRequest<GetAllARCHIVE_TransferFilterForSearchResult>;
}
