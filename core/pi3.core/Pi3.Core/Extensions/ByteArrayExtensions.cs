// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] ComputeHashAsSha512(this byte[] content)
        {
            using (var algorithm = HashAlgorithm.Create("SHA512"))
                return algorithm.ComputeHash(content);
        }

        public static string ComputeHashAsSha512String(this byte[] content)
        {
            return Convert.ToBase64String(ComputeHashAsSha512(content));
        }

        public static byte[] ComputeHashAsSha256(this byte[] content)
        {
            using (var algorithm = HashAlgorithm.Create("SHA256"))
                return algorithm.ComputeHash(content);
        }

        public static string ComputeHashAsSha256String(this byte[] content)
        {
            return Convert.ToBase64String(ComputeHashAsSha256(content));
        }
    }
}
