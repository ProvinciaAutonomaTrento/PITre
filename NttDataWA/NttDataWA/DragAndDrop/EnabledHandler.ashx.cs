using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DragAndDrop.File;
using NttDataWA.UIManager;
using System.Web.SessionState;

namespace NttDataWA.DragAndDrop
{
    /// <summary>
    /// Descrizione di riepilogo per EnabledHandler
    /// </summary>
    public class EnabledHandler : IHttpHandler, IRequiresSessionState 
    {

        public void ProcessRequest(HttpContext context)
        {
            string response = "FE_ENABLE_DRAG_AND_DROP NOT ENABLED";
            DragAndDropHandler.CallerPage callerPage = DragAndDropHandler.CallerPage.NONE;
            string caller;
            context.Response.StatusCode = 500;
            if (!string.IsNullOrEmpty(Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString())) && Utils.InitConfigurationKeys.GetValue("0", Utils.DBKeys.FE_ENABLE_DRAG_AND_DROP.ToString()).Equals("1"))
            {
                if (context != null && context.Request != null && !String.IsNullOrEmpty(context.Request.QueryString["caller"]))
                {
                    caller = context.Request.QueryString["caller"];
                    callerPage = (DragAndDropHandler.CallerPage)Enum.Parse(typeof(DragAndDropHandler.CallerPage), caller);                  
                }
                switch (callerPage)
                {
                    case (DragAndDropHandler.CallerPage.ATTACHMENTS):
                        if (DocumentManager.GetSelectedAttachment() != null && !String.IsNullOrEmpty(DocumentManager.GetSelectedAttachment().docNumber) && !CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
                        {
                            context.Response.StatusCode = 200;
                            response = "FE_ENABLE_DRAG_AND_DROP ENABLED";
                        }
                        break;
                    case (DragAndDropHandler.CallerPage.DOCUMENT):
                        if (DocumentManager.getSelectedRecord() != null &&
                             DocumentManager.getSelectedRecord().documenti != null &&
                             DocumentManager.getSelectedRecord().documenti[0] != null &&
                            (string.IsNullOrEmpty(DocumentManager.getSelectedRecord().documenti[0].fileSize) ||
                            Convert.ToUInt32(DocumentManager.getSelectedRecord().documenti[0].fileSize) == 0) &&
                            (!DiagrammiManager.IsDocumentInFinalState() ||
                            Convert.ToInt32(DocumentManager.getAccessRightDocBySystemID(DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser())) > 45) &&
                            !String.IsNullOrEmpty(DocumentManager.getSelectedRecord().docNumber) &&
                            !DocumentManager.IsDocumentCheckedOut() && !CheckInOut.CheckInOutServices.IsCheckedOutDocument(DocumentManager.getSelectedRecord().docNumber, DocumentManager.getSelectedRecord().docNumber, UserManager.GetInfoUser(), true, DocumentManager.getSelectedRecord()))
                        {
                            context.Response.StatusCode = 200;
                            response = "FE_ENABLE_DRAG_AND_DROP ENABLED";
                        }
                        break;
                    default:
                        context.Response.StatusCode = 200;
                        response = "FE_ENABLE_DRAG_AND_DROP ENABLED";
                        break;
                }
            }          

            context.Response.ContentType = "text/plain";
            context.Response.Write(response);
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