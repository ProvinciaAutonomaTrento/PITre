////////////////////////////////////////////////////////////////////////////
/*
Copyright (C) 1997-2010 Adlib Software
All rights reserved.

DISCLAIMER OF WARRANTIES:
 
Permission is granted to copy this Sample Code for internal use only, 
provided that this permission notice and warranty disclaimer appears in all copies.
 
SAMPLE CODE IS LICENSED TO YOU AS-IS.
 
ADLIB SOFTWARE AND ITS SUPPLIERS AND LICENSORS DISCLAIM ALL WARRANTIES, 
EITHER EXPRESS OR IMPLIED, IN SUCH SAMPLE CODE, INCLUDING THE WARRANTY OF 
NON-INFRINGEMENT AND THE IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS FOR A 
PARTICULAR PURPOSE. IN NO EVENT WILL ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS 
BE LIABLE FOR ANY DAMAGES ARISING OUT OF THE USE OF OR INABILITY TO USE THE SAMPLE 
APPLICATION OR SAMPLE CODE, DISTRIBUTION OF THE SAMPLE APPLICATION OR SAMPLE CODE, 
OR COMBINATION OF THE SAMPLE APPLICATION OR SAMPLE CODE WITH ANY OTHER CODE. 
IN NO EVENT SHALL ADLIB SOFTWARE OR ITS LICENSORS AND SUPPLIERS BE LIABLE FOR ANY 
LOST REVENUE, LOST PROFITS OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, 
INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, 
EVEN IF ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
*/
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Net;
using System.Reflection;

namespace Adlib.Director.DirectorWSAWrapper
{
    public class Utilities
    {
        /// <summary>
        /// GetFilesList
        /// </summary>
        /// <param name="files">ArrayList of files passed by reference</param>
        /// <param name="folder">The folder to search in</param>
        /// <param name="fileExtension">File extension to search for (format ".xml")</param>
        /// <param name="recursive">Recurse through all sub-folders</param>
        public static void GetFilesList(ref List<string> files, string folder, string fileExtension, bool recursive)
        {
            DirectoryInfo di = new DirectoryInfo(folder);

            if (!string.IsNullOrEmpty( fileExtension))
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    if (fi.Extension == fileExtension)
                        files.Add(fi.FullName);
                }
            }
            else
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    files.Add(fi.FullName);
                }
            }

            if (recursive)
            {
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    GetFilesList(ref files, dir.FullName, fileExtension, recursive);
                }
            }
        }
        public static void DownloadToLocalPath(string httpPath, string targetLocalPath)
        {
            Uri xHTTPPath = new Uri(httpPath);
            if (xHTTPPath.IsAbsoluteUri)
            {

                WebRequest xRequest = WebRequest.Create(xHTTPPath);
                xRequest.Method = "GET";
                xRequest.PreAuthenticate = true;
                xRequest.Credentials = CredentialCache.DefaultCredentials;
                using (WebResponse xResponse = xRequest.GetResponse())
                {
                    Stream xResponseStream = xResponse.GetResponseStream();

                    BinaryReader br = new BinaryReader(xResponseStream);

                    FileStream fsTemp = File.Create(targetLocalPath);

                    BinaryWriter bw = new BinaryWriter(fsTemp);

                    BinaryWrite(bw, br, xResponse.ContentLength, true);

                }
            }

        }
        public static string AssemblyVersion
        {
            get
            {
                return "V" + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString()
                    + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString()
                    + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();

            }
        }
        public static string AssemblyVersionFormatted
        {
            get
            {
                return string.Format("Version {0}.{1}.{2}", Assembly.GetExecutingAssembly().GetName().Version.Major.ToString(),
                 Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString(),
                    Assembly.GetExecutingAssembly().GetName().Version.Build.ToString());

            }
        }
        public static bool ValidateXml(string xml, ref string errorMsg)
        {
            bool result = false;
            if (string.IsNullOrEmpty(xml))
            {
                errorMsg = "Data is null or empty.";
                return result;
            }
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.XmlResolver = null; 
                xmlDoc.LoadXml(xml);
                result = true;
                errorMsg = null;
            }
            catch (Exception ex)
            {
                result = false;
                errorMsg = ex.Message;
            }
            return result;
        }
        private static void BinaryWrite(BinaryWriter writer, BinaryReader reader, long readerLength, bool closeWhenDone)
        {
            try
            {
                if (reader.BaseStream.CanSeek)
                    reader.BaseStream.Position = 0;
                int bufferLength = 4096;
                long remaining = readerLength;
                bool done = false;
                while (!done)
                {
                    if (remaining <= bufferLength)
                    {
                        // this happens on the last iteration
                        bufferLength = Convert.ToInt32(remaining);
                        done = true;
                    }
                    byte[] buffer = reader.ReadBytes(bufferLength);
                    writer.Write(buffer, 0, bufferLength);
                    remaining -= bufferLength;
                }

            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                if (closeWhenDone)
                {
                    writer.Close();
                    reader.Close();
                }
            }
        }
    }
    public class Serialize
    {
        /// <summary>
        /// To convert a Byte Array of UTF8 values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">UTF8 Byte Array to be converted to String</param>
        /// <returns>String converted from UTF8 Byte Array</returns>
        private static string UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        private static Byte[] StringToUTF8ByteArray(string xmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(xmlString);
            return byteArray;
        }

        /// <summary>
        /// Method to convert a custom Object to XML string
        /// </summary>
        /// <param name="pObject">Object that is to be serialized to XML</param>
        /// <returns>XML string</returns>
        public static string SerializeObject(Object value, System.Type type)
        {
            try
            {
                string xmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

                xmlSerializer.Serialize(memoryStream, value);
                //memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                xmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                memoryStream.Close();
                return xmlizedString;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Method to reconstruct an Object from XML string
        /// </summary>
        /// <param name="pXmlizedString"></param>
        /// <returns></returns>
        public static Object DeserializeObject(string xmlString, System.Type type)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(xmlString));
            //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            Object returnObj = xmlSerializer.Deserialize(memoryStream);
            memoryStream.Close();

            return returnObj;
        }
    }
}
