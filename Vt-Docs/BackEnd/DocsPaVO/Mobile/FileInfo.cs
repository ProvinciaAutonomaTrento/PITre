using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;

namespace DocsPaVO.Mobile
{
    public class FileInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get; 
            set;
        }

        public string FullName
        {
            get;
            set;
        }

        public string OriginalFileName
        {
            get;
            set;
        }

        public byte[] Content
        {
            get;
            set;
        }

        public int Length
        {
            get; 
            set;
        }

        public string ContentType
        {
            get; 
            set; 
        }

        public string EstensioneFile
        {
            get; 
            set; 
        }

        public static FileInfo buildInstance(FileDocumento input)
        {
            FileInfo res = new FileInfo();
            res.Content = input.content;
            res.ContentType = input.contentType;
            res.EstensioneFile = input.estensioneFile;
            res.FullName = input.fullName;
            res.OriginalFileName = input.nomeOriginale;
            res.Length = input.length;
            res.Name = input.name;
            res.Path = input.path;
            return res;
        }
    }
}
