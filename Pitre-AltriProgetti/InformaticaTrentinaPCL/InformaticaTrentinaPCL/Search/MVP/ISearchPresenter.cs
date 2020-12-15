using System;
using InformaticaTrentinaPCL.Home.MVP;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Search.MVP
{
    public interface ISearchPresenter : IDocumentListPresenter, IBasePresenter
    {
        /// <summary>
        /// Chiamato quando si invia la stringa per la ricerca
        /// </summary>
        /// <param name="text"></param>
        void UpdateSearchString(string text);
        
        /// <summary>
        /// Invocato quando si clicca su cancella
        /// </summary>
        void ClearSearch();
        
        /// <summary>
        /// Usato per richiedere la pagina successiva
        /// </summary>
        void DoNextResultPage();

    }
}
