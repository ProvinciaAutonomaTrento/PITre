using System.Collections.Generic;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.OpenFile.Network;
using InformaticaTrentinaPCL.Utils;
using TestRefit.Network;
using System.Linq;
using InformaticaTrentinaPCL.Home.MVPD;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class OpenFascPresenter : OpenDocumentPresenter, IOpenFascPresenter
    {
        protected IOpenFascModel modelFasc;
        protected string searchString;
        protected int countPage;
        protected int totalCount;
        protected List<AbstractDocumentListItem> results = new List<AbstractDocumentListItem>();

        public OpenFascPresenter(IOpenFascView view, INativeFactory nativeFactory, string currentFascId):base(view,nativeFactory,currentFascId )
        {
#if CUSTOM
            this.modelFasc = new DummyOpenFascModel();
#else
            this.modelFasc = new OpenFascModel();
#endif
        }

        protected void Initialize()
        {
            ((WS)model).Dispose();
            this.totalCount = int.MaxValue;
            this.countPage = 0;
            this.searchString = null;
            results.Clear();
        }

        public override void OnViewReady()
        {
             LoadFileList();
        }

        public async void LoadFileList()
        {
            PreRequest();
            int listPageSize = countPage == 1 ?  NetworkConstants.DEFAULT_LIST_PAGE_SIZE * 2:  NetworkConstants.DEFAULT_LIST_PAGE_SIZE;
            GetFascRicercaRequestModel.Body body = new GetFascRicercaRequestModel.Body(currentIdDoc, listPageSize, countPage, searchString);
            GetFascRicercaRequestModel request = new GetFascRicercaRequestModel(body, sessionData.userInfo.token);
            GetFascRicercaResponseModel response = await modelFasc.GetFascRicerca(request);
            PostRequest(response);
        }

        public void DoNextPage()
        {
            if (ListHasMoreElement())
            {
                LoadFileList();
            }
        }

        public void OnSearch(string text)
        {

            if (text.Trim().Length < NetworkConstants.CONSTANT_SEARCH_CHARS)
            {
                view.ShowError(LocalizedString.SEARCH_STRING_TOO_SHORT.Get(),true);
            }
            else
            {
                Initialize();
                this.searchString = text;
                LoadFileList();
            }
        }

        public void ClearSearch()
        {
            Initialize();
            LoadFileList();
        }
        
        

        #region Utility
        /// <summary>
        /// L'update del loader è fatto solo per la prima pagina, mentre per le
        /// altre pagine il loading è trasparente.
        /// Valutare questo comportamento con UX.
        /// </summary>
        /// <param name="isShow">If set to <c>true</c> the loader is shown.</param>
        protected void shouldUpdateLoader(bool isShow)
        {
            if (countPage == 1)
            {
                view.OnUpdateLoader(isShow);
            }
        }

        protected void PreRequest()
        {
            // WARNING: Non muovere l'incremento del countPage da qua. 
            // Deve essere la prima operazione dell'if.
            countPage++;
            shouldUpdateLoader(true);
        }

        protected void PostRequest(GetFascRicercaResponseModel response)
        {
            shouldUpdateLoader(false);
            ResponseHelper responseHelper = new ResponseHelper(view, response,reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        totalCount = response.totalRecordCount;
                        shouldRemoveCellLoader();
                        if (null != response.risultati)
                        {
                            ConvertResponseItemsInSharedDocumentBundleItems(response.risultati);
                        }
                        shouldAddCellLoader();
                        view.ShowList(results);
                        break;
                    default: view.ShowError(LocalizedString.SERVICE_ERROR.Get()); break;
                }
            }
        }

        private void ConvertResponseItemsInSharedDocumentBundleItems(List<ResultFascRicerca> apiResults)
        {
            foreach (var item in apiResults)
            {
                results.Add(new SharedDocumentBundle(item.documenti, item.infoElement.id));
            }
        }

        protected void shouldRemoveCellLoader()
        {
            if (results.Count == 0)
            {
                return;
            }

            int lastItemIndex = results.Count - 1;
            if (results.ElementAt(lastItemIndex) is DocumentListLoader)
            {
                results.RemoveAt(lastItemIndex);
            }
        }

        protected void shouldAddCellLoader()
        {
            if (ListHasMoreElement())
            {
                results.Add(new DocumentListLoader());
            }
        }

        public void OnSelect(AbstractDocumentListItem file)
        {
            if (file is SharedDocumentBundle)
            {
                
                ((IOpenFascView) view).OpenDocumentBundle((SharedDocumentBundle) file);
            }
            else
            {
                base.OnSelect(file);    
            }
        }


        /// <summary>
        /// Check if there are more elements to show
        /// </summary>
        /// <returns><c>true</c>, if has more element was listed, <c>false</c> otherwise.</returns>

        public bool ListHasMoreElement()
        {
            return totalCount > NetworkConstants.DEFAULT_LIST_PAGE_SIZE * countPage;
        }
        #endregion
    }
}
