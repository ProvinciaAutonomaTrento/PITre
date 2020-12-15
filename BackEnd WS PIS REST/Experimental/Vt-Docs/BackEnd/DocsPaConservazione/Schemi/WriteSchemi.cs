using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace DocsPaConservazione.Schemi
{
    public class WriteSchemi
    {

        public WriteSchemi(string path)
        {
            Assembly _assembly = Assembly.GetExecutingAssembly() ;
            StreamReader _textStreamReader;

            string [] fileList = _assembly.GetManifestResourceNames();
            Type type = this.GetType();
            string nameSpace = type.Namespace+ ".";
            foreach (string file in fileList)
            {
                if (file.StartsWith(nameSpace))
                {

                    string filename = file.Replace(nameSpace, string.Empty);
                    _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream(file));
                    string filecontent = _textStreamReader.ReadToEnd();

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    System.IO.File.WriteAllText(String.Format("{0}\\{1}", path, filename), filecontent);
                }
            }
           
           

        }
        
    }
}
