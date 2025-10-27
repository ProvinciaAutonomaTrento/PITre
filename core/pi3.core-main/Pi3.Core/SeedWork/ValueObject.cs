// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.Core.SeedWork
{
    public abstract class ValueObject : IValueObject
    {
        public ValueObject()
        {
        }

        protected static bool EqualOperator(ValueObject left, ValueObject right) 
        { 
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null)) 
            { 
                return false; 
            } 
            return ReferenceEquals(left, null) || left.Equals(right); 
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right) 
        { 
            return !(EqualOperator(left, right)); 
        }

        protected virtual IEnumerable<object> GetEqualityComponents()
        {
            var components = new List<object>();

            foreach (var p in this.GetType().GetProperties())
            {
                components.Add(p.GetValue(this));
            }

            return components;
        }

        public override bool Equals(object obj) 
        { 
            if (obj == null || obj.GetType() != GetType()) 
            { 
                return false; 
            } 
            
            var other = (ValueObject)obj; 
            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents()); 
        }

        public override int GetHashCode() 
        { 
            return GetEqualityComponents().Select(x => x != null ? x.GetHashCode() : 0).Aggregate((x, y) => x ^ y); 
        }

        public static bool operator ==(ValueObject one, ValueObject two) 
        { 
            return EqualOperator(one, two); 
        }

        public static bool operator !=(ValueObject one, ValueObject two) 
        { 
            return NotEqualOperator(one, two); 
        }
    }
}
