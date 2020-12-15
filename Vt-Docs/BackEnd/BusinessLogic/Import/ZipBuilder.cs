using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace BusinessLogic.Import
{
    public class ZipBuilder
    {
        private ZipOutputStream _zipOutStream;
        private string _outPath;
        private bool _invalid;

        public ZipBuilder()
        {
            _outPath = Path.GetTempPath() + "/" + Guid.NewGuid() + ".zip";
            _zipOutStream = new ZipOutputStream(File.Create(_outPath));
            _zipOutStream.SetLevel(1);
            _invalid = false;
        }

        public void AddEntry(string filename,string[] path,byte[] content)
        {
            CheckInvalid();
            string filepath = "";
            foreach (string temp in path)
            {
                filepath += temp + "\\";
            }
            filepath += filename;
            ZipEntry entry = new ZipEntry(filepath);
            entry.Size = content.Length;
            _zipOutStream.PutNextEntry(entry);
            _zipOutStream.Write(content, 0, content.Length);
            _zipOutStream.CloseEntry();
        }

        public byte[] GetOutput()
        {
            CheckInvalid();
            _zipOutStream.Finish();
            _zipOutStream.Close();
            _zipOutStream.Dispose();
            BinaryReader reader = new BinaryReader(File.Open(_outPath, FileMode.Open));
            byte[] arr = new byte[reader.BaseStream.Length];
            reader.Read(arr, 0, arr.Length);
            reader.Close();
            File.Delete(_outPath);
            _invalid = true;
            return arr;
        }

        private void CheckInvalid()
        {
            if (_invalid) throw new Exception("Builder invalidato");
        }
    }
}
