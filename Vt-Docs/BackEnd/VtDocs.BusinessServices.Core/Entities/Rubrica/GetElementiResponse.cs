using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VtDocs.BusinessServices.Entities.Rubrica
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class GetElementiResponse : Response
    {
        /// <summary>
        /// Lista degli elementi restituiti dalla ricerca
        /// </summary>
        public DocsPaVO.rubrica.ElementoRubrica[] Elementi
        {
            get;
            set;
        }
    }
}
