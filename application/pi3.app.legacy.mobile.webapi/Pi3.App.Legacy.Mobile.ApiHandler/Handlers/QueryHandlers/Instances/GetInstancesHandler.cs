// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using System.Xml;



namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Instances;
public class GetInstancesHandler: IRequestHandler<QUERY.GetInstanceList, IEnumerable<DTO.Instances.Instance>>
{

    public async Task<IEnumerable<DTO.Instances.Instance>> Handle( QUERY.GetInstanceList request, CancellationToken cancellationToken )
    {
        IEnumerable<DTO.Instances.Instance> result = await Task.Run<IEnumerable<DTO.Instances.Instance>>(() =>
        {
            List<DTO.Instances.Instance> tempInstances = [];
            string? istanzePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ListaIstanze.xml");
            if ( !String.IsNullOrEmpty(istanzePath) && File.Exists(istanzePath) )
            {
                XmlDocument xmlDoc = new();
                string xmlContent = File.ReadAllText(istanzePath);
                xmlDoc.LoadXml(xmlContent);
                XmlNode? xmlNode = xmlDoc.ChildNodes[0];
                if ( xmlNode != null )
                {
                    string nome = String.Empty, descrizione = String.Empty, tipo = String.Empty, url = String.Empty;
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
                            DTO.Instances.Instance istanza = new()
                            {
                                Nome = nome,
                                Descrizione = descrizione,
                                Tipo = tipo,
                                URL = url
                            };

                            tempInstances.Add(istanza);
                        }
                    }
                }
            }
            return tempInstances;
        });

        if ( result == null || !result.Any() )
        {
            throw new EXCEPTIONS.ExpectedResultNotFoundException("ListaIstanze file");
        }

        return result;
    }
}
