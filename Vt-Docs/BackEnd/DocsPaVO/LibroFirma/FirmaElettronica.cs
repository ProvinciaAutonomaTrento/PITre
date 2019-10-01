using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace DocsPaVO.LibroFirma
{
    public class FirmaElettronica
    {
        #region private fields

        private string _docnumber;
        private string _versionid;
        private string _docAll;
        private string _numAll;
        private string _numVersione;
        private string _imponta;
        private string _idFirma;
        private string _xml;
        private string _dataApposizione;
        private string _firmatario;

        #endregion

        #region public property


        public string Docnumber
        {
            get { return _docnumber; }
            set { _docnumber = value; }
        }


        public string Versionid
        {
            get { return _versionid; }
            set { _versionid = value; }
        }

        public string DocAll
        {
            get { return _docAll; }
            set { _docAll = value; }
        }

        public string NumAll
        {
            get { return _numAll; }
            set { _numAll = value; }
        }

        public string NumVersione
        {
            get { return _numVersione; }
            set { _numVersione = value; }
        }

        public string Imponta
        {
            get { return _imponta; }
            set { _imponta = value; }
        }

        public string IdFirma
        {
            get { return _idFirma; }
            set { _idFirma = value; }
        }

        public string Xml
        {
            get { return _xml; }
            set { _xml = value; }
        }

        public string DataApposizione
        {
            get { return _dataApposizione; }
            set { _dataApposizione = value; }
        }

        /// <summary>
        /// Firmatario
        /// </summary>
        public string Firmatario
        {
            get
            {
                return _firmatario;
            }
            set
            {
                _firmatario = value;
            }
        }

        public void GeneraXML(string utente, string ruolo, string delegato, string idRuolo, string idPeople)
        {
            if (!string.IsNullOrEmpty(this.Xml))
            {
                utente = utente.Replace("'", "''");
                ruolo = ruolo.Replace("'", "''");
                delegato = delegato.Replace("'", "''");

                XmlDocument XML_Firma = new XmlDocument();
                XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(XML_Firma.NameTable);

                XML_Firma.LoadXml(this.Xml);

                XmlNode node = XML_Firma.SelectSingleNode("xml/FirmaElettronica/Documento", xmlnsManager);
                node.Attributes["id"].Value = this.Docnumber;

                node = XML_Firma.SelectSingleNode("xml/FirmaElettronica/Documento/Imponta", xmlnsManager);
                node.InnerText = this.Imponta;

                node = XML_Firma.SelectSingleNode("xml/FirmaElettronica/Firmatario", xmlnsManager);
                node.Attributes["delega"].Value = delegato;

                node = XML_Firma.SelectSingleNode("xml/FirmaElettronica/Firmatario/Ruolo", xmlnsManager);
                node.Attributes["id"].Value = idRuolo;
                node.InnerText = ruolo;

                node = XML_Firma.SelectSingleNode("xml/FirmaElettronica/Firmatario/Utente", xmlnsManager);
                node.Attributes["id"].Value = idPeople;
                node.InnerText = utente;

                node = XML_Firma.SelectSingleNode("xml/FirmaElettronica/DataCreazione", xmlnsManager);
                node.InnerText = this.DataApposizione;

                this.Xml = XML_Firma.InnerXml;
            }
        }

        public void UpdateXml(string improntaFile, string nuovoIdVersione, string numeroVersione)
        {
            if(!string.IsNullOrEmpty(this.Xml))
            {
                XmlDocument XML_Firma = new XmlDocument();
                XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(XML_Firma.NameTable);

                XML_Firma.LoadXml(this.Xml);

                XmlNode node = XML_Firma.SelectSingleNode("xml/FirmaElettronica/Documento", xmlnsManager);
            
                XmlElement improntaNew = XML_Firma.CreateElement("Imponta");
                improntaNew.InnerText = improntaFile;
            
                node.AppendChild(improntaNew);

                this.Versionid = nuovoIdVersione;
                this.NumVersione = numeroVersione;

                this.Xml = XML_Firma.InnerXml;
            }
        }

        #endregion
    }
}
