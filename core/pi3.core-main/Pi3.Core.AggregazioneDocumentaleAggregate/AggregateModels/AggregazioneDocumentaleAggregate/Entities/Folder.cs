// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Entities
{
    public class Folder : Entity<string>
    {
        #region Public Members

        internal Folder(string id, TextValue name, IReadOnlyList<IdDoc>? idDocs = null, Folder? parent = null)
        {
            Id = id;
            Name = name;
            _idDocs = new List<IdDoc>(idDocs ?? new IdDoc[0]);
            _parent = parent;
            _folders = new List<Folder>();
        }

        internal void ChangeName(TextValue newName)
        {
            Name = newName;
        }

        public TextValue Name { get; protected set; }

        internal void AddIdDoc(IdDoc idDoc)
        {
            _idDocs.Add(idDoc);
        }

        internal void RemoveIdDoc(IdDoc idDoc)
        {
            _idDocs.Remove(idDoc);
        }

        public Folder? Parent
        {
            get
            {
                return _parent;
            }
        }

        public IReadOnlyList<IdDoc> IdDocs
        {
            get
            {
                return _idDocs.AsReadOnly();
            }
        }

        internal void AddFolder(Folder folder)
        {
            _folders.Add(folder);
        }

        internal void RemoveFolder(Folder folder)
        {
            _folders.Remove(folder);
        }

        public IReadOnlyList<Folder> Folders
        {
            get
            {
                return _folders.AsReadOnly();
            }
        }

        internal Folder? FindFolderById(string id)
        {
            Folder folderFound = null;
            foreach (var f in _folders)
            {
                if (f.Id == id)
                    return f;
                else if (f.Folders.Any())
                {
                    folderFound = f.FindFolderById(id);
                    if (folderFound != null)
                        return folderFound;
                }
                    
                
            }

            return null;
        }

        #endregion

        #region Private Members

        protected readonly Folder? _parent;
        protected readonly List<IdDoc> _idDocs;
        protected readonly List<Folder> _folders;

        #endregion
    }

}
