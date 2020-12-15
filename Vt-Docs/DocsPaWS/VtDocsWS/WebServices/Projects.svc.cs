using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VtDocsWS.Services;
using System.ServiceModel.Activation;
using log4net;

namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per la gestione dei documenti
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class Projects : IProjects
    {
        private ILog logger = LogManager.GetLogger(typeof(Projects));


        /// <summary>
        /// Servizio per creare un fascicolo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.CreateProject.CreateProjectResponse CreateProject(Services.Projects.CreateProject.CreateProjectRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.CreateProject.CreateProjectResponse response = Manager.ProjectsManager.CreateProject(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per modificare lo stato di un fascicolo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.EditPrjStateDiagram.EditPrjStateDiagramResponse EditPrjStateDiagram(Services.Projects.EditPrjStateDiagram.EditPrjStateDiagramRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.EditPrjStateDiagram.EditPrjStateDiagramResponse response = Manager.ProjectsManager.EditPrjStateDiagram(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per modificare lo stato di un fascicolo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.EditProject.EditProjectResponse EditProject(Services.Projects.EditProject.EditProjectRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.EditProject.EditProjectResponse response = Manager.ProjectsManager.EditProject(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per reperire il dettaglio di un fascicolo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.GetProject.GetProjectResponse GetProject(Services.Projects.GetProject.GetProjectRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.GetProject.GetProjectResponse response = Manager.ProjectsManager.GetProject(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per reperire i fascicoli in cui è classificato un documento.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.GetProjectsByDocument.GetProjectsByDocumentResponse GetProjectsByDocument(Services.Projects.GetProjectsByDocument.GetProjectsByDocumentRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.GetProjectsByDocument.GetProjectsByDocumentResponse response = Manager.ProjectsManager.GetProjectsByDocument(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per reperire lo stato di un fascicolo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.GetProjectStateDiagram.GetProjectStateDiagramResponse GetProjectStateDiagram(Services.Projects.GetProjectStateDiagram.GetProjectStateDiagramRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.GetProjectStateDiagram.GetProjectStateDiagramResponse response = Manager.ProjectsManager.GetProjectStateDiagram(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per reperire lo stato di un documento.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.GetTemplatePrj.GetTemplatePrjResponse GetTemplatePrj(Services.Projects.GetTemplatePrj.GetTemplatePrjRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.GetTemplatePrj.GetTemplatePrjResponse response = Manager.ProjectsManager.GetTemplatePrj(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per reperire i template dei fascicoli.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.GetTemplatesProjects.GetTemplatesProjectsResponse GetTemplatesProjects(Services.Projects.GetTemplatesProjects.GetTemplatesProjectsRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.GetTemplatesProjects.GetTemplatesProjectsResponse response = Manager.ProjectsManager.GetTemplatesProjects(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per aprire/chiudere un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.OpenCloseProject.OpenCloseProjectResponse OpenCloseProject(Services.Projects.OpenCloseProject.OpenCloseProjectRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.OpenCloseProject.OpenCloseProjectResponse response = Manager.ProjectsManager.OpenCloseProject(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per cercare i fascicoli
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response </returns>
        public Services.Projects.SearchProjects.SearchProjectsResponse SearchProjects(Services.Projects.SearchProjects.SearchProjectsRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.SearchProjects.SearchProjectsResponse response = Manager.ProjectsManager.SearchProjects(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio che restituisce la lista dei filtri disponibili
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.Projects.GetProjectFilters.GetProjectFiltersResponse GetProjectFilters(Services.Projects.GetProjectFilters.GetProjectFiltersRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.GetProjectFilters.GetProjectFiltersResponse response = Manager.ProjectsManager.GetProjectFilters(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// 
        /// Servizio che permette alla coppia (ruolo,utente) di monitorare un fascicolo presente nel sistema.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        public Services.FollowDomainObject.FollowResponse FollowProject(Services.FollowDomainObject.FollowRequest request)
        {
            logger.Info("BEGIN");
            Services.FollowDomainObject.FollowResponse response = Manager.ProjectsManager.FollowProject(request);
            logger.Info("END");
            Utils.CheckFaultException(response);
            return response;
        }

        /// <summary>
        /// Servizio per il prelievo del link al Fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Services.Projects.GetLinkPrjByID.GetLinkPrjByIDResponse GetLinkPrjByID(Services.Projects.GetLinkPrjByID.GetLinkPrjByIDRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.GetLinkPrjByID.GetLinkPrjByIDResponse response = Manager.ProjectsManager.GetLinkPrjByID(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        /// <summary>
        /// Servizio per rimuovere un sottofascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Services.Projects.RemoveProjectFolders.RemoveProjectFoldersResponse RemoveProjectFolders(Services.Projects.RemoveProjectFolders.RemoveProjectFoldersRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.RemoveProjectFolders.RemoveProjectFoldersResponse response = Manager.ProjectsManager.RemoveProjectFolder(request);
            logger.Info("END");
            Utils.CheckFaultException(response);
            return response;
        }

        #region Servizi sviluppati per MIT
        /// <summary>
        /// Servizio per la creazione di un sottofascicolo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Services.Projects.CreateFolder.CreateFolderResponse CreateFolder(Services.Projects.CreateFolder.CreateFolderRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.CreateFolder.CreateFolderResponse response = Manager.ProjectsManager.CreateFolder(request);
            logger.Info("END");
            Utils.CheckFaultException(response);
            return response;
        }

        /// <summary>
        /// Servizio per prelevare la struttura di un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Services.Projects.GetProjectFolders.GetProjectFoldersResponse GetProjectFolders(Services.Projects.GetProjectFolders.GetProjectFoldersRequest request)
        {
            logger.Info("BEGIN");
            Services.Projects.GetProjectFolders.GetProjectFoldersResponse response = Manager.ProjectsManager.GetProjectFolders(request);
            logger.Info("END");
            Utils.CheckFaultException(response);
            return response;
        }
        #endregion
    }
}
