using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Conservazione
{
    [Serializable()]
    public class ExecutionPolicy
    {
        public string idExecution = string.Empty;
        public string idPolicy = string.Empty;
        public string startExecutionDate = string.Empty;
        public string endExecutionDate = string.Empty;
        public string docNumberProcessed = string.Empty;
        public string idLastDocumentProcessed = string.Empty;
        public string idFirstDocumentProcessed = string.Empty;
        public string idIstanza = string.Empty;
        public string idAmm = string.Empty;
        public bool pending = false;
    }
}
