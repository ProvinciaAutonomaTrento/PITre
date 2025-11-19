// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Security.Cryptography;
using System.Text;

namespace Pi3.App.Legacy.Mobile.Shared.Helpers;
public class AuthenticationHelper
{
    public static string CalcolaImpronta(string clearText  )
    {
        byte[] stream = System.Text.Encoding.Unicode.GetBytes(clearText);
        byte[] impronta = SHA1.HashData(stream);
        return BitConverter.ToString(impronta).Replace("-", "");
    }

    public static string GeneraToken( 
        long idRuolo, 
        long idPeople, 
        long idGruppo, 
        string dst, 
        long idAmministrazione, 
        string userId,
        string sede,
        string urlWA)
    {
        string clearToken = $"{idRuolo}|{idPeople}|{idGruppo}|{dst}|{idAmministrazione}|{userId}|{sede}|{urlWA}";
        return $"SSO={EncryptToken(clearToken)}";
    }


    public static string EncryptToken( string token )
    {
        byte[] keyArray;
        byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(token);

        // La chiave deve essere di 24 caratteri
        string key = "ValueTeamDocsPa3Services";

        keyArray = UTF8Encoding.UTF8.GetBytes(key);

        using var tripleDes = Aes.Create();

        tripleDes.Key = keyArray;
        tripleDes.Mode = CipherMode.ECB; // ECB è generalmente meno sicuro di altre modalità
        tripleDes.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = tripleDes.CreateEncryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tripleDes.Clear();

        return Convert.ToBase64String(resultArray);
    }

    public static string DecryptToken( string token )
    {
        byte[] keyArray;
        byte[] toEncryptArray = Convert.FromBase64String(token);

        //La chiave deve essere di 24 caratteri
        string key = "ValueTeamDocsPa3Services";

        keyArray = UTF8Encoding.UTF8.GetBytes(key);

        using var tripleDes = Aes.Create();

        tripleDes.Key = keyArray;
        tripleDes.Mode = CipherMode.ECB; // ECB è generalmente meno sicuro di altre modalità
        tripleDes.Padding = PaddingMode.PKCS7;
        ICryptoTransform cTransform = tripleDes.CreateDecryptor();
        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        tripleDes.Clear();

        return UTF8Encoding.UTF8.GetString(resultArray);
    }

}
