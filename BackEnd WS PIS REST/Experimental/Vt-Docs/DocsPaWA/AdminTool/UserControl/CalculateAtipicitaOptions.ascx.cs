using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.UserControl
{
    public partial class CalculateAtipicitaOptions : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// True se bisogna calcolare l'atipicità
        /// </summary>
        /// <returns></returns>
        public bool CalculateAtipicita()
        {
            return this.optCalculate.Checked;
        }
    }
}