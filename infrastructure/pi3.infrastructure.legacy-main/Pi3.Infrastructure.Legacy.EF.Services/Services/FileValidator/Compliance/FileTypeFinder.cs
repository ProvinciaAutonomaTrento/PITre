// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Pi3.Infrastructure.Legacy.EF.Services.FileValidator.Compliance
{
    internal delegate string typeHandlerDelegate(byte[] content, MagicNumers Magic);

    internal class BinMagicHeader
    {
        public byte[] Header;
        public byte[] Mask;
        public int HeaderOffset;
    }

    internal class MagicNumers
    {
        public bool Ascii;
        public string Type;
        public BinMagicHeader header;
        public typeHandlerDelegate typeHandler;
    }


    internal class FileTypeFinder
    {

        List<MagicNumers> magics = null;


        static string xmlHandler(byte[] content, MagicNumers Magic)
        {
            if (System.Text.ASCIIEncoding.ASCII.GetString(content).ToLower().Contains("<svg"))
                return "SVG";
            else return "XML";
        }

        static string pdfHandler(byte[] content, MagicNumers Magic)
        {
            bool isPdf = false;

            var pdfString = "%PDF-";
            var pdfBytes = Encoding.ASCII.GetBytes(pdfString);
            var len = pdfBytes.Length;
            var buf = new byte[len];
            var remaining = len;
            var pos = 0;
            using (var f = new MemoryStream(content))
            {
                while (remaining > 0)
                {
                    var amtRead = f.Read(buf, pos, remaining);
                    if (amtRead == 0) isPdf = false;
                    remaining -= amtRead;
                    pos += amtRead;
                }
            }
            isPdf = pdfBytes.SequenceEqual(buf);

            if (isPdf)
                return Magic.Type;
            else
                return null;
        }

        string dxfHander(byte[] content, MagicNumers Magic)
        {
           byte [] appo= (byte[])content.Clone();
           Array.Reverse ( appo);
           bool cont = false;
            cont =System.Text.ASCIIEncoding.ASCII.GetString(appo).StartsWith("\n\rFOE");
           if (cont)
             return Magic.Type;
            return null;
        }


        static string CompoundDocumentHandler(byte[] content, MagicNumers Magic)
        {
            string retval = CompoundDocumentFinder (content,Magic);

            if (CompoundDocumentHasMacros(content))
                retval += "+Macro";
          
            return retval;
        }


        static bool CompoundDocumentHasMacros(byte[] content)
        {
            MemoryStream ms = new MemoryStream(content);
            ms.Position = 0;
            CompoundDocument doc = CompoundDocument.Open(ms);

            DirectoryEntry di = new DirectoryEntry();
            di = doc.FindDirectoryEntry(doc.RootStorage, "Macros");
            if (di != null)
            {
                di = doc.FindDirectoryEntry(di, "VBA");
                if (di != null)
                    return true;
            }

            di = doc.FindDirectoryEntry(doc.RootStorage, "_VBA_PROJECT_CUR");
            if (di != null)
            {
                di = doc.FindDirectoryEntry(di, "VBA");
                if (di != null)
                    return true;
            }

            di = doc.FindDirectoryEntry(doc.RootStorage, "VBA");
            if (di != null)
                return true;

            return false;
        }

        static string CompoundDocumentFinder(byte[] content, MagicNumers Magic)
        {

            
            MemoryStream ms = new MemoryStream(content);
            ms.Position = 0;
            CompoundDocument doc = CompoundDocument.Open(ms);
            
            DirectoryEntry di = new DirectoryEntry();
            di= doc.FindDirectoryEntry(doc.RootStorage, "Workbook");
            if (di != null)
                return "XLS";

            di = doc.FindDirectoryEntry(doc.RootStorage, "PowerPoint Document");
            if (di != null)
                return "PPT";

            di = doc.FindDirectoryEntry(doc.RootStorage, "WordDocument");
            if (di != null)
                return "DOC";

            di = doc.FindDirectoryEntry(doc.RootStorage, "__nameid_version1.0");
            if (di != null)
                return "MSG";

            di = doc.FindDirectoryEntry(doc.RootStorage, "VisioDocument");
            if (di != null)
                return "VSD";

            di = doc.FindDirectoryEntry(doc.RootStorage, "CompObj");
            if (di != null)
            {
                if (System.Text.ASCIIEncoding.ASCII.GetString(di.Data ).Contains("Microsoft Project"))
                 return "MPP";
            }
           

            return "DOCFILE";
            

        
        }


        bool markupFinder(string text)
        {
        
            

            Regex r = new Regex ("/?\\w+((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?>");
            Match m = r.Match(text);
            
            return m.Success ;
        }
        string textHandler(byte[] content, MagicNumers Magic)
        {
            string txtFile = System.Text.ASCIIEncoding.ASCII.GetString(content);
            if (txtFile.Contains("Date") && txtFile.Contains("From") && txtFile.Contains("Received"))
            {
                return "EML";
            }

            if (markupFinder(txtFile))
            {
                if (txtFile.ToLower().Contains("<html"))
                    return "HTM";

                return "MARKUP";
            }
            

            return null;

        }


        string txtOrBinHandler(byte[] content, MagicNumers Magic)
        {
            string retval=null;
            if (System.Text.ASCIIEncoding.ASCII.GetString(content).Contains("\0\0\0\0"))
                retval= "BIN";
            else
            {
                if (content != null && content.Length < 100000)
                {
                    retval = textHandler(content, null);
                    if (retval != null)
                        return retval;
                }

                retval= "TXT";
            }
            return retval;
        }


        string getFileFromZip(byte[] content, string filename)
        {
            try
            {

                using var stream = new MemoryStream(content);
                using var archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Read);

                var entry = archive.Entries
                        .Where(e => e.Name.Equals(filename, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

                if (entry != null)
                {
                    using var extractedEntry = entry.Open();
                    using var streamReader = new StreamReader(extractedEntry);
                    return streamReader.ReadToEnd();
                }
                else
                    return null!;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        string OOXMLFinder(byte[] content, MagicNumers Magic)
        {
            
            try
            {
                //MS Office
                string file = getFileFromZip(content,"[Content_Types].xml");
                if (file != null)
                {
                    if (file.Contains(@"application/vnd.ms-office.vbaProject"))
                    {
                        if (file.Contains(@"<Override PartName=""/word/"))
                            return "DOCM";
                        if (file.Contains(@"<Override PartName=""/xl/"))
                            return "XLSM";
                        if (file.Contains(@"<Override PartName=""/ppt/"))
                            return "PPTM";
                    }
                    else
                    {
                        if (file.Contains(@"<Override PartName=""/word/"))
                            return "DOCX";
                        if (file.Contains(@"<Override PartName=""/xl/"))
                            return "XLSX";
                        if (file.Contains(@"<Override PartName=""/ppt/"))
                            return "PPTX";
                    }
                }
                //OpenOffice
                file = getFileFromZip(content, "mimetype");
                if (file != null)
                {
                    string macro = "";  
                    string manifestFile = getFileFromZip(content, "META-INF/manifest.xml");
                    if (manifestFile != null)
                    {
                        if (manifestFile.Contains ("application/binary") ||
                            manifestFile.Contains ("Basic/script-lc.xml"))
                            macro = "+Macro";
                      
                    }
                    if (file.Contains("vnd.oasis.opendocument.text"))
                        return "ODT"+macro;
                    if (file.Contains("vnd.oasis.opendocument.spreadsheet"))
                        return "ODS"+macro;
                    if (file.Contains("vnd.oasis.opendocument.presentation"))
                        return "ODP"+macro;

                    if (file.Contains("vnd.oasis.opendocument.graphics"))
                        return "ODG"+macro;

                  
                }

            }
            catch
            {

            }
            return "OOXML";
        }

        private void   initalizeMagics ()
        {
            magics = new List<MagicNumers>();

            //Tiff
            //GIF
            //JPG
            magics.Add(new MagicNumers() { Type = "JPG", header = new BinMagicHeader { Header = new Byte[] { 0xFF, 0xD8, 0xFF }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "GIF", header = new BinMagicHeader { Header = new Byte[] { 0x47, 0x49, 0x46, 0x38 }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "TIFF", header = new BinMagicHeader { Header = new Byte[] { 0x49, 0x49 ,0x2A ,0x00 }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "MP3", header = new BinMagicHeader { Header = new Byte[] { 0x49 ,0x44 ,0x33 }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "PNG", header = new BinMagicHeader { Header = new Byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "BMP", header = new BinMagicHeader { Header = new Byte[] { 0x42, 0x4D }, HeaderOffset = 0 } });
            
            magics.Add(new MagicNumers() { Type = "ONE", header = new BinMagicHeader { Header = new Byte[] { 0xe4, 0x52, 0x5c, 0x7b, 0x8c, 0xd8, 0xa7, 0x4d, 0xae, 0xb1, 0x53, 0x78, 0xd0, 0x29, 0x96, 0xd3 }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "RTF", header = new BinMagicHeader { Header = new Byte[] { 0x7b, 0x5c, 0x72, 0x74, 0x66, 0x31, 0x5c }, HeaderOffset = 0 } });

            //Vettoriali
            magics.Add(new MagicNumers() { Type = "DWG", header = new BinMagicHeader { Header = new Byte[] { 0x41, 0x43, 0x31, 0x30 }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "DWF", header = new BinMagicHeader { Header = new Byte[] { 0x28, 0x44, 0x57, 0x46, 0x20, 0x56 }, HeaderOffset = 0 }});

            magics.Add(new MagicNumers() { Type = "DXF", header = new BinMagicHeader { Header = new Byte[] { 0x20, 0x20, 0x30, 0x0a, 0x53, 0x45, 0x43, 0x54, 0x49, 0x4f, 0x4e}, HeaderOffset = 0 }, typeHandler = new typeHandlerDelegate(dxfHander), Ascii = true});
            magics.Add(new MagicNumers() { Type = "DXF", header = new BinMagicHeader { Header = new Byte[] { 0x20, 0x20, 0x30, 0x0d,0x0a, 0x53, 0x45, 0x43, 0x54, 0x49, 0x4f, 0x4e }, HeaderOffset = 0 }, typeHandler = new typeHandlerDelegate(dxfHander), Ascii = true });

            
            magics.Add(new MagicNumers() { Type = "XML", header = new BinMagicHeader { Header = new Byte[] { 0x3c, 0x3f, 0x78, 0x6d, 0x6c }, HeaderOffset = 0 }, typeHandler = new typeHandlerDelegate(xmlHandler) });

            //Acrobat
            magics.Add(new MagicNumers() { Type = "PDF", header = new BinMagicHeader { Header = new Byte[] { 0x25, 0x50, 0x44, 0x46 }, HeaderOffset = 0 }, typeHandler = new typeHandlerDelegate(pdfHandler) });
            
            //Office vecchio
            magics.Add(new MagicNumers() { Type = "CompoundDocument", header = new BinMagicHeader { Header = new Byte[] { 0xD0, 0xCF, 0x11, 0xE0 }, HeaderOffset = 0 }, typeHandler = new typeHandlerDelegate(CompoundDocumentHandler) });
           
            //office Nuovo
            magics.Add(new MagicNumers() { Type = "OOXML", header = new BinMagicHeader { Header = new Byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00 }, HeaderOffset = 0 }, typeHandler = new typeHandlerDelegate(OOXMLFinder) });
            magics.Add(new MagicNumers() { Type = "EXE", header = new BinMagicHeader { Header = new Byte[] { 0x4D ,0x5A }, HeaderOffset = 0 } });
           
            magics.Add(new MagicNumers() { Type = "ZIP", header = new BinMagicHeader { Header = new Byte[] { 0x50, 0x4B }, HeaderOffset = 0 } });
            
            //Con il TAR VERO funziona.
            magics.Add(new MagicNumers() { Type = "TAR", header = new BinMagicHeader { Header = new Byte[] { 0x75, 0x73, 0x74, 0x61, 0x72 }, HeaderOffset = 257 } });

            magics.Add(new MagicNumers() { Type = "7Z", header = new BinMagicHeader { Header = new Byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "RAR", header = new BinMagicHeader { Header = new Byte[] { 0x52, 0x61, 0x72,0x21, 0x1A, 0x07, 0x00 }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "ARJ", header = new BinMagicHeader { Header = new Byte[] { 0x60, 0xea }, HeaderOffset = 0 } });
            magics.Add(new MagicNumers() { Type = "ISO", header = new BinMagicHeader { Header = new Byte[] { 0x43, 0x44, 0x30, 0x30, 0x31 }, HeaderOffset = 32769 } });
            
        }
    
        bool compareByte(byte[] inputByte, byte[] searchPattern,byte[] mask, int offset)
        {
            int i;
            int a = 0;
            int len= searchPattern.Length;

            if (inputByte.Length <= offset)
                return false;
            if (inputByte.Length <= searchPattern.Length)
                return false;

            for (i = offset; i < (offset + len); i++)
            {
                if (mask != null)
                {
                    if (inputByte[i] != (searchPattern[a] & mask[a]))
                        return false;
                    a++;
                }
                else
                {
                    if (inputByte[i] != searchPattern[a++])
                        return false;
                }
               
            }
            return true;
        }

        public string FileType(byte[] content)
        {
            if (magics == null)
                initalizeMagics();

            bool result=false;
            string retval = null;
            try
            {
                foreach (MagicNumers m in magics)
                {

                    if (m.header != null)
                    {
                        result = compareByte(content, m.header.Header, m.header.Mask, m.header.HeaderOffset);
                        if (result)
                            retval = m.Type;
                    }
                    if ((retval != null) && (m.typeHandler != null))
                    {
                        if (retval == m.Type)
                        {
                            string st = m.typeHandler(content, m);
                            if (st != null)
                                return st;
                        }
                    }

                }

                if (retval == null)
                    retval = txtOrBinHandler(content, null);

            }
            catch
            {

                //Errore.. da qualche parte ritorno null
            }
            return retval;
        }

        private  byte[] openRoFile (string path)
        {
            return System.IO.File.ReadAllBytes (path);
        }


        //EntryPoint!
        public string FileType(Stream file)
        {
            file.Position=0;

            BinaryReader   br = new BinaryReader (file);
            byte[] buffer = new byte[file.Length];
            buffer=  br.ReadBytes (buffer.Length );
            return FileType(buffer);
        }

        public string FileType(string path)
        {
                byte[] fileData = openRoFile(path);
                return FileType(fileData);
        }

        public void addMagicType(MagicNumers magic)
        {
            if (magics != null)
                magics.Add(magic);
        }

        public void removeMagicType(string Extension)
        {
            int i;
            for (i = 0; i < magics.Count; i++)
            {
                if (magics[i].Type == Extension)
                {
                    magics.RemoveAt(i);
                    break;
                }
            }
        }
        public void clearAllMagics()
        {
            magics.Clear();
        }
        public FileTypeFinder()
        {
            if (magics == null)
                initalizeMagics();
        }

        public bool isExecutable(string extension)
        {
            if(
                    extension.ToLower().Contains("+macro") ||
                    extension.ToLower().Contains("+javascript") ||
                    extension.ToLower().Contains("+forms") ||
                    extension.ToLower().Contains("docm") ||
                    extension.ToLower().Contains("xlsm") ||
                    extension.ToLower().Contains("pptm")
                ) return true;
            if(extension.Contains("exe")) return true;
            if (extension.Contains("dll")) return true;
            if (extension.Contains("sys")) return true;
            return false;
        }

        public bool isPdfSiged(string extension)
        {
            if (extension.ToLower().Contains("pades"))
                return true;
            return false;
        }
    }
}
 