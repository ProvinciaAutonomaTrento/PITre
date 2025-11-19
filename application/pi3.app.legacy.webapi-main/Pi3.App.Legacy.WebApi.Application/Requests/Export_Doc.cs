// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ExportData;
using DocsPaVO.fascicolazione;
using DocsPaVO.filtri;
using DocsPaVO.Grid;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record ExportRicercaDocInFascCustom(Folder folder, string codFasc, string tipologiaExport, string titolo, FiltroRicerca[][] currentFilters,
            InfoUtente userInfo, List<CampoSelezionato> objects, string[] selectedDocumentsId, Grid selectedGrid, bool v, Field[] visibleArray, FiltroRicerca[][] orderFilters) : IRequest<ExportRicercaDocInFascCustomResult>;
    public record ExportRicercaDocInFascCustomResult(DocsPaVO.documento.FileDocumento output);

    public record ExportRicercaDocInCest(DocsPaVO.utente.InfoUtente infoUtente, string exportType, string title, DocsPaVO.filtri.FiltroRicerca[][] filtri, List<CampoSelezionato> campiSelezionati) :IRequest<ExportRicercaDocInCestResult>;
    public record ExportRicercaDocInCestResult(DocsPaVO.documento.FileDocumento output);

    public record RegistroAccessiPubblicazione(DocsPaVO.RegistroAccessi.RegistroAccessiReportRequest request):IRequest<RegistroAccessiPubblicazioneResult>;
    public record RegistroAccessiPubblicazioneResult(DocsPaVO.RegistroAccessi.RegistroAccessiReportResponse output);
}
