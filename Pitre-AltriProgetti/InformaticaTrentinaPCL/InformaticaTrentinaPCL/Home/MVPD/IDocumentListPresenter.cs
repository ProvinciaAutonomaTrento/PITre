using System;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Home.MVP
{
    public interface IDocumentListPresenter : IBasePresenter
    {
        void LoadDocumentList();
        void OnTapDocument(AbstractDocumentListItem abstractDocument);
        void OnOpenDocument(AbstractDocumentListItem abstractDocument);
        void OnPullToRefresh();
        void SetFilterModel(FilterModel filterModel);
        void OpenFilterView();
        void ShareDocument(AbstractDocumentListItem abstractDocument, AbstractRecipient recipient);
        void OnShareDocumentReceived(string sharedUrl);
        void RemoveADLAction(AbstractDocumentListItem abstractDocument);
        void AddADLAction(AbstractDocumentListItem abstractDocument);
        void DoViewed(AbstractDocumentListItem abstractDocument);
        void OnDocumentsSignCadesActionFinished(bool success);
        void OnDocumentsSignCadesNotSignedActionFinished(bool success);
        void OnDocumentsSignPadesActionFinished(bool success);
        bool HasAttachments(AbstractDocumentListItem abstractDocument);
        bool HasDocumentFather(AbstractDocumentListItem abstractDocument);
        bool IsAttachment(AbstractDocumentListItem abstractDocument);
        void DoViewedADL(AbstractDocumentListItem abstractDocument);
    }
}
