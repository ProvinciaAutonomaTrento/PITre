using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DocsPAWA.models
{
    public class ModelProviderFactory<T>
    {

        // Singleton del provider
        private static T instance;

        private static object syncRoot = new Object();

        /// <summary>
        /// Funzione per il reperimento di una istanza del provider
        /// </summary>
        /// <param name="mTextServiceUrl">Url del servizio</param>
        /// <returns>Istanza del provider</returns>
        public static T GetInstance()
        {
            lock (syncRoot)
            {
                if (instance == null)
                {
                    Type tipo = typeof(T);

                    String url = ConfigurationManager.AppSettings[tipo.Name];
                    if (String.IsNullOrEmpty(url))
                        url = "http://localhost:8080";
                    // Costruisci istanza
                    instance = (T) Activator.CreateInstance(tipo, new object[1] { url });
                }
            }

            return instance;
        }
    }
}
