using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Subscriber
{
    /// <summary>
    /// Classe di utility per la serializzazione di oggetti
    /// </summary>
    public sealed class ObjectSerializerHelper
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(ObjectSerializerHelper));

        /// <summary>
        /// 
        /// </summary>
        private ObjectSerializerHelper()
        { }

        /// <summary>
        /// Serializzazione di un oggetto in codifica Base64
        /// </summary>
        /// <param name="obj">
        /// Oggetto da serializzare
        /// </param>
        /// <returns></returns>
        public static string Serialize(object data)
        {
            _logger.Info("BEGIN");

            try
            {
                byte[] b = null;

                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, data);

                    stream.Position = 0;

                    b = new byte[stream.Length];
                    stream.Read(b, 0, b.Length);
                }

                return Convert.ToBase64String(b);
            }
            catch (SubscriberException pubEx)
            {
                _logger.Error(pubEx.Message);

                throw pubEx;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, 
                                    string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message),
                                    ex);
            }
            finally
            {
                _logger.Info("END");
            }
        }

        /// <summary>
        /// Deserializzazione di un oggetto dal valore codificato in Base64
        /// </summary>
        /// <param name="dataAsBase64">
        /// Oggetto in codifica Base64
        /// </param>
        /// <returns></returns>
        public static object Deserialize(string dataAsBase64)
        {
            _logger.Info("BEGIN");

            try
            {
                object obj = null;

                byte[] b = Convert.FromBase64String(dataAsBase64);

                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(b, 0, b.Length);

                    stream.Position = 0;

                    BinaryFormatter formatter = new BinaryFormatter();
                    obj = formatter.Deserialize(stream);
                }

                return obj;
            }
            catch (SubscriberException pubEx)
            {
                _logger.Error(pubEx.Message);

                throw pubEx;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                throw new SubscriberException(ErrorCodes.UNHANDLED_ERROR, 
                            string.Format(ErrorDescriptions.UNHANDLED_ERROR, ex.Message),
                            ex);
            }
            finally
            {
                _logger.Info("END");
            }
        }
    }
}
