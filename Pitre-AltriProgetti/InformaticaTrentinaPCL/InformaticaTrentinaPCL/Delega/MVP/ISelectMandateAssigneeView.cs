using System.Collections.Generic;
using InformaticaTrentinaPCL.Delega.Network;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Delega.MVP
{
    public interface ISelectMandateAssigneeView : IGeneralView
    {
        void UpdateFavoriteList(List<AbstractRecipient> list);
        void UpdateSearchList(List<AbstractRecipient> list);
        void ClearList();
        void OnFavoriteError(AbstractRecipient recipient);
        void ShowFavoriteError(string message);
    }
}
