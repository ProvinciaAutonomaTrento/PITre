using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DocsPaVO.Report;

namespace DocsPaDB.Query_DocsPAWS.Reporting
{
    /// <summary>
    /// Questa classe fornisce funzioni di utilità per la ricerca di metodi dedicati alla generazione
    /// di dati per la reportistica
    /// </summary>
    public sealed class ReportMethodFinder
    {
        /// <summary>
        /// Metodo per la ricerca di un metodo che genera i dati relativi ad un report
        /// </summary>
        /// <param name="contextName">Nome del report (Chiave di report)</param>
        /// <returns>Metodo da richiamare per l'esecuzione dell'estrazione dati</returns>
        public static MethodInfo FindMethod(string contextName)
        {
            // Recupero del tipo di questa classe
            Type myType = typeof(ReportMethodFinder);

            // Reperimento tipi decorati dall'attributo "ReportDataExtractorClassAttribute"
            Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, myType.Namespace, StringComparison.Ordinal) &&
                                    t.GetCustomAttributes(typeof(ReportDataExtractorClassAttribute), true).Length > 0).ToArray();

            // Metodo da restituire
            MethodInfo methodInfo = null;
            
            foreach (Type t in types)
            {
                // Reperimento di tutti i metodi del tipo decorati con l'attributo "ReportDataExtractor"
                var method = t.GetMethods().Where(m => m.GetCustomAttributes(typeof(ReportDataExtractorMethodAttribute), true).Length > 0).ToArray();

                foreach (var m in method)
                {
                    DocsPaVO.Report.ReportDataExtractorMethodAttribute[] attrs = (ReportDataExtractorMethodAttribute[])m.GetCustomAttributes(typeof(ReportDataExtractorMethodAttribute), false);

                    if (attrs[0].ContextName == contextName)
                    {
                        // Controllo sui parametri
                        ParameterInfo[] paramsInfo = m.GetParameters();
                        
                        if(paramsInfo.Length == 2)
                        {
                            methodInfo = m;
                            break;
                        }
                    }
                }

                if (methodInfo != null)
                    break;
            }

            // Restituzione del metodo trovato
            return methodInfo;
        }

    }

}
