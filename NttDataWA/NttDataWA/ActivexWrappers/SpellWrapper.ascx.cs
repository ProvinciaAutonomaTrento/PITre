using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace NttDataWA.ActivexWrappers
{
    public partial class SpellWrappers : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected bool UseActivexWrapper
        {
            get
            {
                return Configurations.UseActivexWrappersControl;
            }
        }

        /// <summary>
        /// restituisce un valore booleano che indica se abilitare o meno il controllo
        /// </summary>
        protected bool UseSpellWrapper
        {
            get
            {
                return Configurations.UseSpellWrapperControl;
            }
        }
    }
}