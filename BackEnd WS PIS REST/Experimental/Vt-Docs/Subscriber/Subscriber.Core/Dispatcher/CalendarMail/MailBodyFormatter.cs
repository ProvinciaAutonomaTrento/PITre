using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Dispatcher.CalendarMail
{
    /// <summary>
    /// Interfaccia per la formattazione del contenuto della mail
    /// </summary>
    public interface IMailBodyFormatter
    {
        /// <summary>
        /// Accodamento dati
        /// </summary>
        /// <param name="data"></param>
        void AddData(params string[] data);
    }

    /// <summary>
    /// Classe per la formattazione del body dell'appuntamento
    /// </summary>
    public class CalendarMailBodyFormatter : IMailBodyFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        private string _data = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cols"></param>
        public CalendarMailBodyFormatter()
        { }

        /// <summary>
        /// Accodamento dati
        /// </summary>
        /// <param name="data"></param>
        public void AddData(params string[] data)
        {
            if (!string.IsNullOrEmpty(this._data))
                this._data += @"\n";
                
            foreach (string item in data)
                this._data += item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._data;
        }
    }

    /// <summary>
    /// Classe per la formattazione del body della mail in html
    /// </summary>
    public class HtmlMailBodyFormatter : IMailBodyFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        private string _data = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cols"></param>
        public HtmlMailBodyFormatter()
        {
        }

        /// <summary>
        /// Accodamento dati
        /// </summary>
        /// <param name="data"></param>
        public void AddData(params string[] data)
        {
            if (!string.IsNullOrEmpty(this._data))
                this._data += "<BR />";

            foreach (string item in data)
                this._data += string.Format("{0}", item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._data;
        }
    }
}
