using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class GetListaCorrVeloceRequest
    {
         public string Descrizione
        {
            get;
            set;
        }

        //public int NumMaxResults
        //{
        //    get;
        //    set;
        //}


        //public int numMaxResultsForCategory
        //{
        //    get;
        //    set;
        //}

        // MEV MOBILE
        public string Ragione
        {
            get;
            set;
        }
    }
    
}