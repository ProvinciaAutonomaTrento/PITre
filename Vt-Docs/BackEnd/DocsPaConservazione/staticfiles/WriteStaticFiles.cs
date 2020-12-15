using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace DocsPaConservazione.staticfiles
{
	public class WriteStaticFiles
	{
        public WriteStaticFiles(string path)
        {
            Assembly _assembly = Assembly.GetExecutingAssembly();
            BinaryReader _reader;

            string[] fileList = _assembly.GetManifestResourceNames();
            Type type = this.GetType();
            string nameSpace = type.Namespace + ".";
            foreach (string file in fileList)
            {
                if (file.StartsWith(nameSpace))
                {
                    string filename = file.Replace(nameSpace, string.Empty);
                    _reader = new BinaryReader(_assembly.GetManifestResourceStream(file));
                    byte [] filecontent = _reader.ReadBytes((int)_reader.BaseStream.Length);

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    System.IO.File.WriteAllBytes  (String.Format("{0}\\{1}", path, filename), filecontent);
                }
            }
        }
	}
}
