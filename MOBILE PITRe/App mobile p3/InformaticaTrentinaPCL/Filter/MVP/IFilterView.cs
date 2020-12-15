using System;
namespace InformaticaTrentinaPCL.Filter.MVP
{
    public interface IFilterView
    {
        void EnableButton(bool enabled);
       
        /// <summary>
        ///  Per gestire un nuovo filtro creato al click sul bottone 'CONFERMA'
        /// </summary>
        /// <param name="filterModel">Filter model.</param>
        void OnNewFilter(FilterModel filterModel);

        /// <summary>
        /// Per aggiornare i campi della view in caso di apertura con filtro precedentemente popolato
        /// </summary>
        /// <param name="filterModel">Filter model.</param>
        void UpdateFilterView(FilterModel filterModel); //

        void OnFilterError(string error);
    }
}
