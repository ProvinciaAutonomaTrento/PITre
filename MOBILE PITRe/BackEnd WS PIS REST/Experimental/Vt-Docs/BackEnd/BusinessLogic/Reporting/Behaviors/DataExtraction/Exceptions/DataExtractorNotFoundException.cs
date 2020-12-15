using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Reporting.Behaviors.DataExtraction.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando una classe di estrazione dati con query list non viene trovata o
    /// se viene trovata ma non contiene un metodo decorato con l'attributo ReportDataExtractor
    /// </summary>
    [Serializable]
    public class DataExtractorNotFoundException : Exception
    {
        public DataExtractorNotFoundException() { }
        public DataExtractorNotFoundException(string message) : base(message) { }
        public DataExtractorNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected DataExtractorNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
