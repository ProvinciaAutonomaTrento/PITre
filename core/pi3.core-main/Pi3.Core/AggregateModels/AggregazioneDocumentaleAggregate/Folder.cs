// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate
{
    public class Folder : Entity<string>
    {
        #region Public Members

        internal Folder(string id, TextValue name, IReadOnlyList<IdDoc>? idDocs = null)
        {
            this.Id = id;
            this.Name = name;
            this._idDocs = new List<IdDoc>(idDocs ?? new IdDoc[0]);
            this._folders = new List<Folder>();
        }

        internal void ChangeName(TextValue newName)
        {
            this.Name = newName;
        }

        public TextValue Name { get; protected set; }

        internal void AddIdDoc(IdDoc idDoc)
        {
            this._idDocs.Add(idDoc);
        }

        internal void RemoveIdDoc(IdDoc idDoc)
        {
            this._idDocs.Remove(idDoc);
        }

        public IReadOnlyList<IdDoc> IdDocs
        {
            get
            {
                return this._idDocs.AsReadOnly();
            }
        }

        internal void AddFolder(Folder folder)
        {
            this._folders.Add(folder);
        }

        internal void RemoveFolder(Folder folder)
        {
            this._folders.Remove(folder);
        }

        public IReadOnlyList<Folder> Folders
        {
            get
            {
                return this._folders.AsReadOnly();
            }
        }

        internal Folder? FindFolderById(string id)
        {
            foreach (var f in this._folders)
            {
                if (f.Id == id)
                    return f;
                else 
                    return f.FindFolderById(id);
            }

            return null;
        }

        #endregion

        #region Private Members

        protected readonly List<IdDoc> _idDocs;
        protected readonly List<Folder> _folders;

        #endregion
    }

}
