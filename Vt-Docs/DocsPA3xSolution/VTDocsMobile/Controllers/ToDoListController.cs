using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VTDocsMobile.VTDocsWSMobile;
using VTDocs.mobile.fe.commands;

using System.Web.Mvc;
using VTDocs.mobile.fe.commands.model;
using log4net;

namespace VTDocs.mobile.fe.controllers
{
    public class ToDoListController : GeneralController{
        private ILog logger = LogManager.GetLogger(typeof(ToDoListController));

        [NoCache]
        public ActionResult UpFolder()
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new TDLUpFolderCommand());
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult EnterInFolder(string idFolder,string nameFolder,string idTrasm)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new TDLEnterInFolderCommand(idFolder, nameFolder, idTrasm));
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult ChangePage(int numPage)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new TDLChangePageCommand(numPage));
            logger.Info("end");
            return res;

        }

        [NoCache]
        public ActionResult ChangeRole(string idRuolo)
        {
            logger.Info("begin");
            RuoloInfo[] ruoli = NavigationHandler.CurrentUser.Ruoli;
            foreach (RuoloInfo temp in ruoli)
            {
                if(temp.Id.Equals(idRuolo)){
                    NavigationHandler.RuoloInfo = temp;
                    NavigationHandler.ToDoListMemento = null;
                }
            }
            ActionResult res=CommandExecute(new ToDoListCommand());
            logger.Info("end");
            return res;
        }

        [NoCache]
        public ActionResult RimuoviNotificaToDoList(string idEvento)
        {
            logger.Info("begin");
            ActionResult res = CommandExecute(new TDLRimuoviNotifica(idEvento));
            logger.Info("end");
            return res;
        }

    }
}