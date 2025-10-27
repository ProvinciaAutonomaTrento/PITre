// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.TextExtractors
{
    public class PageModel
    {
        public PageModel(int pageNumber, string textExtracted)
        {
            this.PageNumber = pageNumber;
            this.TextExtracted = textExtracted;
        }

        public int PageNumber { get; protected set; }

        public string TextExtracted { get; protected set; }
    }

    public class FileTextExtractionResultsModel
    {
        public FileTextExtractionResultsModel(List<PageModel> pages)
        {
            this.Pages = pages;
        }

        public List<PageModel> Pages 
        { 
            get; 
            protected set; 
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            this.Pages?.ForEach(p => sb.Append(p.TextExtracted));

            return sb.ToString();
        }
    }
}
