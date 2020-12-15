using System;
using System.Reflection;
using System.Resources;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Resources
{
    public class StringManager
    {

        private static StringManager instance;

        //Sostituire con il valore del tag AssemblyTitle nel file "AssemblyInfo.cs" in genere corrisponde al nome del progetto
        private static readonly string AssemblyName = "InformaticaTrentinaPCL";

        //Sostituire con il namespace corretto in cui è stato generato il file Strings.resx
        // Ad esempio in questo caso il file è stato generato nella cartella (NomeProgetto)/Resources/Strings.resx
        private static readonly string NamespaceResx = "InformaticaTrentinaPCL.Resources.CoreStrings";

        public static StringManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StringManager();
                }
                return instance;
            }
        }

        ResourceManager resourceManager;
        
        private StringManager()
        {
            var ass = new AssemblyName(AssemblyName);
            resourceManager = new ResourceManager(NamespaceResx, Assembly.Load(ass));
        }

        public string GetString(LocalizedString stringEnum)
        {
            return resourceManager.GetString(stringEnum.ToString()).Replace("\\n", Environment.NewLine);
        }

        public string GetString(LocalizedString stringEnum, string culture)
        {
            return resourceManager.GetString(stringEnum.ToString(), new System.Globalization.CultureInfo(culture)).Replace("\\n", Environment.NewLine);
        }
    }
}
