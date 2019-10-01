using System;
using System.IO;

namespace StampaRegistri.Utils
{
	public class FSO
	{
		public static string VerificaECreaFolder(string pathFolder)
		{
			string text = pathFolder.EndsWith("\\") ? pathFolder.Substring(0, pathFolder.Length - 1) : pathFolder;
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}

		public static bool EsisteFolder(string pathFolder)
		{
			string path = pathFolder.EndsWith("\\") ? pathFolder.Substring(0, pathFolder.Length - 1) : pathFolder;
			return Directory.Exists(path);
		}

		public static string GetPathCompletoFile(string pathfolder, string fileName)
		{
			if (pathfolder == null || fileName == null || pathfolder.Equals(string.Empty) || fileName.Equals(string.Empty))
			{
				return null;
			}
			return string.Format("{0}{2}{1}", pathfolder, fileName, pathfolder.EndsWith("\\") ? "" : "\\");
		}

		public static bool EsisteFile(string pathfolder, string fileName)
		{
			return File.Exists(FSO.GetPathCompletoFile(pathfolder, fileName));
		}

		public static bool EsisteFile(string pathCompletoFile)
		{
			return File.Exists(pathCompletoFile);
		}

		public static string GetTimeFileName(string filePrefix, string estensione)
		{
			return string.Format("{0}_{1}{2}{3}", new object[]
			{
				filePrefix,
				DateTime.Now.ToString("yyyyMMdd_hhmmss"),
				".",
				estensione
			});
		}
	}
}
