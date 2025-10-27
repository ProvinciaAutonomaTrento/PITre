// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
//using Pi3.Core.AggregateModels.ElementAggregate;
//using Pi3.Core.AggregateModels.UserAggregate;
//using Pi3.Core.SeedWork;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Security.AccessControl;
//using System.Text;
//using System.Threading.Tasks;

//namespace Pi3.Core.AggregateModels.OrganizationalUnitAggregate
//{
//    public class IndirizzoTelematico : ValueObject
//    {
//        public IndirizzoTelematico()
//        { }

//        [Required(AllowEmptyStrings = false)]
//        public string StringProperty { get; init; }
//    }

//    public class IndirizzoPostale : ValueObject
//    {
//        public IndirizzoPostale()
//        { }

//        [Required(AllowEmptyStrings = false)]
//        public string StringProperty { get; init; }
//    }

//    public class User : Entity<string>
//    {
//        #region Public Members

//        public User(string id, TextValue name)
//        {
//            this.Id = id;
//            this.Name = name;
//        }

//        public TextValue Name { get; protected set; }

//        #endregion

//        #region Private Members

//        #endregion
//    }


//    public class Registro : Entity<string>
//    {
//        #region Public Members

//        public Registro(string id, TextValue nome)
//        {
//            this.Id = id;
//            this.Nome = nome;
//        }

//        public TextValue Nome { get; protected set; }

//        #endregion

//        #region Private Members

//        #endregion
//    }

//    public class FunctionGroup : Entity<string>
//    {
//        #region Public Members

//        public FunctionGroup(string id, TextValue name)
//        {
//            this.Id = id;
//            this.Name = name;
//        }

//        public TextValue Name { get; protected set; }

//        #endregion

//        #region Private Members

//        #endregion
//    }


//    public class RoleType : Entity<string>
//    {
//        #region Public Members

//        public RoleType(string id, TextValue name, int hierarchicalLevel)
//        {
//            this.Id = id;
//            this.Name = name;
//            this.HierarchicalLevel = hierarchicalLevel;
//        }

//        public TextValue Name { get; protected set; }

//        public int HierarchicalLevel { get; protected set; }

//        #endregion

//        #region Private Members

//        #endregion
//    }

//    public class Role : Entity<string>
//    {
//        #region Public Members

//        public Role(string id, TextValue name, RoleType roleType)
//        {
//            this.Id = id;
//            this.Name = name;
//            this.RoleType = roleType;
//            this._users = new List<User>();
//            this._functionGroups = new List<FunctionGroup>();
//            this._registri = new List<Registro>();
//        }

//        internal void ChangeName(TextValue newName)
//        {
//            this.Name = newName;
//        }

//        public TextValue Name { get; protected set; }

//        internal void ChangeRoleType(RoleType newRoleType)
//        {
//            this.RoleType = newRoleType;
//        }

//        public RoleType RoleType { get; protected set; }

//        internal void AddUser(User user)
//        {
//            if (this._users.Contains(user))
//            {

//            }

//            this._users.Add(user);
//        }

//        internal void RemoveUser(User user)
//        {
//            if (!this._users.Contains(user))
//            {

//            }

//            this._users.Remove(user);
//        }


//        internal void RemoveUser(string idUser)
//        {
//        }

//        public IReadOnlyList<User> Users
//        {
//            get
//            {
//                return this._users.AsReadOnly();
//            }
//        }

//        internal void AddFunctionGroup(FunctionGroup functionGroup)
//        {
//            if (this._functionGroups.Contains(functionGroup))
//            {

//            }

//            this._functionGroups.Add(functionGroup);
//        }

//        internal void RemoveFunctionGroup(string idFunctionGroup)
//        {

//        }

//        public IReadOnlyList<FunctionGroup> Functions
//        {
//            get
//            {
//                return this._functionGroups.AsReadOnly();
//            }
//        }

//        internal void AddRegistro(Registro registro)
//        {

//        }

//        internal void RemoveRegistro(string idRegistro)
//        {

//        }

//        public IReadOnlyList<Registro> Registri
//        {
//            get
//            {
//                return this._registri.AsReadOnly();
//            }
//        }

//        #endregion

//        #region Private Members

//        protected readonly List<User> _users = null;
//        protected readonly List<FunctionGroup> _functionGroups = null;
//        protected readonly List<Registro> _registri = null;

//        #endregion
//    }

//    public class RoleCreated : Event
//    {
//        public RoleCreated()
//        { }

//        public TextValue RoleName { get; init; }

//        public TextValue RoleDescription { get; init; }

//        public RoleType RoleType { get; init; }
//    }
    
//    public class OrganizationalUnit : Element
//    {
//        #region Public Members

//        protected OrganizationalUnit() : base()
//        { }

//        public OrganizationalUnit(string idTenant, DateTime creationDate, TextValue name, TextValue? description = null)
//            : base(idTenant, "OrganizationalUnit", creationDate, name, description)
//        {
//        }

//        public OrganizationalUnit(
//            string id,
//            string idTenant,
//            DateTime creationDate,
//            TextValue name,
//            TextValue? description = null)
//            : base(id, idTenant, "OrganizationalUnit", creationDate, name, description)
//        {            
//        }

//        public void CreateNewRole(TextValue roleName, TextValue roleDescription, RoleType roleType)
//        {
//            roleName = roleName ?? throw new ArgumentNullException(nameof(roleName));
//            roleDescription = roleDescription ?? throw new ArgumentNullException(nameof(roleDescription));
//            roleType = roleType ?? throw new ArgumentNullException(nameof(roleType));

//            this.ApplyChange(new RoleCreated() { RoleName = roleName, RoleDescription = roleDescription, RoleType = roleType });
//        }

//        public IReadOnlyList<Role> Roles
//        {
//            get
//            {
//                return this._roles.AsReadOnly();
//            }
//        }

//        #endregion

//        #region Private Members

//        protected List<Role> _roles;

//        protected void Handle(RoleCreated @event)
//        {            
//        }

//        #endregion
//    }
//}
