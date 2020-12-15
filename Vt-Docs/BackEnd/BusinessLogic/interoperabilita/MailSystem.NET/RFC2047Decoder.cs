using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Interoperabilità
{
    public static class RFC2047Decoder
    {
        public static string Parse(string input)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder currentWord = new StringBuilder();
            bool readingWord = false;

            Int32 i = 0;
            while (i < input.Length)
            {
                char currentChar = input[i];
                char peekAhead;
                switch (currentChar)
                {
                    case '=':
                        peekAhead = (i == input.Length - 1) ? ' ' : input[i + 1];

                        if (peekAhead == '?')
                            readingWord = true;
                        break;

                    case '?':
                        peekAhead = (i == input.Length - 1) ? ' ' : input[i + 1];

                        if (peekAhead == '=')
                        {
                            readingWord = false;

                            currentWord.Append(currentChar);
                            currentWord.Append(peekAhead);

                            sb.Append(ParseEncodedWord(currentWord.ToString()));
                            currentWord = new StringBuilder();

                            i += 2;
                            continue;
                        }
                        break;
                }

                if (readingWord)
                {
                    currentWord.Append(currentChar);
                    i++;
                }
                else
                {
                    sb.Append(currentChar);
                    i++;
                }
            }

            return sb.ToString();
        }

        private static string ParseEncodedWord(string input)
        {
            StringBuilder sb = new StringBuilder();

            if (!input.StartsWith("=?"))
                return input;

            if (!input.EndsWith("?="))
                return input;

            // Get the name of the encoding but skip the leading =?
            string encodingName = input.Substring(2, input.IndexOf("?", 2) - 2);
            Encoding enc = Encoding.GetEncoding(encodingName);

            // Get the type of the encoding
            char type = input[encodingName.Length + 3];

            // Start after the name of the encoding and the other required parts
            Int32 i = encodingName.Length + 5;

            switch (char.ToLowerInvariant(type))
            {
                case 'q':
                    while (i < input.Length)
                    {
                        char currentChar = input[i];
                        char[] peekAhead = new char[2];
                        switch (currentChar)
                        {
                            case '=':
                                peekAhead = (i >= input.Length - 2) ? null : new char[] { input[i + 1], input[i + 2] };

                                if (peekAhead == null)
                                {
                                    sb.Append(currentChar);
                                    i++;
                                    break;
                                }

                                string decodedChar = enc.GetString(new byte[] { Convert.ToByte(new string(peekAhead, 0, 2), 16) });
                                sb.Append(decodedChar);
                                i += 3;
                                break;
                            case '?':
                                if (input[i + 1] == '=')
                                    i += 2;
                                break;
                            default:
                                sb.Append(currentChar);
                                i++;
                                break;
                        }
                    }
                    break;
                case 'b':
                    string baseString = input.Substring(i, input.Length - i - 2);
                    byte[] baseDecoded = Convert.FromBase64String(baseString);
                    sb.Append(enc.GetString(baseDecoded));
                    break;
            }
            return sb.ToString();
        }
    }
}
