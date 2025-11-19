// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;

namespace Pi3.App.AggregazioneDocumentale.WebApi.Application.Commands
{
    public static class AggregateHelpers
    {
        public static Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy
            FolderHierarcy2ValueObject(FolderHierarcy element)
        {
            var docs = new List<Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc>();
            var folders = new List<Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy>();

            foreach (var doc in element.Documenti ?? Enumerable.Empty<IdDoc>())
            {
                docs.Add(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc()
                {
                    Identiticativo = doc.Identiticativo
                });
            }

            foreach (var folder in element.Sottofascicoli ?? Enumerable.Empty<FolderHierarcy>())
            {
                folders.Add(FolderHierarcy2ValueObject(folder));
            }

            var result = new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.FolderHierarcy()
            {
                Name = new TextValue(element.Nome),
                IdDocs = docs,
                Folders = folders
            };
            return result;
        }

    }
}
