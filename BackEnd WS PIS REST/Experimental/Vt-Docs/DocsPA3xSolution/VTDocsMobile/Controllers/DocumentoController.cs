using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe.model;
using VTDocs.mobile.fe.commands.model;
using log4net;

namespace VTDocs.mobile.fe.controllers
{
    public class DocumentoController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(DocumentoController));

        [NoCache]
        public ActionResult File(string id)
        {
            try
            {
                logger.Info("begin");
                GetFileRequest request = new GetFileRequest();
                request.UserInfo = NavigationHandler.CurrentUser;
                request.IdDoc = id;
                request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
                request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
                GetFileResponse response = WSStub.getFile(request);
                if (response.Code == GetFileResponseCode.OK)
                {
                    //forza il content-type ad octet-stream se vuoto
                    if (response.File.ContentType =="")
                        if (response.File.FullName.ToUpper().Contains(".PDF"))
                            response.File.ContentType = "Content-type: application/pdf";
                        else
                         response.File.ContentType = "Content-type: application/octet-stream";
                    logger.Info("return doc");
                    return File(response.File.Content, response.File.ContentType, response.File.OriginalFileName);
                }
                else
                {
                    logger.Info("doc not found");
                    return View("DocNotFound");
                }
            }
            catch (Exception e)
            {
                logger.Error("eccezione: "+e);
                return View("DocNotFound");
            }


        }

        [NoCache]
        public ActionResult Preview(string id,string page,string dimX,string dimY)
        {
            try
            {
                logger.Info("begin");
                logger.Info("page: " + page + ", dimX: " + dimX + ", dimY: " + dimY);
                GetPreviewRequest request = new GetPreviewRequest();
                int numPage = 1;
                if (!string.IsNullOrEmpty(page))
                {
                    numPage=int.Parse(page);
                }
                int _dimX = 845;
                if (!string.IsNullOrEmpty(dimX))
                {
                    _dimX = int.Parse(dimX);
                }
                int _dimY = 1200;
                if (!string.IsNullOrEmpty(dimY))
                {
                    _dimY = int.Parse(dimY);
                }
                request.UserInfo = NavigationHandler.CurrentUser;
                request.IdDoc = id;
                request.RequestedPage = numPage;
                request.DimX = _dimX;
                request.DimY = _dimY;
                request.IdGruppo = NavigationHandler.RuoloInfo.IdGruppo;
                request.IdCorrGlobali = NavigationHandler.RuoloInfo.Id;
                GetPreviewResponse response = WSStub.getPreview(request);
                switch (response.Code)
                {
                    case GetPreviewResponseCode.OK:
                        logger.Info("return doc");
                        return File(response.File.Content, response.File.ContentType);
                    case GetPreviewResponseCode.NO_PREVIEW_FOR_EXTENSION:
                        logger.Info("doc not found");
                        return File("~/Content/Green/img/ipad/no_preview.jpg", "image/jpeg");
                    case GetPreviewResponseCode.PAGE_NOT_FOUND:
                        return File("~/Content/Green/img/ipad/no_page.jpg", "image/jpeg");
                    default:
                        logger.Info("doc not found");
                        return File("~/Content/img/ipad/no_acquisito.jpg", "image/jpeg");
                }
            }
            catch (Exception e)
            {
                logger.Error("Eccezione: " + e);
                return File(NavigationHandler.NoPreviewImage, "image/jpeg");
            }

        }

        [NoCache]
        public ActionResult DettaglioDocTrasm(string idDoc,string idTrasm, string idEvento)
        {
            logger.Info("begin");
            ActionResult res=CommandExecute(new DettaglioDocTrasmCommand(idDoc,idTrasm, idEvento,true));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult DettaglioDoc(string idDoc,string idTrasm, string idEvento)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new DettaglioDocTrasmCommand(idDoc,idTrasm, idEvento,false));
            logger.Info("end");
            return res;
        }
    }
}
