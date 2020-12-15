using NttDataWA.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NttDataWA.UIManager
{
    public class DragAndDropManager
    {
        private const string sessionValue = "massiveDragAndDropReport";

        public static MassiveOperationReport Report
        {
            get
            {
                try
                {
                    MassiveOperationReport report = (MassiveOperationReport)HttpContext.Current.Session[sessionValue];
                    return report;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    HttpContext.Current.Session[sessionValue] = value;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        public static void ClearReport(){
            HttpContext.Current.Session.Remove(sessionValue);
        }
    }

    public class DragAndDropResult
    {
        private Boolean success;
        private String fileName;
        private int contentLength;
        private String docNumber;
        private String error;

        public DragAndDropResult()
        {

        }

        public DragAndDropResult(Boolean success, String fileName, int contentLength, String docNumber, String error)
        {
            this.success = success;
            this.fileName = fileName;
            this.contentLength = contentLength;
            this.docNumber = docNumber;
            this.error = error;
        }

        public Boolean Success
        {
            get
            {
                try
                {
                    return success;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                    return false;
                }
            }
            set
            {
                try
                {
                    success = value;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        public String FileName
        {
            get
            {
                try
                {
                    return fileName;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    fileName = value;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        public int ContentLength
        {
            get
            {
                try
                {
                    return contentLength;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                    return 0;
                }
            }
            set
            {
                try
                {
                    contentLength = value;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        public String DocNumber
        {
            get
            {
                try
                {
                    return docNumber;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }
            set
            {
                try
                {
                    docNumber = value;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

        public String Error
        {
            get
            {
                try
                {
                    return error;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                    return null;
                }
            }

            set
            {
                try
                {
                    error = value;
                }
                catch (System.Exception ex)
                {
                    NttDataWA.UIManager.AdministrationManager.DiagnosticError(ex);
                }
            }
        }

    }
}