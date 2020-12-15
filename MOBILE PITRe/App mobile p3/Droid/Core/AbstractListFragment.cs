
using System.Collections.Generic;
using Android.App;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Home;
using Java.Lang;

namespace InformaticaTrentinaPCL.Droid.Core
{
    public abstract class AbstractListFragment : Fragment
    {
        
        public virtual void RequestOpenFilterView()
        {
            throw new RuntimeException("Not Supported for this fragment!");
        }

        public virtual void RequestShowActionsDocuments()
        {
            throw new RuntimeException("Not Supported for this fragment!");
        }

        
        /// <summary>
        /// Metodo da implementare nei fragment dove è possibile utilizzare i filtri
        /// </summary>
        /// <param name="filterModel">Filter model.</param>
        public virtual void SetFilter(FilterModel filterModel)
        {
            throw new RuntimeException("Not Supported for this fragment!");
        }

        /// <summary>
        /// Metodo da implementare nei fragment dove è possibile utilizzare i filtri
        /// </summary>
        /// <returns>The filter.</returns>
        public FilterModel GetFilter()
        {
            throw new RuntimeException("Not Supported for this fragment!");
        }

        /// <summary>
        /// Utility method per verificare se il fragment ha un filtro attivo.
        /// </summary>
        /// <returns><c>true</c>, if fragment has filter active, <c>false</c> otherwise.</returns>
        public bool HasaFilterActive()
        {
            throw new RuntimeException("Not Supported for this fragment!");
        }

        /// <summary>
        /// Metodo da implementare per ricaricare i dati del fragment.
        /// </summary>
        public abstract void reloadData();
    }
}
