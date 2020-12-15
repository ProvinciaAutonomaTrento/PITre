using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Search.MVP;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Search
{
    public class SearchPresenter : DocumentListPresenter, ISearchPresenter
    {

        protected ISearchModel model;
        protected string currentSearchString;

        public SearchPresenter(ISearchView view, INativeFactory nativeFactory) : base(view, nativeFactory, SectionType.SEARCH)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.reachability = nativeFactory.GetReachability();
#if CUSTOM
            model = new DummySearchModel();
#else
            model = new SearchModel();
#endif
            Initialize();
        }

        public void ClearSearch()
        {
            Initialize();
            currentSearchString = null;
            view.UpdateList(null);
        }

        public override void LoadDocumentList()
        {
            if (ListHasMoreElement())
            {
                DoNextResultPage();
            }
        }

        public void UpdateSearchString(string text)
        {
            if (text.Trim().Length < NetworkConstants.CONSTANT_SEARCH_CHARS)
            {
                view.ShowError(LocalizedString.SEARCH_STRING_TOO_SHORT.Get(),true);
            }
            else
            {
                Initialize();
                currentSearchString = text;
                DoNextResultPage();
            }
        }

        public async void DoNextResultPage()
        {
            PreRequest();
            RicercaRequestModel request = GenereteRicercaRequest();
            RicercaResponseModel response = await model.GetRicercaElementList(request);
            PostRequest(response);
        }

        protected RicercaRequestModel GenereteRicercaRequest()
        {
            RicercaRequestModel.Body body = (null != currentFilterModel)
                ? new RicercaRequestModel.Body(NetworkConstants.DEFAULT_LIST_PAGE_SIZE,
                    countPage,
                    currentFilterModel.GetSearchRequestType(),
                    currentFilterModel.fromDate,
                    currentFilterModel.endDate,
                    currentFilterModel.fromDateProtocol,
                    currentFilterModel.endDateProtocol,
                    currentFilterModel.idDocument,
                    currentFilterModel.NumProto, 
                    currentFilterModel.yearProto)
                
                : new RicercaRequestModel.Body(NetworkConstants.DEFAULT_LIST_PAGE_SIZE, 
                    countPage,
                    NetworkConstants.TIPO_RICERCA_ALL);

            body.text = currentSearchString;
            
            return new ADLListRequestModel(body, sessionData.userInfo.token);
        }
    }
}