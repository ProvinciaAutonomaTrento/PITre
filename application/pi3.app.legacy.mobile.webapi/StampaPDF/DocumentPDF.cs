// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StampaPDF
{
    // deriva dalla classe document di iTextSharp
    //public class DocumentPDF : Document
    public class DocumentPDF    {
        public MemoryStream memoryStream;

        public DocumentPDF()
        {
            memoryStream = new MemoryStream();
        }

    }
}
