using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.ChangeRole;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;

namespace InformaticaTrentinaPCL.Assegna.MVPD
{
    public interface ISelectRoleView : IGeneralView
    {
        void UpdateView(AbstractRecipient user, List<RuoloInfo> listRoles);
        void OnFavoriteError(AbstractRecipient recipient);
        void ShowFavoriteError(string message);
        void GoBack(AbstractRecipient abstractRecipient, bool isFavoriteChanged);
    }
}
