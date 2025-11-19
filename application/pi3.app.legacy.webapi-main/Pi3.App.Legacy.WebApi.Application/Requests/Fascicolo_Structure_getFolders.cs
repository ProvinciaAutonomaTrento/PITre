// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using DocsPaVO.fascicolazione;
using DocsPaVO.filtri;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record FascicolazioneGetFolderAndChildResult(Folder output);

    public record FascicolazioneGetFolderAndChild(string idPeople, string idGruppo, Folder folder) : IRequest<FascicolazioneGetFolderAndChildResult>;
    public record FascicolazioneGetFolderByDescrResult(Folder[] output);

    public record FascicolazioneGetFolderByDescr(string idPeople, string idGruppo, string idFascicolo, string descrFolder) : IRequest<FascicolazioneGetFolderByDescrResult>;
    
    public record FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisibleResult(bool output);

    public record FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisible(InfoUtente infoUtente, Folder folder) : IRequest<FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisibleResult>;
    public record FascicolazioneMoveFolderResult(bool output);

    public record FascicolazioneMoveFolder(string folderId, string parentId) : IRequest<FascicolazioneMoveFolderResult>;
    public record getNodoTitolarioByIdResult(OrgNodoTitolario output);

    public record getNodoTitolarioById(string idNodoTitolario) :IRequest<getNodoTitolarioByIdResult>;
}
