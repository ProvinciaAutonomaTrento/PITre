// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Grids;
using DocsPaVO.ricerche;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadFileToDocument;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.FascicolazioneGetDocumenti
{
    public class FascicolazioneGetDocumentiCommand : IRequest<FascicolazioneGetDocumentiCommandResponse>
    {
        public DocsPaVO.utente.InfoUtente InfoUtente { get; set; }
        public Projects.Folder Folder { get; set; }
        public DocsPaVO.filtri.FiltroRicerca[][] FiltriRicerca { get; set; }
        public int NumPage { get; set; }
        public int NumTotPage { get; set; }
        public int NRec { get; set; }
        public bool CompileIdProfileList { get; set; }
        public bool ShowGridPersonalization { get; set; }
        public bool Export { get; set; }
        public DocsPaVO.Grid.Field[] VisibleFieldsTemplate { get; set; }
        public String[] DocumentsSystemId { get; set; }
        public int PageSize { get; set; }
        public DocsPaVO.filtri.FiltroRicerca[][] OrderRicerca { get; set; }
        public List<SearchResultInfo> IdProfiles { get; set; }
    }

    public class FascicolazioneGetDocumentiCommandResponse
    {

        public SearchObject[] Output { get; set; }
        public SearchResultInfo[] IdProfiles { get; set; }
        public int NumTotPage { get; set; }
        public int nRec { get; set; }

        public FascicolazioneGetDocumentiCommandResponse(SearchObject[] output, int numTotPage, int nRec, SearchResultInfo[] idProfiles)
        {
            Output = output;
            NumTotPage = numTotPage;
            this.nRec = nRec;
            this.IdProfiles = idProfiles;
        }
    }
}
