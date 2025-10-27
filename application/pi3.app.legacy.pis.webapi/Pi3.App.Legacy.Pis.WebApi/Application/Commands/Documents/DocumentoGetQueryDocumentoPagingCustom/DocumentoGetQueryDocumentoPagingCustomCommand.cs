// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Grid;
using DocsPaVO.Grids;
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetQueryDocumentoPagingCustom
{
    public class DocumentoGetQueryDocumentoPagingCustomCommand : IRequest<DocumentoGetQueryDocumentoPagingCustomResponse>
    {
        public InfoUtente InfoUtente { get; set; }
        public DocsPaVO.filtri.FiltroRicerca[][] QueryList { get; set; }
        public int NumPage { get; set; }
        public bool Security { get; set; }
        public int PageSize { get; set; }
        public bool GetIdProfilesList { get; set; }
        public bool GridPersonalization { get; set; }
        public bool Export { get; set; }
        public DocsPaVO.Grid.Field[] VisibleFieldsTemplate { get; set; }
        public String[] DocumentsSystemId { get; set; }
    }

    public class DocumentoGetQueryDocumentoPagingCustomResponse
    {

        public SearchObject[] Output { get; set; }
        public int NumTotPage { get; set; }
        public int nRec { get; set; }
        public List<DocsPaVO.ricerche.SearchResultInfo> IdProfileList { get; set; }
        public DocumentoGetQueryDocumentoPagingCustomResponse()
        {
            
        }

        public DocumentoGetQueryDocumentoPagingCustomResponse(SearchObject[] output, int numTotPage, int nRec, List<SearchResultInfo> idProfileList)
        {
            Output = output;
            NumTotPage = numTotPage;
            this.nRec = nRec;
            IdProfileList = idProfileList;
        }
    }
}
