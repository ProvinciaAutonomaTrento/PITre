using System;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.OpenFile
{
    public class FileInfo
    {
        [JsonProperty("Content")]
        public string content { get; set; }

        [JsonProperty("ContentType")]
        public string contentType { get; set; }

        [JsonProperty("EstensioneFile")]
        public string estensioneFile { get; set; }

        [JsonProperty("FullName")]
        public string fullName { get; set; }

        [JsonProperty("Length")]
        public long length { get; set; }

        [JsonProperty("Name")]
        public string name { get; set; }

        [JsonProperty("OriginalFileName")]
        public string originalFileName { get; set; }

        [JsonProperty("Path")]
        public string path { get; set; }

        public FileInfo()
        {
        }

        public FileInfo(string content, string contentType, string estensioneFile, string fullName, long length, string name, string originalFileName, string path)
        {
            this.content = content;
            this.contentType = contentType;
            this.estensioneFile = estensioneFile;
            this.fullName = fullName;
            this.length = length;
            this.name = name;
            this.originalFileName = originalFileName;
            this.path = path;
        }
    }
}
