// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Grids;
using DocsPaVO.ricerche;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetListaFascicoliPagingCustom
{

    public record FascicolazioneGetListaFascicoliPagingCustomCommandResponse(SearchObject[] output, int numTotPage, int nRec, List<SearchResultInfo> idProjectList);

    public record FascicolazioneGetListaFascicoliPagingCustomCommand(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.utente.Registro registro, DocsPaVO.filtri.FiltroRicerca[] listaFiltri, bool enableUfficioRef, bool enableProfilazione, bool childs, int numPage, int pageSize, bool getSystemIdList, byte[] excelDati, bool showGridPersonalization, bool export, DocsPaVO.Grid.Field[] visibleFieldsTemplate, String[] documentsSystemId, bool security) : IRequest<FascicolazioneGetListaFascicoliPagingCustomCommandResponse>;



}
