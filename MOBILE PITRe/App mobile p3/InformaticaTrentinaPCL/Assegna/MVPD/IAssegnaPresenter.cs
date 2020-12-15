using System;
using InformaticaTrentinaPCL.Assign;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Assegna.MVPD
{
    public interface IAssegnaPresenter : IBasePresenter
    {
        void Trasmetti();
        void UpdateAssegnatario(AbstractRecipient assegnatario);
        void UpdateNote(string note);
        void UpdateRagione(Ragione ragione);
        void GetListaRagioni();
        void OnViewReady();

    }
}
