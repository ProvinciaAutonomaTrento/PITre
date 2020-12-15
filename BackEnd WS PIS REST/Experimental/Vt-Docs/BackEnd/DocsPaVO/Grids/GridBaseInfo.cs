using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Grid
{
    /// <summary>
    /// Informazioni di base sulla griglia
    /// </summary>
    public class GridBaseInfo
    {
        /// <summary>
        /// Nome assegnato alla griglia
        /// </summary>
        public String GridName { get; set; }

        /// <summary>
        /// Id della griglia
        /// </summary>
        public String GridId { get; set; }

        /// <summary>
        /// True se la griglia è temporanea
        /// </summary>
        public Boolean IsSearchGrid { get; set; }

        /// <summary>
        /// checked se la griglia è la preferita
        /// </summary>
        public Boolean IsPreferred { get; set; }

        /// <summary>
        /// checked se la griglia è a ruolo
        /// </summary>
        public Boolean RoleGrid { get; set; }

        /// <summary>
        /// checked se la griglia è a utente
        /// </summary>
        public Boolean UserGrid { get; set; }

        /// <summary>
        /// tipo di griglia
        /// </summary>
        public string GridType { get; set; }
    }
}
