using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubrica.Library.Data
{
    /// <summary>
    /// Url associato al corrispondente, utilizzato per i servizi di interoperabilità
    /// semplificata
    /// </summary>
    public class UrlInfo
    {
        public UrlInfo() { }
        public UrlInfo(String url) { this.Url = url; }

        /// <summary>
        /// Url associata al corrispondente
        /// </summary>
        public String Url { get; set; }

        public override bool Equals(object obj)
        {
            return (obj is UrlInfo) && (obj as UrlInfo).Url == this.Url;
        }

        public override int GetHashCode()
        {
            return this.Url.GetHashCode();
        }

    }
}
