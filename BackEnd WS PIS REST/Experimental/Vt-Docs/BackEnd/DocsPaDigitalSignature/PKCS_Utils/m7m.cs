using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BusinessLogic.Documenti.DigitalSignature.PKCS_Utils
{
    /// <summary>
    /// Classe per gestire i file M7M
    /// </summary>
	public class m7m : PKCS_Utils.ITimeStampedContainer
	{
        private List<CryptoFile> _tsr = new List<CryptoFile>();
        public List<CryptoFile> TSR
        {
            get { return _tsr; }
            set { _tsr = value; }
        }

        private CryptoFile _p7m;
        public CryptoFile Data
        {
            get { return _p7m; }
            set { _p7m = value; }
        }

        private CryptoFile _m7m;
        public CryptoFile cryptoFile
        {
            get { return _m7m; }
            set { _m7m = value; }
        }

        public m7m()
        {
        }
        public m7m(CryptoFile p7mFile, List<CryptoFile> tsrFiles)
        {
            this._p7m = p7mFile;
            this._tsr = tsrFiles;
            create(p7mFile, tsrFiles.ToArray());
        }


        public m7m(CryptoFile M7Mfile)
        {
            explode(M7Mfile.Content);
        }

        public m7m(byte[] m7mFile)
        {
            _m7m = new CryptoFile { Content = m7mFile, MessageFileType = fileType.Unknown, Name = "default.m7m" };
            explode(_m7m.Content);
        }

        public CryptoFile create(CryptoFile p7mFile,CryptoFile[] tsrFiles)
        {
            MemoryStream m7mStream = new MemoryStream();
            string p7mName = p7mFile.Name;
            if (p7mName == null)
            {
                p7mName = "default.p7m";
            } 

            StringBuilder sb = new StringBuilder ();
            sb.AppendLine ("Mime-Version: 1.0");
            sb.AppendLine ("Content-Type: multipart/mixed; boundary=\"DiKeCades\"");
            sb.AppendLine("");
            sb.AppendLine ("--DiKeCades");
            sb.AppendLine(String.Format("Content-Type: application/pkcs7-mime; smime-type=signed-data; name=\"{0}\"", p7mName));
            sb.AppendLine ("Content-Transfer-Encoding: binary");
            sb.AppendLine(String.Format("Content-Disposition: attachment; filename=\"{0}\"", p7mName));
            sb.AppendLine ("Content-Description: Signed envelope");
            sb.AppendLine ("");
            byte[] header = System.Text.ASCIIEncoding.Default.GetBytes(sb.ToString());
            m7mStream.Write(header,0,header.Length);
            m7mStream.Write(p7mFile.Content, 0, p7mFile.Content.Length);
            int counter =1;
            foreach (CryptoFile tsrFile in tsrFiles)
            {
                sb = new StringBuilder();
                sb.AppendLine("");
                sb.AppendLine("--DiKeCades");
                sb.AppendLine(String.Format("Content-Type: application/timestamp-reply; name=\"{0}\"", tsrFile.Name));
                sb.AppendLine("Content-Transfer-Encoding: base64");
                sb.AppendLine(String.Format("Content-Disposition: attachment; filename=\"{0}\"", tsrFile.Name));
                sb.AppendLine("Content-Description: time-stamp response");
                sb.AppendLine("");
                sb.AppendLine(CryptoFile.splitInLines(tsrFile.Base64Content, 76));
                if (counter++ == tsrFiles.Length)
                {
                    sb.AppendLine("");
                    sb.AppendLine("--DiKeCades--");
                }
                header = System.Text.ASCIIEncoding.Default.GetBytes(sb.ToString());
                m7mStream.Write(header, 0, header.Length);
            }
            m7mStream.Position = 0;
            BinaryReader br = new BinaryReader(m7mStream);
            CryptoFile retval = new CryptoFile { Content = br.ReadBytes((int)m7mStream.Length), MessageFileType = fileType.Unknown, Name = p7mName.ToLower().Replace("p7m", "m7m") };
            return retval;

        }

        /// <summary>
        /// Esplode un M7m infocert nelle sue due compomenti (un p7m e un tsr)
        /// </summary>
        /// <param name="fileContents">bytearray contente il messaggio m7m</param>
        public void explode(byte[] fileContents)
        {
            int posi = BusinessLogic.Documenti.DigitalSignature.Helpers.IndexOfInArray(fileContents, System.Text.ASCIIEncoding.ASCII.GetBytes("Mime-Version:"));
            if (posi == 0) //E' un mime m7m
            {
                using (MemoryStream ms = new MemoryStream(fileContents))
                {
                    anmar.SharpMimeTools.SharpMessage sm = new anmar.SharpMimeTools.SharpMessage(ms);
                    if (sm.Attachments.Count > 0)
                    {
                        foreach (anmar.SharpMimeTools.SharpAttachment att in sm.Attachments)
                        {
                            if (System.IO.Path.GetExtension(att.Name).ToLower().Contains("p7m"))
                            {
                                att.Stream.Position = 0;
                                BinaryReader sr = new BinaryReader(att.Stream);
                                _p7m = new CryptoFile { Content = sr.ReadBytes((int)att.Size), MessageFileType = CryptoFile.mapType (att.ContentTransferEncoding), Name = att.Name };
                            }

                            if (System.IO.Path.GetExtension(att.Name).ToLower().Contains("tsr"))
                            {
                                att.Stream.Position = 0;
                                BinaryReader sr = new BinaryReader(att.Stream);
                                _tsr.Add( new CryptoFile { Content = sr.ReadBytes((int)att.Size),MessageFileType = CryptoFile.mapType (att.ContentTransferEncoding), Name = att.Name });
                            }
                        }
                    }
                }
            }
        }


    
	}
}
