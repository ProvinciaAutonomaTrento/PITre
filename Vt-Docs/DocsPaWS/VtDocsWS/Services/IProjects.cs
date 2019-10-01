using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per i fascicoli dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IProjects
    {
        [OperationContract]
        VtDocsWS.Services.Projects.CreateProject.CreateProjectResponse CreateProject(VtDocsWS.Services.Projects.CreateProject.CreateProjectRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.EditPrjStateDiagram.EditPrjStateDiagramResponse EditPrjStateDiagram(VtDocsWS.Services.Projects.EditPrjStateDiagram.EditPrjStateDiagramRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.EditProject.EditProjectResponse EditProject(VtDocsWS.Services.Projects.EditProject.EditProjectRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.GetProject.GetProjectResponse GetProject(VtDocsWS.Services.Projects.GetProject.GetProjectRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.GetProjectsByDocument.GetProjectsByDocumentResponse GetProjectsByDocument(VtDocsWS.Services.Projects.GetProjectsByDocument.GetProjectsByDocumentRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.GetProjectStateDiagram.GetProjectStateDiagramResponse GetProjectStateDiagram(VtDocsWS.Services.Projects.GetProjectStateDiagram.GetProjectStateDiagramRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.GetTemplatePrj.GetTemplatePrjResponse GetTemplatePrj(VtDocsWS.Services.Projects.GetTemplatePrj.GetTemplatePrjRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.GetTemplatesProjects.GetTemplatesProjectsResponse GetTemplatesProjects(VtDocsWS.Services.Projects.GetTemplatesProjects.GetTemplatesProjectsRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.SearchProjects.SearchProjectsResponse SearchProjects(VtDocsWS.Services.Projects.SearchProjects.SearchProjectsRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.OpenCloseProject.OpenCloseProjectResponse OpenCloseProject(VtDocsWS.Services.Projects.OpenCloseProject.OpenCloseProjectRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.GetProjectFilters.GetProjectFiltersResponse GetProjectFilters(VtDocsWS.Services.Projects.GetProjectFilters.GetProjectFiltersRequest request);

        /// <summary>
        /// Servizio che permette alla coppia (ruolo,utente) di monitorare un fascicolo presente nel sistema.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Response</returns>
        [OperationContract]
        VtDocsWS.Services.FollowDomainObject.FollowResponse FollowProject(VtDocsWS.Services.FollowDomainObject.FollowRequest request);

        [OperationContract]
        VtDocsWS.Services.Projects.GetLinkPrjByID.GetLinkPrjByIDResponse GetLinkPrjByID(VtDocsWS.Services.Projects.GetLinkPrjByID.GetLinkPrjByIDRequest request);

        [OperationContract]
        Services.Projects.RemoveProjectFolders.RemoveProjectFoldersResponse RemoveProjectFolders(Services.Projects.RemoveProjectFolders.RemoveProjectFoldersRequest request);

        #region Servizi sviluppati per MIT
        /// <summary>
        /// Servizio per la creazione di un sottofascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        VtDocsWS.Services.Projects.CreateFolder.CreateFolderResponse CreateFolder(VtDocsWS.Services.Projects.CreateFolder.CreateFolderRequest request);

        /// <summary>
        /// Servizio per prelevare la struttura di un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        VtDocsWS.Services.Projects.GetProjectFolders.GetProjectFoldersResponse GetProjectFolders(VtDocsWS.Services.Projects.GetProjectFolders.GetProjectFoldersRequest request);
        #endregion
    }
}