using System;
using System.Security.Cryptography;
using System.Text;

namespace DocsPaUtils.Security
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class CryptographyManager
	{
		/// <summary>
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static string CalcolaImpronta256(byte[] stream) 
		{
            SHA256Managed sha = new SHA256Managed();
			byte[] impronta = sha.ComputeHash(stream);
			return BitConverter.ToString(impronta).Replace("-", "");
		}

        public static string CalcolaImpronta(byte[] stream)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] impronta = sha.ComputeHash(stream);
            //Console.WriteLine(impronta.Length); //Commentato per evitare la stampa.

            return BitConverter.ToString(impronta).Replace("-", "");
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="clearPassword"></param>
        ///// <returns></returns>
        //public static string HashPasswordWithSalt(string clearPassword)
        //{
        //    byte[] clearPwdBytes = Encoding.Unicode.GetBytes(clearPassword);

        //    string salt = CreateSalt(clearPwdBytes.Length);

        //    return string.Concat(CalcolaImpronta(clearPwdBytes), salt);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="size"></param>
        ///// <returns></returns>
        //private static string CreateSalt(int size)
        //{
        //    // Generate a cryptographic random number using the cryptographic service provider
        //    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        //    byte[] buff = new byte[size];
        //    rng.GetBytes(buff);

        //    // Return a Base64 string representation of the random number
        //    return Convert.ToBase64String(buff);
        //}
	}
}
