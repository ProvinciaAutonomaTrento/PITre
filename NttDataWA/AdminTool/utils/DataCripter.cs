using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;


namespace SAAdminTool.utils
{
	/// <summary>	
	/// Di solito per crittografare si usa la chiave ufficiale e il risultato della codifica dei byte precedenti.
	//  Quando si inizia da una stringa è chiaro che prima dei primi 8 byte non c'è niente. 
	/// Quindi definiamo un vettore di inizalizzazione che serve proprio a questo, cioè a fornire una base di partenza all'algoritmo.
	/// Chiave e vettore di inizalizzazione hanno la stessa dimensione 16 byte.
	/// 
	/// Alcune proprietà e metodi (come IV, Key, ecc.) della classe Rijndael vengono ereditati dalla classe Madre SymmetricAlgorithm così come altre classi di algortimi appartenenti agli algoritmi simmetrici.
	/// Dentro la classe madre ci sono anche due metodi GenerateIV e GenerateKey per creare automaticamente ed in modo casuale le due chiavi enunciate poco fa.
	/// </summary>
	public class DataCripter
	{
		public DataCripter()
		{			
		}

		private const string chiave= "AxTYQWCvGTFRbgLL"; //16 byte - chiave ufficiale
		private const string iv = "QWExcfTyUxxLOafO"; //16 byte - Initialization Vector

		public string Encode(string S)
		{
			RijndaelManaged rjm = new RijndaelManaged();
			rjm.KeySize = 128;
			rjm.BlockSize = 128;
			rjm.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
			rjm.IV = ASCIIEncoding.ASCII.GetBytes(iv);
			Byte[] input = Encoding.UTF8.GetBytes(S);
			Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0,	input.Length);
			return Convert.ToBase64String(output);
		}

		public string Decode(string S)
		{
			RijndaelManaged rjm = new RijndaelManaged();
			rjm.KeySize = 128;
			rjm.BlockSize = 128;
			rjm.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
			rjm.IV = ASCIIEncoding.ASCII.GetBytes(iv);
			try
			{
				Byte[] input = Convert.FromBase64String(S);
				Byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0,	input.Length);
				return Encoding.UTF8.GetString(output);
			}
			catch 
			{
				return S;
			}
		}
	}
}
