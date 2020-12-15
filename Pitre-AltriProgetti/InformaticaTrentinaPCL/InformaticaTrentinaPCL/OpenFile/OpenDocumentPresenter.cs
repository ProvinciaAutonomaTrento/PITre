using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.OpenFile.MVP;
using InformaticaTrentinaPCL.OpenFile.Network;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class OpenDocumentPresenter : IOpenDocumentPresenter
    {
        protected IOpenDocumentView view;
        protected IOpenDocumentModel model;
        protected IFileSystemManager fileSystemManager;
        protected SessionData sessionData;
        protected IAnalyticsManager analyticsManager;
        protected IReachability reachability;
        protected string currentIdDoc;
        protected List<AbstractDocumentListItem> listDocuments;

        public OpenDocumentPresenter(IOpenDocumentView view, INativeFactory nativeFactory, string idDoc)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.fileSystemManager = nativeFactory.GetFileSystemManager();
            this.reachability = nativeFactory.GetReachability();
            this.currentIdDoc = idDoc;
#if CUSTOM
            this.model = new DummyOpenDocumentModel();
#else
            this.model = new OpenDocumentModel();
#endif
        }

        public OpenDocumentPresenter(IOpenDocumentView view, INativeFactory nativeFactory, SharedDocumentBundle documentBundle)
          : this(view, nativeFactory, documentBundle.docId)
        {
            this.listDocuments = new List<AbstractDocumentListItem>(documentBundle.documents);
        }

        public virtual void OnViewReady()
        {
            if (null != listDocuments)
            {
                view.ShowList(listDocuments);
            }
            else
            {
                LoadDocInfo(currentIdDoc);
            }
        }

        public void Dispose()
        {
            model.Dispose();
        }

        public async void OnSelect(AbstractDocumentListItem file)
        {
            if (file is DocInfo)
            {
                await OpenDocInfoIsAquired(file as DocInfo);
            }
            else
            {
                await OpenAbastractItem(file);
            }
        }

        private async Task OpenDocInfoIsAquired(DocInfo file)
        {
            var docInfo = file as DocInfo;
            if (docInfo.isAcquisito)
            {
                await OpenAbastractItem(file);
            }
            else
            {
                view.ShowError(LocalizedString.IMAGE_NOT_AQUIRED.Get());
            }
        }

        private async Task OpenAbastractItem(AbstractDocumentListItem file)
        {
            view.OnUpdateLoaderWithAction(true);
            GetFileRequestModel.Body body = new GetFileRequestModel.Body(file.GetIdDocumento());
            GetFileRequestModel request = new GetFileRequestModel(body, sessionData.userInfo.token);
            GetFileResponseModel response = await model.GetFile(request);
            this.ManageResponse(response, file);
        }

        protected void OpenContent(GetFileResponseModel response, AbstractDocumentListItem original)
        {
            //string fileName = file.GetName()+"."+file.GetExtension();
            //string content = file.GetBase64Content();
            string fileName = Path.GetFileNameWithoutExtension(response.file.name);
            string content = response.file.content;
            string ext = null != response.file.estensioneFile ? response.file.estensioneFile : Path.GetExtension(response.file.fullName).Replace(".", ""); // Check if service return always extension in this format
            view.OnUpdateLoaderWithAction(false);

            Byte[] inputBytes = Convert.FromBase64String(content);
            fileSystemManager.saveFileAndOpen(inputBytes, fileName, ext, view);
        }

        protected async void LoadDocInfo(string currentIdDoc)
        {
            view.OnUpdateLoader(true);
            GetDocInfoRequestModel.Body body = new GetDocInfoRequestModel.Body(currentIdDoc);
            GetDocInfoRequestModel request = new GetDocInfoRequestModel(body, sessionData.userInfo.token);
            GetDocInfoResponseModel response = await model.GetDocInfo(request);
            view.OnUpdateLoader(false);
            this.ManageResponse(response);
        }

        protected void ManageResponse(GetDocInfoResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        UpdateUI(response);
                        break;
                    case 1:
                        ManageError(response);
                        break;
                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

        protected void ManageError(GetDocInfoResponseModel response)
        {
            if (response.docInfo == null && response.allegati == null)
            {
                view.ShowError(LocalizedString.MESSAGE_NO_DOCUMENT_ASSOCIATE.Get());
            }
        }

        protected void ManageResponse(GetFileResponseModel response, AbstractDocumentListItem original)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        OpenContent(response, original);
                        break;
                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        view.OnUpdateLoaderWithAction(false);
                        break;
                }
            }
        }

        protected void UpdateUI(GetDocInfoResponseModel response)
        {
            if (response.docInfo.idDocPrincipale == null)
            {
                view.ShowList(new List<AbstractDocumentListItem>(response.GetDocumentsList()));
            }
            else
            {
                //download master document and show all attachments
                LoadDocInfo(response.docInfo.idDocPrincipale);
            }
        }

        public void CancelDownload()
        {
            view.OnUpdateLoaderWithAction(false);
            model.Dispose();
        }

        public string GetDocumentTitle(AbstractDocumentListItem file)
        {
            return file is DocInfo && ((DocInfo)file).isProtocollato
                ? ((DocInfo)file).GetOggetto() + Environment.NewLine + ((DocInfo)file).segnatura + Environment.NewLine + file.ConvertDateFormatIfNeeded(((DocInfo)file).dataProto)
                : ((DocInfo)file).GetOggetto() + Environment.NewLine + file.GetIdDocumento() + Environment.NewLine + file.ConvertDateFormatIfNeeded(file.GetData());
        }
    }
}