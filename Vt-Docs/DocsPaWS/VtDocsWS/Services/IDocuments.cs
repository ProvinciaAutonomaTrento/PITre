using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    /// <summary>
    ///  Definizione dei servizi per i documenti dei Product Integration Services.  
    /// </summary>
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IDocuments
    {
        [OperationContract]
        VtDocsWS.Services.Documents.AddDocInProject.AddDocInProjectResponse AddDocInProject(VtDocsWS.Services.Documents.AddDocInProject.AddDocInProjectRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.CreateDocument.CreateDocumentResponse CreateDocument(VtDocsWS.Services.Documents.CreateDocument.CreateDocumentRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.CreateDocument.CreateDocumentResponse CreateDocumentFromWord(VtDocsWS.Services.Documents.CreateDocument.CreateDocumentRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.EditDocStateDiagram.EditDocStateDiagramResponse EditDocStateDiagram(VtDocsWS.Services.Documents.EditDocStateDiagram.EditDocStateDiagramRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.EditDocument.EditDocumentResponse EditDocument(VtDocsWS.Services.Documents.EditDocument.EditDocumentRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.EditDocument.EditDocumentResponse ProtocolPredisposed(VtDocsWS.Services.Documents.ProtocolPredisposed.ProtocolPredisposedRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetDocument.GetDocumentResponse GetDocument(VtDocsWS.Services.Documents.GetDocument.GetDocumentRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetDocumentsInProject.GetDocumentsInProjectResponse GetDocumentsInProject(VtDocsWS.Services.Documents.GetDocumentsInProject.GetDocumentsInProjectRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetDocumentStateDiagram.GetDocumentStateDiagramResponse GetDocumentStateDiagram(VtDocsWS.Services.Documents.GetDocumentStateDiagram.GetDocumentStateDiagramRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetFileDocumentById.GetFileDocumentByIdResponse GetFileDocumentById(VtDocsWS.Services.Documents.GetFileDocumentById.GetFileDocumentByIdRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetFileWithSignatureAndSignerInfo.GetFileWithSignatureAndSignerInfoResponse GetFileWithSignatureAndSignerInfo(VtDocsWS.Services.Documents.GetFileWithSignatureAndSignerInfo.GetFileWithSignatureAndSignerInfoRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampResponse GetFileWithSignatureOrStamp(VtDocsWS.Services.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetTemplateDoc.GetTemplateDocResponse GetTemplateDoc(VtDocsWS.Services.Documents.GetTemplateDoc.GetTemplateDocRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetTemplatesDocuments.GetTemplatesDocumentsResponse GetTemplatesDocuments(VtDocsWS.Services.Documents.GetTemplatesDocuments.GetTemplatesDocumentsRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.SearchDocuments.SearchDocumentsResponse SearchDocuments(VtDocsWS.Services.Documents.SearchDocuments.SearchDocumentsRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.SendDocument.SendDocumentResponse SendDocument(VtDocsWS.Services.Documents.SendDocument.SendDocumentRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse UploadFileToDocument(VtDocsWS.Services.Documents.UploadFileToDocument.UploadFileToDocumentRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse UploadFileToDocumentFromWord(VtDocsWS.Services.Documents.UploadFileToDocument.UploadFileToDocumentRequest request, string creationDate);

        [OperationContract]
        VtDocsWS.Services.Documents.GetStampAndSignature.GetStampAndSignatureResponse GetStampAndSignature(VtDocsWS.Services.Documents.GetStampAndSignature.GetStampAndSignatureRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetDocumentFilters.GetDocumentFiltersResponse GetDocumentFilters(VtDocsWS.Services.Documents.GetDocumentFilters.GetDocumentFiltersRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectResponse CreateDocumentAndAddInProject(VtDocsWS.Services.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectRequest request);

        [OperationContract]
        VtDocsWS.Services.FollowDomainObject.FollowResponse FollowDocument(VtDocsWS.Services.FollowDomainObject.FollowRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetLinkDocByID.GetLinkDocByIDResponse GetLinkDocByID(VtDocsWS.Services.Documents.GetLinkDocByID.GetLinkDocByIDRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.GetEnvelopedFileById.GetEnvelopedFileByIdResponse GetEnvelopedFileById(VtDocsWS.Services.Documents.GetEnvelopedFileById.GetEnvelopedFileByIdRequest request);

        [OperationContract]
        VtDocsWS.Services.Documents.AddAttachment.AddAttachmentResponse AddAttachment(VtDocsWS.Services.Documents.AddAttachment.AddAttachmentRequest request);
    }
}