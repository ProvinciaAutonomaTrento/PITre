using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.UIManager;
using NttDataWA.DocsPaWR;
using System.IO;
using System.Web.SessionState;
using Newtonsoft.Json;
using System.Web.UI;
using NttDataWA.Utils;
using NttDataWA.Navigation;

namespace DragAndDrop.File
{
    /// <summary>
    /// Descrizione di riepilogo per FileUploadHandler
    /// </summary>
    public class DragAndDropHandler : IHttpHandler, IRequiresSessionState 
    {
        private const String proto = "G";

        public enum CallerPage
        {
            NONE = -1,
            ADLDOCUMENT = 0,
            PROJECT = 1,
            DOCUMENT = 2,
            ATTACHMENTS = 3
        }

        public void ProcessRequest(HttpContext context)
        {
            if (!string.IsNullOrEmpty(NttDataWA.Utils.InitConfigurationKeys.GetValue("0", NttDataWA.Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString())) && NttDataWA.Utils.InitConfigurationKeys.GetValue("0", NttDataWA.Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString()).Equals("1"))
            {
                
                DragAndDropResult response = null; 
                HttpPostedFile file = null;
                HttpFileCollection files = null;
                HttpRequest request = null;
                bool first = true;
                CallerPage callerPage;

                try
                {
                    if (context != null && context.Request != null && context.Request.Files != null && context.Request.Files.Count > 0)
                    {
                        request = context.Request;
                        files = request.Files;
                        first = Boolean.Parse(request["First"]);
                        callerPage = (CallerPage)Enum.Parse(typeof(CallerPage), request["CallerPage"]);
                        if (first)
                            DragAndDropManager.ClearReport();

                        for (int i = 0; i < files.Count; i++)
                        {
                            file = files[i];

                            switch (callerPage)
                            {
                                case(CallerPage.ADLDOCUMENT):
                                    response = AddWorkArea(file, context.Session);
                                    break;
                                case(CallerPage.PROJECT):
                                    response = AddInProject(file, first);
                                    break;
                                case (CallerPage.DOCUMENT):
                                case (CallerPage.ATTACHMENTS):
                                    response = AddNewDocument(file, context.Session);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    response = new DragAndDropResult();
                    response.Success = false;
                    if (file != null)
                    {
                        response.FileName = file.FileName;
                        response.ContentLength = file.ContentLength;
                    }
                    response.Error = ex.Message;

                    MassiveOperationReport.MassiveOperationResultEnum result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    if (DragAndDropManager.Report == null)
                        DragAndDropManager.Report = new MassiveOperationReport();
                    DragAndDropManager.Report.AddReportRow(response.FileName + " " + response.DocNumber, result, response.Error);
                }

                context.Response.Write(JsonConvert.SerializeObject(response));
            }
        }

        private DragAndDropResult AddWorkArea(HttpPostedFile file, HttpSessionState session)
        {
            string docNumber = string.Empty;
            DragAndDropResult response = null;

            SchedaDocumento schedaDocumento = DocumentManager.NewSchedaDocumento();
            schedaDocumento.oggetto.descrizione = Path.GetFileNameWithoutExtension(file.FileName);
            schedaDocumento = DocumentManager.creaDocumentoGrigio(null, schedaDocumento);
            FileManager.uploadFileFromSchedaDocumento(null, file, schedaDocumento);
            docNumber = schedaDocumento.docNumber;
            DocumentManager.addAreaLavoro(session, schedaDocumento);
            response = new DragAndDropResult(true, file.FileName, file.ContentLength, docNumber, string.Empty);

            return response;
        }

        private DragAndDropResult AddInProject(HttpPostedFile file, bool addNavigation)
        {
            string docNumber = string.Empty;
            DragAndDropResult response = null;
            String error;
            List<NttDataWA.Navigation.NavigationObject> navigationList = null;
            NavigationObject page = null;

            if (addNavigation)
            {
                navigationList = NavigationUtils.GetNavigationList();
                page = navigationList.Last();
                page = (NavigationObject)page.Clone();
                navigationList.Add(page);
                NavigationUtils.SetNavigationList(navigationList);
            }

            SchedaDocumento schedaDocumento = DocumentManager.NewSchedaDocumento();
            schedaDocumento.oggetto.descrizione = Path.GetFileNameWithoutExtension(file.FileName);
            schedaDocumento = DocumentManager.creaDocumentoGrigio(null, schedaDocumento);
            FileManager.uploadFileFromSchedaDocumento(null, file, schedaDocumento);
            docNumber = schedaDocumento.docNumber;
            Fascicolo fascicolo = NttDataWA.UIManager.ProjectManager.getProjectInSession();

            if (!DocumentManager.fascicolaRapida(null, schedaDocumento.systemId, schedaDocumento.docNumber, string.Empty, fascicolo, out error))
            {
                if (string.IsNullOrEmpty(error))
                {
                    string language = NttDataWA.UIManager.UserManager.GetUserLanguage();
                    error = NttDataWA.Utils.Languages.GetMessageFromCode("WarningDocumentNoClassificated", language);
                }
                throw new Exception(error);
            }

            response = new DragAndDropResult(true, file.FileName, file.ContentLength, docNumber, string.Empty);

            return response;
        }

        private DragAndDropResult AddNewDocument(HttpPostedFile file, HttpSessionState session)
        {
            string docNumber = string.Empty;
            DragAndDropResult response = null;
            NttDataWA.DocsPaWR.FileDocumento fileDoc = new NttDataWA.DocsPaWR.FileDocumento();

            if (file != null)
            {
                bool cartaceo = false;
                fileDoc.name = System.IO.Path.GetFileName(file.FileName);
                fileDoc.fullName = fileDoc.name;
                fileDoc.contentType = file.ContentType;
                fileDoc.length = file.ContentLength;
                fileDoc.content = new Byte[fileDoc.length];
                fileDoc.cartaceo = cartaceo;
                file.InputStream.Read(fileDoc.content, 0, fileDoc.length);


                //FileManager.uploadFile(this,fileDoc,false);
                FileManager.getInstance(session.SessionID).uploadFile(null, false, fileDoc);
                //FileManager.uploadFile(this, fileDoc, false, false, true);
            }
            response = new DragAndDropResult(true, file.FileName, file.ContentLength, docNumber, string.Empty);

            return response;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}