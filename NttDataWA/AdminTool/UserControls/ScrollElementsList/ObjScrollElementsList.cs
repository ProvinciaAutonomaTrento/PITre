using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SAAdminTool.UserControls.ScrollElementsList
{
    public class ObjScrollElementsList
    {

        public enum EmunSearchContext
        {
            RICERCA_DOCUMENTI,
            RICERCA_TRASM_DOC_TO_DO_LIST,
            RICERCA_TRASM_DOC,
            RICERCA_DOC_IN_FASC,
            RICERCA_FASCICOLI,
            RICERCA_TRASM_FASC_TO_DO_LIST,
            RICERCA_TRASM_FASC
        }

        public EmunSearchContext searchContext;
        public ArrayList objList = null;

        public int totalNumberOfElements;
        public int totalNumberOfPage;
        public int selectedElement;
        public int selectedPage;
        public int pageSize;        
    }   
}


