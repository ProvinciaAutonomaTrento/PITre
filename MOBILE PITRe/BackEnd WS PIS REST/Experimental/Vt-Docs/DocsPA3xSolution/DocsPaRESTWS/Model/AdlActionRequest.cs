using DocsPaVO.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocsPaRESTWS.Model
{
    public class AdlActionRequest
    {
        public string ADLAction { get; set; }
        public string IdElemento { get; set; }
        public string TipoElemento { get; set; }

        //public ADLActions AdlAction
        //{
        //    get;
        //    set;
        //}

        //public DocInfo DocInfo
        //{
        //    get;
        //    set;
        //}
        
        //public enum ADLActions
        //{
        //    ADD,
        //    REMOVE
        //}
    }
}