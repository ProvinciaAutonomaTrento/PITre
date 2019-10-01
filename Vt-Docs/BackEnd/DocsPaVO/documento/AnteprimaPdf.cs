using System;
using System.Collections;
using System.Xml.Serialization;

namespace DocsPaVO.documento
{
    /// <summary>
    /// </summary>
    [XmlInclude(typeof(VerifySignatureResult))]
    [Serializable()]
    public class AnteprimaPdf
    {
        public string docNumber; 
        public string versionId;
        public string pathFile;
        public string hashFile;
        public int totalPageNumber;
        public int previewPageNamber;
        public int previewPageFrom;

        public string fileName
        {
            get
            {
                string retVal = string.Empty;

                if (!string.IsNullOrEmpty(pathFile) && pathFile.Contains("/"))
                {
                    string[] tempColl = pathFile.Split('/');
                    retVal = tempColl[tempColl.Length - 1];
                }

                return retVal;
            }
        }
    }
}
