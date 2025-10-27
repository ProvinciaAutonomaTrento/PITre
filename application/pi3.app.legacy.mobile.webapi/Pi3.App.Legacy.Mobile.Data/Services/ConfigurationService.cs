// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;
using System.Security.Policy;
using System.Xml;

using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;

namespace Pi3.App.Legacy.Mobile.Data.Services;
public class ConfigurationService( 
    IConfigurationRepository configurationRepository) : IConfigurationService
{
    readonly IConfigurationRepository _configurationRepository = configurationRepository;

    public async Task<string?> GetChiaveByIdAmmAsync( string chiave, long idAmministrazione, CancellationToken cancellationToken)
    {
        return await this._configurationRepository.GetChiaveByIdAmmAsync(chiave, idAmministrazione, cancellationToken);
    }

    public IEnumerable<SERVICE_DTO.Istanza>? GetInstanceList()
    {
        List<SERVICE_DTO.Istanza>? istanze = null;
        string? istanzePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ListaIstanze.xml");
        if ( !String.IsNullOrEmpty(istanzePath) && File.Exists(istanzePath) )
        {
            XmlDocument xmlDoc = new ();
            string xmlContent = File.ReadAllText(istanzePath);
            xmlDoc.LoadXml(xmlContent);
            XmlNode? xmlNode = xmlDoc.ChildNodes[1];
            if(xmlNode != null)
            {
                string nome = String.Empty, descrizione = String.Empty, tipo = String.Empty, url = String.Empty;
                istanze = [];
                foreach ( XmlNode x1 in xmlNode.ChildNodes )
                {
                    if ( x1.Name == "Istanza" )
                    {
                        foreach ( XmlNode x2 in x1.ChildNodes )
                        {
                            switch ( x2.Name )
                            {
                                case "Nome":
                                    nome = x2.InnerText;
                                    break;
                                case "Descrizione":
                                    descrizione = x2.InnerText;
                                    break;
                                case "Tipo":
                                    tipo = x2.InnerText;
                                    break;
                                case "URL":
                                    url = x2.InnerText;
                                    break;
                            }
                        }
                        SERVICE_DTO.Istanza istanza = new()
                        {
                            Nome = nome,
                            Descrizione = descrizione,
                            Tipo = tipo,
                            Url = url
                        };

                        istanze.Add(istanza);
                    }
                }
            }
        } 
        else
        {
            throw new EXCEPTIONS.ExpectedResultNotFoundException("ListaIstanze file");
        }

        return istanze;
    }
}
