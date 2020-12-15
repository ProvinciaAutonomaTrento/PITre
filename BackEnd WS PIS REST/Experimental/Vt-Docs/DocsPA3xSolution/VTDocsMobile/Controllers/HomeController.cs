using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe.commands;
using VTDocs.mobile.fe;
using VTDocs.mobile.fe.commands.model;
using VTDocs.mobile.fe.model;
using log4net;
using VTDocsMobile.VTDocs.mobile.fe.commands.model;

namespace VTDocs.mobile.fe.controllers
{
    public class HomeController : GeneralController
    {
        private ILog logger = LogManager.GetLogger(typeof(HomeController));

        public ActionResult Index()
        {
            logger.Info("begin");
            logger.Info("UserAgent: "+Request.UserAgent);
            if (NavigationHandler.CurrentUser == null)
            {
                return RedirectToAction("Login", "Login");
            }
            MainModel model = NavigationHandler.Model;
            if (model == null)
            {
                //dopo la login.
                model = new ToDoListCommand().Execute();
                //calcolo delege in esercizio
                CountDelegheAttiveRequest cdr = new CountDelegheAttiveRequest();
                cdr.UserInfo = NavigationHandler.CurrentUser;
                CountDelegheAttiveResponse resp = WSStub.getCountDelegheAttive(cdr);
                int numDeleghe = resp.NumDeleghe;
                //inserimento nel model
                model.NumDeleghe = numDeleghe;
            }
            else
            {
                //dopo il refresh
                model.NumDeleghe = 0;
            }
            logger.Info("end");
            return View(model);
        }

        [NoCache]
        public ActionResult ShowTab(Tab idTab)
        {
            logger.Info("begin");
            logger.Info("tab to change: " + idTab);
            MainModelCommand command = null;
            if (idTab == Tab.TODO_LIST)
            {
                NavigationHandler.AdlMemento = null;
                NavigationHandler.RicercaMemento = null;

                //if (NavigationHandler.Model.PreviousTabName != "TODO_LIST" && NavigationHandler.Model.PreviousTabName != "DETTAGLIO_DOC")
                //    NavigationHandler.ToDoListMemento = null;

                //if (
                //    (NavigationHandler.Model.PreviousTabName == "DETTAGLIO_DOC" && NavigationHandler.Model.PreviousTabName2 == "TODO_LIST")
                //    /*|| (NavigationHandler.Model.PreviousTabName == "TODO_LIST" && NavigationHandler.Model.PreviousTabName != "DETTAGLIO_DOC")*/
                //    )
                //    NavigationHandler.ToDoListMemento = null;

                command = new ToDoListCommand();
            }
            if (idTab == Tab.RICERCA)
            {
                //NavigationHandler.ToDoListMemento = null;
                NavigationHandler.AdlMemento = null;

                //if (NavigationHandler.Model.PreviousTabName!="RICERCA")
                //    NavigationHandler.RicercaMemento = null;

                if (NavigationHandler.Model.PreviousTabName != "RICERCA" && NavigationHandler.Model.PreviousTabName != "DETTAGLIO_DOC")
                    NavigationHandler.RicercaMemento = null;

                if (
                    !((NavigationHandler.Model.PreviousTabName == "RICERCA" || NavigationHandler.Model.PreviousTabName == "DETTAGLIO_DOC") && (NavigationHandler.Model.PreviousTabName2 == "RICERCA" || NavigationHandler.Model.PreviousTabName2 == "DETTAGLIO_DOC"))
                    || (NavigationHandler.Model.PreviousTabName == "RICERCA" && !(NavigationHandler.Model.PreviousTabName2 == "DETTAGLIO_DOC" || NavigationHandler.Model.PreviousTabName2 == "RICERCA"))
                    )
                    NavigationHandler.RicercaMemento = null;

                command = new RicercaCommand();
            }
            if (idTab == Tab.AREA_DI_LAVORO)
            {
                //NavigationHandler.ToDoListMemento = null;
                NavigationHandler.RicercaMemento = null;

                //if (NavigationHandler.Model.PreviousTabName != "AREA_DI_LAVORO")
                //    NavigationHandler.AdlMemento = null;

                if (NavigationHandler.Model.PreviousTabName != "AREA_DI_LAVORO" && NavigationHandler.Model.PreviousTabName != "DETTAGLIO_DOC")
                    NavigationHandler.AdlMemento = null;

                if (
                    !((NavigationHandler.Model.PreviousTabName == "AREA_DI_LAVORO" || NavigationHandler.Model.PreviousTabName == "DETTAGLIO_DOC") && (NavigationHandler.Model.PreviousTabName2 == "AREA_DI_LAVORO" || NavigationHandler.Model.PreviousTabName2 == "DETTAGLIO_DOC"))
                    || (NavigationHandler.Model.PreviousTabName == "AREA_DI_LAVORO" && !(NavigationHandler.Model.PreviousTabName2 == "DETTAGLIO_DOC" || NavigationHandler.Model.PreviousTabName2 == "AREA_DI_LAVORO"))
                    )
                    NavigationHandler.AdlMemento = null;

                command = new AdlCommand();
            }
            if (idTab == Tab.LISTA_DELEGHE)
            {
                NavigationHandler.AdlMemento = null;
                NavigationHandler.RicercaMemento = null;
                //NavigationHandler.ToDoListMemento = null;

                command = new ListaDelegheCommand();
            }

            if (idTab == Tab.SMISTAMENTO)
            {
                NavigationHandler.AdlMemento = null;
                NavigationHandler.RicercaMemento = null;
                //NavigationHandler.ToDoListMemento = null;

                command = new SmistamentoFormCommand();
            }

            if (idTab == Tab.LIBRO_FIRMA)
            {
                NavigationHandler.AdlMemento = null;
                NavigationHandler.RicercaMemento = null;

                command = new LibroFirmaCommand();
            }

            ActionResult res=CommandExecute(command);
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult ShowAndResetTab(Tab idTab)
        {
            logger.Info("begin");
            logger.Info("tab to change: " + idTab);
            MainModelCommand command = null;
            if (idTab == Tab.TODO_LIST)
            {
                NavigationHandler.AdlMemento = null;
                NavigationHandler.RicercaMemento = null;
                NavigationHandler.ToDoListMemento = null;
                NavigationHandler.LibroFirmaMemento = null;

                command = new ToDoListCommand();
            }
            if (idTab == Tab.RICERCA)
            {
                //NavigationHandler.ToDoListMemento = null;
                NavigationHandler.AdlMemento = null;
                NavigationHandler.RicercaMemento = null;
                NavigationHandler.LibroFirmaMemento = null;

                command = new RicercaCommand();
            }
            if (idTab == Tab.AREA_DI_LAVORO)
            {
                //NavigationHandler.ToDoListMemento = null;
                NavigationHandler.RicercaMemento = null;
                NavigationHandler.AdlMemento = null;
                NavigationHandler.LibroFirmaMemento = null;

                command = new AdlCommand();
            }
            if (idTab == Tab.LISTA_DELEGHE)
            {
                NavigationHandler.AdlMemento = null;
                NavigationHandler.RicercaMemento = null;
                NavigationHandler.LibroFirmaMemento = null;
                //NavigationHandler.ToDoListMemento = null;

                command = new ListaDelegheCommand();
            }

            if (idTab == Tab.SMISTAMENTO)
            {
                NavigationHandler.AdlMemento = null;
                NavigationHandler.RicercaMemento = null;
                NavigationHandler.LibroFirmaMemento = null;
                //NavigationHandler.ToDoListMemento = null;

                command = new SmistamentoFormCommand();
            }

            if (idTab == Tab.LIBRO_FIRMA)
            {
                NavigationHandler.AdlMemento = null;
                NavigationHandler.RicercaMemento = null;
            }

            ActionResult res = CommandExecute(command);
            logger.Info("end");
            return res;
        }

        public ActionResult Logout()
        {
            logger.Info("begin");
            try
            {
                //Ho una delega in esercizio la dismetto prima
                if (NavigationHandler.DeleganteInfo != null)
                {
                    DismettiDelegaRequest delegaRequest = new DismettiDelegaRequest();
                    delegaRequest.IdDelegante = NavigationHandler.DeleganteInfo.UserId;
                    delegaRequest.UserInfo = NavigationHandler.CurrentUser;
                    DismettiDelegaResponse response = WSStub.dismettiDelega(delegaRequest);

                    if (response.Code == DismettiDelegaResponseCode.OK)
                    {
                        NavigationHandler.DeleganteInfo = null;
                        NavigationHandler.ToDoListMemento = null;
                        NavigationHandler.RicercaMemento = null;
                        NavigationHandler.DelegaEsercitata = null;
                    }
                }
                LogoutRequest request = new LogoutRequest();
                request.UserInfo = NavigationHandler.CurrentUser;
                
                LogoutResponse resp = WSStub.logout(request);
                logger.Info("logout success");
            }
            catch (Exception e)
            {
                logger.Info("exception: " + e);
            }
            NavigationHandler.clearSession();
            logger.Info("end");
            return RedirectToAction("Login", "Login");
        }

    }

    

    
}
