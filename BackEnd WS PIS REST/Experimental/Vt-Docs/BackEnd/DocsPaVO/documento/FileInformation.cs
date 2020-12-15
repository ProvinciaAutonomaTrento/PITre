using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.documento
{
    //Carta identità documento
    public class FileInformation
    {

        //Campo sul DB di 64 byte:

        //  00000000001111111111222222222233333333334444444444555555555566666
        //  01234567890123456789012345678901234567890123456789012345678901234
        //  --------------------YYYYMMDDYYYYMMDDYYYYMMDDVVAAXXXXXXXXXXXFTCsNS

        //  66666555555555544444444443333333333222222222211111111110000000000
        //  43210987654321098765432109876543210987654321098765432109876543210                                         
        //  --------------------YYYYMMDDYYYYMMDDYYYYMMDDVVAAXXXXXXXXXXXFTCsNS
        //  1    status globale
        //  2    NoMacroOrExe
        //  3    Signature
        //  4    CrlStatus
        //  5    TimeStampStatus
        //  6    FileFormatOK
        //  7    HashOK
        // ..    RFU
        // ..    RFU
        // ..    RFU
        // 15    RFU
        // 16    Signable
        // 17    Preservable
        // 18    pdf T (pdfa pdxe pdfxe)
        // 19    pdfVerMinor
        // 20    pdfVerMajor
        // 28-21 YYYYMMDD  data Acquisizione FILE                  
        // 36-29 YYYYMMDD  data controllo FILE                  
        // 44-37 YYYYMMDD  data di riferimento per controllo CRL
        // 64-45 RFU 

        const int STATUS_FIELD_LEN = 64;

        public enum DateType
        {
            ADMREFDATE = 28,
            CHKREFDATE = 36,
            CRLREFDATE = 20,
        }

        public enum VerifyStatus
        {
            Invalid       = 0, //0
            Valid         = 1, //1
            Expired       = 2, //2
            //RFU3
            //RFU4
            //RFU5
            //RFU6
            Untested      = 7, //7
            InProgress    = 8, //8
            NotApplicable = 9  //9
        }
        //DB Filed CHAR64

        public VerifyStatus Status              = VerifyStatus.InProgress;                 //0 Global Status
        public VerifyStatus NoMacroOrExe        = VerifyStatus.Untested;                   //1 
        public VerifyStatus Signature           = VerifyStatus.Untested;                   //2
        public VerifyStatus CrlStatus           = VerifyStatus.Untested;                   //3
        public VerifyStatus TimeStampStatus     = VerifyStatus.Untested;                   //4
        public VerifyStatus FileFormatOK        = VerifyStatus.Untested;                   //5
        public VerifyStatus FileHashOK          = VerifyStatus.Untested;                   //6
        
        //MiniFoto dei Formati in amministrazione
        public VerifyStatus Signable            = VerifyStatus.Untested;                   //15 Ammesso alla firma
        public VerifyStatus Preservable         = VerifyStatus.Untested;                   //16 Ammesso alla conservazione
        
        public DateTime CheckRefDate            = DateTime.MinValue;
        public DateTime AdminRefDate            = DateTime.MinValue;
        public DateTime CrlRefDate              = DateTime.MinValue;
        public string PdfVer = null;

        /// <summary>
        /// Decodes an input field coming from the db 
        /// </summary>
        /// <param name="inputfield"></param>
        /// <returns></returns>
        public static FileInformation decodeMask(string inputfield)
        {
            FileInformation retval = new FileInformation();
            if (String.IsNullOrEmpty(inputfield))
            {
                retval.Status = VerifyStatus.Untested;
                return retval;
            }
            else
            {
                retval.Status = retval.getValueAt(inputfield, 1);
                retval.NoMacroOrExe = retval.getValueAt(inputfield, 2);
                retval.Signature = retval.getValueAt(inputfield, 3);
                retval.CrlStatus = retval.getValueAt(inputfield, 4);
                retval.TimeStampStatus = retval.getValueAt(inputfield, 5);
                retval.FileFormatOK = retval.getValueAt(inputfield, 6);
                retval.FileHashOK = retval.getValueAt(inputfield, 7);
                retval.Signable = retval.getValueAt(inputfield, 16);
                retval.Preservable = retval.getValueAt(inputfield, 17);
                retval.PdfVer = getPdfVer(inputfield);
                retval.CheckRefDate = decodeDate(inputfield, DateType.CHKREFDATE);
                retval.AdminRefDate = decodeDate(inputfield, DateType.ADMREFDATE);
                retval.CrlRefDate = decodeDate(inputfield, DateType.CRLREFDATE);
            }
            return retval;
        }

        public static string encodeDate(DateTime date, string inputfield, DateType dateType)
        {
            if (inputfield.Length == 64)
            {
                int index = (int)dateType;
                string data = date.ToString("yyyyMMdd");
                var aStringBuilder = new StringBuilder(inputfield);
                aStringBuilder.Remove(index, data.Length);
                aStringBuilder.Insert(index, data);
                return aStringBuilder.ToString();
            }
            return inputfield;
        }

        public static DateTime decodeDate(string inputfield, DateType dateType)
        {
            DateTime outDate = DateTime.Now;
            if (inputfield.Length == 64)
            {
                int index = (int)dateType;
                string data = inputfield.Substring(index, 8);
                DateTime.TryParseExact(data, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out outDate);
                return outDate;
            }
            return outDate;
        }

        public static string encodeMask(FileInformation fileInfo)
        {
            string retval = resetValue();
            if (fileInfo == null)
                return retval;

            retval = setValueAt(retval, 1, fileInfo.Status);
            retval = setValueAt(retval, 2, fileInfo.NoMacroOrExe);
            retval = setValueAt(retval, 3, fileInfo.Signature);
            retval = setValueAt(retval, 4, fileInfo.CrlStatus);
            retval = setValueAt(retval, 5, fileInfo.TimeStampStatus);
            retval = setValueAt(retval, 6, fileInfo.FileFormatOK);
            retval = setValueAt(retval, 7, fileInfo.FileHashOK);
            retval = setValueAt(retval, 16, fileInfo.Signable);
            retval = setValueAt(retval, 17, fileInfo.Preservable);
            retval = encodeDate(fileInfo.CheckRefDate, retval, DateType.CHKREFDATE);
            retval = encodeDate(fileInfo.AdminRefDate, retval, DateType.ADMREFDATE);
            retval = encodeDate(fileInfo.CrlRefDate, retval, DateType.CRLREFDATE);

            if (!string.IsNullOrEmpty (fileInfo.PdfVer))
                retval = setPdfVer(retval,fileInfo.PdfVer);
            
            return retval;
        }

        private VerifyStatus getValueAt(string inputfield, int position)
        {
            if (position > inputfield.Length)
                return VerifyStatus.Invalid;

            if ((STATUS_FIELD_LEN-position)<0 ||
                ((STATUS_FIELD_LEN-position)>STATUS_FIELD_LEN))
                return VerifyStatus.Invalid;

            try
            {
                string strVal = inputfield.Substring(STATUS_FIELD_LEN-position, 1);
                int intVal;

                //nel caso il valore sta a resetval  (vecchi campi non valorizzati) torniamo untested.
                if (strVal.Equals("#"))
                    return VerifyStatus.Untested;

                Int32.TryParse(strVal, out intVal);
                return (VerifyStatus)intVal;
            }
            catch { } // non loggo :(

            return VerifyStatus.Invalid;
        }

        private static string resetValue()
        {
            string resetVal =  new String('#', STATUS_FIELD_LEN);
            resetVal =setValueAt(resetVal, 1, VerifyStatus.InProgress);
            resetVal = setValueAt(resetVal, 2, VerifyStatus.Untested);
            resetVal = setValueAt(resetVal, 3, VerifyStatus.Untested);
            resetVal = setValueAt(resetVal, 4, VerifyStatus.Untested);
            resetVal = setValueAt(resetVal, 5, VerifyStatus.Untested);
            resetVal = setValueAt(resetVal, 6, VerifyStatus.Untested);
            resetVal = setValueAt(resetVal, 7, VerifyStatus.Untested);
            resetVal = setValueAt(resetVal, 16, VerifyStatus.Untested);
            resetVal = setValueAt(resetVal, 17, VerifyStatus.Untested);
            return resetVal;
        }

        private static string setPdfVer(string inputfield,string pdfHeader)
        {
            try
            {
                string type=null;
                string[] versionChars = pdfHeader.Replace("%PDF-", string.Empty).Split('.');
                string[] typeArr = pdfHeader.Split('§');
                if (typeArr.Length > 1)
                {
                    type = encodeDecodePDFType(typeArr[1]);
                }

                if (versionChars.Length == 2)
                {
                    char[] chars = inputfield.ToCharArray();
                    chars[44] = versionChars[0].ToCharArray()[0];
                    chars[45] = versionChars[1].ToCharArray()[0];
                    if (!string.IsNullOrEmpty(type))
                        chars[46] = type.ToCharArray()[0];

                    return new string(chars);
                }
            }
            catch { }
            return inputfield;
        }

        public static string encodeDecodePDFType(string ext)
        {
            ext = ext.ToUpperInvariant();
            if (ext == "PDF/A") return "A";
            if (ext == "PDF/A-1") return "A";
            if (ext == "PDF/A-2") return "B";
            if (ext == "PDF/A-3") return "C";
            if (ext == "PDF/E") return "E";
            if (ext == "PDF/X") return "X";
            if (ext == "PDF/VT") return "V";
            if (ext == "PDF/UA") return "U";

            if (ext == "A") return "PDF/A";
            if (ext == "B") return "PDF/A-2";
            if (ext == "C") return "PDF/A-3";
            if (ext == "E") return "PDF/E";
            if (ext == "X") return "PDF/X";
            if (ext == "V") return "PDF/VT";
            if (ext == "U") return "PDF/UA";

            return "";
        }

        private static string getPdfVer(string inputfield)
        {
            string strVal1 = inputfield.Substring(44, 1);
            string strVal2 = inputfield.Substring(45, 1);
            string strType = inputfield.Substring(46, 1);
            if (strVal1 == "#" || strVal2 == "#")
                return null;
            string type = string.Empty;

            type = " " + encodeDecodePDFType(strType);

            return String.Format("{0}.{1}{2}", strVal1, strVal2,type);
        }

        private static string setValueAt(string inputfield, int position, VerifyStatus status)
        {
            if (position > inputfield.Length)
                return inputfield;

            if ((STATUS_FIELD_LEN - position) < 0 ||
                ((STATUS_FIELD_LEN - position) > STATUS_FIELD_LEN))
                return inputfield;

            try
            {
                string strVal = ((int)status).ToString();
                char[] chars = inputfield.ToCharArray();
                chars[STATUS_FIELD_LEN-position] = strVal.ToCharArray()[0];
                return new string(chars);
            }
            catch { } // non loggo :(
            return null;
        }

        public void setGlobalStatus()
        {
            if(
                this.NoMacroOrExe    ==  VerifyStatus.Invalid||
                this.Signature       ==  VerifyStatus.Invalid||
                this.Signature       == VerifyStatus.Expired ||
                this.CrlStatus       ==  VerifyStatus.Invalid||
                this.CrlStatus       ==  VerifyStatus.Expired||
                this.TimeStampStatus ==  VerifyStatus.Invalid||
                this.FileFormatOK    ==  VerifyStatus.Invalid ||
                this.FileHashOK      ==  VerifyStatus.Invalid
                )
                this.Status = VerifyStatus.Invalid;
            else
                this.Status = VerifyStatus.Valid;

            //per la firma 
            if (this.Signature == VerifyStatus.InProgress ||
                this.CrlStatus == VerifyStatus.InProgress)
                this.Status = VerifyStatus.InProgress;

        }
    }


    

}
