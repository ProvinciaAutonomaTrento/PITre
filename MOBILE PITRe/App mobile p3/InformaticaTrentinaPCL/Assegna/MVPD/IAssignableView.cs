using System.Collections.Generic;
using InformaticaTrentinaPCL.Delega;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Login;

namespace InformaticaTrentinaPCL.Assegna.MVPD
{
    public interface IAssignableView : IGeneralView
    {
        void UpdateModel(List<AbstractRecipient> listModels);
        void UpdateFavorite(List<AbstractRecipient> listFavorites);
        void UpdateSearchResults(List<AbstractRecipient> listResults);
        void ShowTabsView(bool show);
        void ShowSearchView(bool show);
        void OnFavoriteError(AbstractRecipient recipient);
        void ShowFavoriteError(string message);
        void OnAssigneeSelected(AbstractRecipient selected); //Notifica alla view di tornare indietro alla view di assegna
        void UserSelected(AbstractRecipient abstractRecipient); //Notifica alla view per mostrare la scelta di un ruolo eventuale per il Corri...Model selezionato;
    }
}
