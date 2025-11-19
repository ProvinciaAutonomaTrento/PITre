// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.Services.File.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate
{
    public static class FormatiFile
    {
        public static IReadOnlyList<FormatoFile> Get()
        {
            var formati = new List<FormatoFile>();

            var assembly = Assembly.GetExecutingAssembly();

            string formatiFileContent = String.Empty;

            using (var stream = assembly.GetManifestResourceStream("Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.FormatiFile.xml"))
            {
                foreach (var e in XElement.Load(stream).Elements())
                {
                    formati.Add(new FormatoFile()
                    {
                        Nome = e.Element("Nome").Value,
                        ContentType = e.Element("ContentType").Value,
                        Estensione = e.Element("Estensione").Value,
                        Produttore = e.Element("Produttore").Value,
                        Versione = e.Element("Versione").Value
                    });
                }
            }
            
            return formati.AsReadOnly();
        }
    }
}
