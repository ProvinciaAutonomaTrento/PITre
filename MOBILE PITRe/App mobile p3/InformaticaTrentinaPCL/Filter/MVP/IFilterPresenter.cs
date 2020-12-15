using System;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;

namespace InformaticaTrentinaPCL.Filter.MVP
{
    public interface IFilterPresenter
    {
        void UpdateStartDate(DateTime startDate);
        void UpdateEndDate(DateTime endDate);
        void UpdateType(TypeDocument type);
        void OnViewReady();
        void OnConfirmButton();
        void UpdateIdDocument(string idDocument);
        void UpdateOggetto(string oggetto);
        void UpdateNumberProtocol(string numProtocol);
        void UpdateYearProtocol(string yearProtocol);
        void UpdateStartDateProtocol(DateTime startDateProtocol);
        void UpdateEndDateProtocol(DateTime startEndProtocol);
    }
}
