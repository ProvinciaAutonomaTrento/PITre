using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using DocsPaConservazione.Metadata.Common;
using DocsPaVO.areaConservazione;
using DocsPaVO.amministrazione;
using BusinessLogic.Utenti;
using System.Text.RegularExpressions;
using System.Collections;
using BusinessLogic.ExportDati;
using System.Globalization;

namespace DocsPaConservazione.Metadata
{
    public static  class Utils
    {
        static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        public static String SerializeObject<t>(Object pObject, bool nsNull)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                if (nsNull)
                {
                    ns.Add("", "");
                    xs.Serialize(xmlTextWriter, pObject, ns);
                } else {
                    ns.Add("sincro", "http://www.cnipa.gov.it/sincro/");
                    xs.Serialize(xmlTextWriter, pObject,ns);   
                }
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                memoryStream.Position = 0;
                //XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                StreamReader sr = new StreamReader(memoryStream);
                XmlizedString = sr.ReadToEnd();
                return XmlizedString;
            }
            catch (Exception e) { System.Console.WriteLine(e); return null; }
        }
        public static String formattaData(DateTime data)
        {
            return String.Format("{0}-{1}-{2}", data.Year, data.Month.ToString().PadLeft(2, '0'), data.Day.ToString().PadLeft(2, '0'));
        }
        public static String formattaOra(DateTime data)
        {
            return String.Format("{0}:{1}", data.Hour.ToString().PadLeft(2, '0'), data.Minute.ToString().PadLeft(2, '0'));
        }
        public static String formattaOraScondi(DateTime data)
        {
            return String.Format("{0}:{1}:{2}", data.Hour.ToString().PadLeft(2, '0'), data.Minute.ToString().PadLeft(2, '0'), data.Second.ToString().PadLeft(2, '0'));
        }
        public static String FormattaDataOraIta(DateTime data)
        {
            string result = string.Empty;
            result = data.ToString("dd/MM/yyyy HH:mm:ss");
            return result;
        }
        public static DateTime convertiData(string data)
        {
            DateTime dt = DateTime.Now;
            DateTime.TryParse(data, out dt);
            return dt;
        }

        public static UnitaOrganizzativa convertiUO(DocsPaVO.utente.UnitaOrganizzativa unitaOrganizzativa)
        {
            UnitaOrganizzativa uoXML = new UnitaOrganizzativa();
            uoXML.CodiceUO = unitaOrganizzativa.codiceRubrica;
            uoXML.DescrizioneUO = unitaOrganizzativa.descrizione;
            uoXML.Livello = unitaOrganizzativa.livello;

            if (unitaOrganizzativa.parent == null)
                return uoXML;

            UnitaOrganizzativa uoXML_Padre = convertiUO(unitaOrganizzativa.parent);
            List<UnitaOrganizzativa> uoXMLLst = new List<UnitaOrganizzativa>();
            uoXMLLst.Add(uoXML_Padre);
            uoXML.UnitaOrganizzativa1 = uoXMLLst.ToArray();

            return uoXML;
        }

        public static DocsPaConservazione.Metadata.Common.Amministrazione getInfoAmministrazione (InfoConservazione infoCons)
        {
            InfoAmministrazione infoAmm = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoCons.IdAmm);
            return new Metadata.Common.Amministrazione { CodiceAmministrazione = infoAmm.Codice , DescrizioneAmministrazione = infoAmm.Descrizione};
        }

        public static DocsPaConservazione.Metadata.Common.Creatore getCreatore(InfoConservazione infoCons, DocsPaVO.utente.Ruolo ruolo)
        {
            Creatore creatore = new Creatore();
            creatore.CodiceRuolo = ruolo.codiceRubrica;
            creatore.DescrizioneRuolo = ruolo.descrizione;
            creatore.CodiceUtente = infoCons.userID;
            creatore.DescrizioneUtente = UserManager.getUtente(infoCons.IdPeople).descrizione;
            return creatore;
        }

        public static string convertiLivelloRiservatezza(string livello)
        {
            if (livello != null && livello.Equals ("1"))
                return "privato";
            else
               return string.Empty;

        }
        public static string convertiTipoPoto(DocsPaVO.documento.SchedaDocumento schDoc)
        {
            string retval = schDoc.tipoProto;
            switch (schDoc.tipoProto)
            {

                case "A":
                case "P":
                case "I":
                    {
                        retval = "Protocollato";
                        if (schDoc.protocollo != null)
                            if (string.IsNullOrEmpty(schDoc.protocollo.segnatura))
                                retval = "Predisposto";
                    }
                    break;

                case "G":
                    retval = "Grigio";
                    break;
            }
            return retval;
        }

        public static CampoTipologia[] getCampiTipologia(DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            if (template == null)
                return null;

            List<CampoTipologia> ctlist = new List<CampoTipologia>();

            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
            {
                ExportDatiManager exportDatiManager = new ExportDatiManager();
                CampoTipologia ct = new CampoTipologia
                {
                    NomeCampo = oggettoCustom.DESCRIZIONE,
                    ValoreCampo = exportDatiManager.getValoreOggettoCustom(oggettoCustom)
                };
                ctlist.Add(ct);
            }
            return ctlist.ToArray();
        }
  
    }

    public class CnipaParser
    {

        private string GetSubjectItem(Hashtable subjectItems, string itemKey)
        {
            if (subjectItems.ContainsKey(itemKey))
                return subjectItems[itemKey].ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Estrazione dei singoli elementi che compongono
        /// la descrizione del firmatario del certificato
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        private Hashtable ParseCNIPASubjectItems(string subject)
        {
            Hashtable retValue = new Hashtable();

            string[] items = subject.Split(',');
            Regex regex = new Regex("[A-Za-z]*=[^\"][^,]*");
            MatchCollection matchColl = regex.Matches(subject);
            for (int i = 0; i < matchColl.Count; i++)
            {
                string item = matchColl[i].Value;
                int indexOfValue = item.IndexOf("=");
                string itemKey = item.Substring(0, indexOfValue).ToUpper().Trim();
                string itemValue = item.Substring(indexOfValue + 1).ToUpper().Trim();
                retValue[itemKey] = itemValue;
            }
            Regex regex2 = new Regex("[A-Za-z]*=\"[\\d\\D]*\"");
            MatchCollection matchColl2 = regex2.Matches(subject);
            for (int i = 0; i < matchColl2.Count; i++)
            {
                string item = matchColl2[i].Value;
                int indexOfValue = item.IndexOf("=");
                string itemKey = item.Substring(0, indexOfValue).ToUpper().Trim();
                string itemValue = item.Substring(indexOfValue + 1).ToUpper().Trim();
                retValue[itemKey] = itemValue;
            }

            return retValue;
        }

        /// <summary>
        /// Parse Certificate Description
        /// </summary>
        /// <param name="sd">a structure filled with results</param>
        /// <param name="subject">the input string</param>
        public void ParseCNIPASubjectInfo(ref DocsPaVO.documento.SubjectInfo subjectInfo, string subject)
        {
            Regex r = new Regex(".*?CN=(?<cognome>.*?)/(?<nome>.*?)/(?<cf>.*?)/(?<cid>.*?),.*", RegexOptions.IgnoreCase);
            Match match = r.Match(subject);

            if (match.Success)
            {
                // Formato supportato dalla normativa 2000
                string commonName = match.Groups["cognome"].Value + " " + match.Groups["nome"].Value;
                if (commonName.Trim() != string.Empty)
                    subjectInfo.CommonName = commonName;

                subjectInfo.CodiceFiscale = match.Groups["cf"].Value;
                subjectInfo.CertId = match.Groups["cid"].Value;
            }

            r = new Regex(@"Description=\""C=(?<cognome>.*?)/N=(?<nome>.*?)/D=(?<g>\d+)-(?<m>\d+)-(?<a>\d+)(/R=(?<r>.*?))?\""(.*C=(?<country>.*?)$)?", RegexOptions.IgnoreCase);
            match = r.Match(subject);

            if (match.Success)
            {
                // Formato supportato dalla normativa 2000
                subjectInfo.Cognome = match.Groups["cognome"].Value;
                subjectInfo.Nome = match.Groups["nome"].Value;
                subjectInfo.DataDiNascita =
                    new DateTime(Int32.Parse(match.Groups["a"].Value),
                                 Int32.Parse(match.Groups["m"].Value),
                                 Int32.Parse(match.Groups["g"].Value)).ToShortDateString();

                subjectInfo.Ruolo = match.Groups["r"].Value;
                subjectInfo.Country = match.Groups["country"].Value;
            }
            else
            {
                // Formato supportato dalla normativa del 17/02/2005
                Hashtable subjectItems = this.ParseCNIPASubjectItems(subject);

                subjectInfo.CommonName = this.GetSubjectItem(subjectItems, "CN");
                subjectInfo.Cognome = this.GetSubjectItem(subjectItems, "SN");
                subjectInfo.Nome = this.GetSubjectItem(subjectItems, "G");
                subjectInfo.CertId = this.GetSubjectItem(subjectItems, "DNQUALIFIER");
                subjectInfo.SerialNumber = this.GetSubjectItem(subjectItems, "OID.2.5.4.5");
                
                // Se il country code è "IT", il "SerialNumber" contiene il codice fiscale del titolare del certificato
                if (subjectInfo.SerialNumber.StartsWith("IT:"))
                    subjectInfo.CodiceFiscale = subjectInfo.SerialNumber.Substring(subjectInfo.SerialNumber.IndexOf(":") + 1);
                subjectInfo.Country = this.GetSubjectItem(subjectItems, "C");

                if (subjectInfo.CodiceFiscale == null)
                    subjectInfo.CodiceFiscale = this.GetSubjectItem(subjectItems, "SERIALNUMBER");

                subjectItems.Clear();
                subjectItems = null;
            }
        }
    }

}
