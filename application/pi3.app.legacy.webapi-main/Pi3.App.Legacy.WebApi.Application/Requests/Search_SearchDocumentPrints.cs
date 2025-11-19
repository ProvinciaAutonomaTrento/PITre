// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.filtri;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ricerche;
using DocsPaVO.utente.Repertori;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record UtenteGetRegistriNoFiltroAOOResult(Registro[] output, bool filtroAOO);

    public record UtenteGetRegistriNoFiltroAOO(string idAmm) : IRequest<UtenteGetRegistriNoFiltroAOOResult>;
    public record GetRegistriesWithAooOrRfSupResult(RegistroRepertorio[] output);

    public record GetRegistriesWithAooOrRfSup(string idRoleResp, string idRolePrinter) : IRequest<GetRegistriesWithAooOrRfSupResult>;
    public record getOggettoByIdResult(OggettoCustom output);

    public record getOggettoById(string idOggetto) : IRequest<getOggettoByIdResult>;
}
