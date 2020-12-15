using System;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assegna.MVPD
{
    public interface IAssignablePresenter : IBasePresenter
    {
        Task OnModelsPullToRefresh();
        Task OnFavoritePullToRefresh();
        void GetLists();
        void SearchCorrispondenti(string text, string ragione = "");
        void SetFavorite(AbstractRecipient recipient, bool favorite); //TODO generare un oggetto per gestire 'preferiti' e 'modelli' 
        void ClearSearch();
        void OnViewReady();
        void OnSelect(AbstractRecipient selected);
        void OnAssigneeReceivedFromChooser(AbstractRecipient selected);
    }
}
