using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Rubrica.Library.Utils
{
    /// <summary>
    /// Classe per la validazione dei dati
    /// </summary>
    public sealed class Validator
    {
        /// <summary>
        /// Validazione di un indirizzo mail
        /// </summary>
        /// <param name="mail"></param>
        public static void CheckMailAddress(string mail)
        {
            const string PATTERN = @"^([\w-\.]+)@((\[[0–9]{1,3}\.[0–9]{1,3}\.[0–9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0–9]{1,3})(\]?)$";

            if (!Regex.IsMatch(mail, PATTERN))
                throw new RubricaException(string.Format(Properties.Resources.MailAddressNotValidException, mail)); 
        }

        /// <summary>
        /// Validazione proprietà in base ai parametri forniti definiti 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="required"></param>
        /// <param name="maxLenght"></param>        
        public static void CheckProperty<T>(object obj, string propertyName, bool required, int maxLenght)
        {
            PropertyInfo p = obj.GetType().GetProperty(propertyName);

            if (p != null)
            {
                T value = (T)p.GetValue(obj, null);

                if (required)
                {
                    bool isNull = false;

                    if (typeof(T) == typeof(string))
                        isNull = string.IsNullOrEmpty((string)Convert.ChangeType(value, TypeCode.String));
                    else
                        isNull = value.Equals(default(T));

                    if (isNull)
                        throw new RubricaException(string.Format(Properties.Resources.MissingFieldException, p.Name));
                }

                // Verifica dimensione massima campo
                if (p.PropertyType.Equals(typeof(string)) && maxLenght > 0 &&
                    value != null && value.ToString().Length > maxLenght)
                    throw new RubricaException(string.Format(Properties.Resources.MaxFieldLenghtException, p.Name, maxLenght.ToString()));
            }
        }

        /// <summary>
        /// Metodo per la validazione di un URL
        /// </summary>
        /// <param name="url"></param>
        public static void CheckUrl(string url)
        {
            if(!String.IsNullOrEmpty(url) && !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new RubricaException(String.Format(Properties.Resources.UrlNotValidException, url)); 
        }

         public static void CheckVatNumber(string vatNum, string exception)
        {
            bool result = false;
            const int character = 11;
            string vatNumber = vatNum;
            Regex pregex = new Regex("^\\d{" + character.ToString() + "}$");

            if (string.IsNullOrEmpty(vatNumber) || vatNum.Length != character)
                throw new RubricaException(exception); 
            Match m = pregex.Match(vatNumber);
            result = m.Success;
            if (!result)
                throw new RubricaException(exception);
            result = (int.Parse(vatNumber.Substring(0, 7)) != 0);
            if (!result)
                throw new RubricaException(exception);
            result = ((int.Parse(vatNumber.Substring(7, 3)) >= 0) && (int.Parse(vatNumber.Substring(7, 3)) < 201));
            if (!result)
                throw new RubricaException(exception);

            /*Algoritmo di verifica della correttezza formale del numero di partita IVA 
            ---------------------------------------------------------------------------------------------
                1. si sommano tra loro le cifre di posto dispari
                2. le cifre di posto pari si moltiplicano per 2
                3. se il risultato del punto precedente è maggiore di 9 si sottrae 9 al risultato
                4. si sommano tra loro i risultati dei 2 punti precedenti
                5. si sommano tra loro le due somme ottenute
            ---------------------------------------------------------------------------------------------
             */
            int sum = 0;
            for (int i = 0; i < character - 1; i++)
            {
                int j = int.Parse(vatNumber.Substring(i, 1));
                if ((i + 1) % 2 == 0)
                {
                    j *= 2;
                    char[] c = j.ToString("00").ToCharArray();
                    sum += int.Parse(c[0].ToString());
                    sum += int.Parse(c[1].ToString());
                }
                else
                    sum += j;
            }
            if ((sum.ToString("00").Substring(1, 1).Equals("0")) && (!vatNumber.Substring(10, 1).Equals("0")))
                throw new RubricaException(exception);
            sum = int.Parse(vatNumber.Substring(10, 1)) + int.Parse(sum.ToString("00").Substring(1, 1));
            if (!sum.ToString("00").Substring(1, 1).Equals("0"))
                throw new RubricaException(exception);
        }

        /// <summary>
        /// Controllo formale della correttezza del Codice Fiscale
        /// </summary>
        /// <param name="taxCode"></param>
        /// <returns>
        /// -1 : Lunghezza Codice Fiscale errata.
        /// -2 : Il formato del Codice Fiscale non è corretto.
        /// -3 : Verifica della correttezza formale del Codice Fiscale non superata 
        /// 0 :  Codice Fiscale corretto
        /// </returns>
        public static void CheckTaxCode(string taxCode)
        {
            taxCode = taxCode.Replace(" ", "");
            bool result = false;
            const int character = 16;
            // stringa per controllo e calcolo omocodia 
            const string omocode = "LMNPQRSTUV";
            // per il calcolo del check digit e la conversione in numero
            const string listControl = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int[] listEquivalent = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 };
            int[] listaUnequal = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23 };

            result = (string.IsNullOrEmpty(taxCode) || taxCode.Length != character);
            if (result)
                throw new RubricaException(Properties.Resources.CodiceFiscaleException);
            taxCode = taxCode.ToUpper();
            char[] arrTaxCode = taxCode.ToCharArray();

            // check della correttezza formale del codice fiscale
            // elimino dalla stringa gli eventuali caratteri utilizzati negli 
            // spazi riservati ai 7 che sono diventati carattere in caso di omocodia
            for (int k = 6; k < 15; k++)
            {
                if ((k == 8) || (k == 11))
                    continue;
                int x = (omocode.IndexOf(arrTaxCode[k]));
                if (x != -1)
                    arrTaxCode[k] = x.ToString().ToCharArray()[0];
            }

            Regex rgx = new Regex(@"^[A-Z]{6}[0-9]{2}[A-Z][0-9]{2}[A-Z][0-9]{3}[A-Z]$");
            Match m = rgx.Match(new string(arrTaxCode));
            result = m.Success;
            // normalizzato il codice fiscale se la regular non ha buon
            // fine è inutile continuare
            if (!result)
                throw new RubricaException(Properties.Resources.CodiceFiscaleException);
            int somma = 0;
            // ripristino il codice fiscale originario 
            arrTaxCode = taxCode.ToCharArray();
            for (int i = 0; i < 15; i++)
            {
                char c = arrTaxCode[i];
                int x = "0123456789".IndexOf(c);
                if (x != -1)
                    c = listControl.Substring(x, 1).ToCharArray()[0];
                x = listControl.IndexOf(c);
                // i modulo 2 = 0 è dispari perchè iniziamo da 0
                if ((i % 2) == 0)
                    x = listaUnequal[x];
                else
                    x = listEquivalent[x];
                somma += x;
            }
            result = (listControl.Substring(somma % 26, 1) == taxCode.Substring(15, 1));
            if (!result)
                throw new RubricaException(Properties.Resources.CodiceFiscaleException);
        }

    }
}
