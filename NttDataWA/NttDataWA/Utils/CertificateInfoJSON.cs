using NttDataWA.DocsPaWR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace NttDataWA.Utils
{
    public class CertificateInfoJSON
    {
        
        public string DigestAlgorithm;
        public string IssuerName;
        public string SerialNumber;
        public string SubjectName;
        public string ThumbPrint;
        public string ValidFromDate;
        public string ValidToDate;
        public string Version;
        public string X509Certificate;





        public CertificateInfo getCertificateInfo()
        {
            CertificateInfo certificateInfo = new CertificateInfo();
            certificateInfo.SignatureAlgorithm = this.DigestAlgorithm;
            certificateInfo.IssuerName = this.IssuerName;
            certificateInfo.SerialNumber = this.SerialNumber;
            certificateInfo.SubjectName = this.SubjectName;
            certificateInfo.ThumbPrint = this.ThumbPrint;
            certificateInfo.ValidFromDateStr = this.ValidFromDate;
            certificateInfo.ValidToDateStr = this.ValidToDate;
            if (!string.IsNullOrEmpty(this.X509Certificate))
                certificateInfo.X509Certificate = Convert.FromBase64String(this.X509Certificate);
            return certificateInfo;

        }
    }
}