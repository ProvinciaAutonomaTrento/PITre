using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.FullTextSearch
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SearchTextOptionsRadio : System.Web.UI.UserControl, ISearchTextOptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                // Impostazione default
                this.searchTextOptionsList.SelectedValue = SearchTextOptionsEnum.InitWithWord.ToString();
            }
        }

        /// <summary>
        /// Reperimento della modalità di ricerca testo correntemente selezionata
        /// </summary>
        /// <returns></returns>
        public SearchTextOptionsEnum GetSearchTextOptions()
        {
            return (SearchTextOptionsEnum) Enum.Parse(typeof(SearchTextOptionsEnum), this.searchTextOptionsList.SelectedValue, true);
        }

        /// <summary>
        /// Impostazione modalità di reperimento testuale
        /// </summary>
        /// <param name="searchOptions"></param>
        public void SetSearchTextOptions(SearchTextOptionsEnum searchOptions)
        {
            this.searchTextOptionsList.SelectedValue = searchOptions.ToString();
        }
    }
}