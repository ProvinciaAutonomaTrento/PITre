// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ElementAggregate.ValueObjects
{
    public interface IElementFieldValue
    { }

    public abstract class ElementFieldValue : ValueObject, IElementFieldValue
    {
    }

    public class ElementFieldLookupValue : ElementFieldValue
    {
        public ElementFieldLookupValue()
        {
        }

        public ElementFieldLookupValue(string idValue, string descriptionValue)
        {
            IdValue = idValue;
            DescriptionValue = descriptionValue;
        }

        public string IdValue { get; init; }

        public string DescriptionValue { get; init; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return IdValue;
            yield return DescriptionValue;
        }

        public override string ToString()
        {
            return $"{IdValue} - {DescriptionValue}";
        }
    }

    public class ElementFieldSingleValue : ElementFieldValue
    {
        public ElementFieldSingleValue()
        {
        }

        public ElementFieldSingleValue(int int32Value)
        {
            Int32Value = int32Value;
            Value = int32Value;
        }

        public ElementFieldSingleValue(long int64Value)
        {
            Int64Value = int64Value;
            Value = int64Value;
        }

        public ElementFieldSingleValue(float floatValue)
        {
            FloatValue = floatValue;
            Value = floatValue;
        }

        public ElementFieldSingleValue(double doubleValue)
        {
            DoubleValue = doubleValue;
            Value = doubleValue;
        }

        public ElementFieldSingleValue(decimal decimalValue)
        {
            DecimalValue = decimalValue;
            Value = decimalValue;
        }

        public ElementFieldSingleValue(TextValue textValue)
        {
            TextValue = textValue;
            Value = textValue;
        }

        public ElementFieldSingleValue(bool booleanValue)
        {
            BooleanValue = booleanValue;
            Value = booleanValue;
        }

        public ElementFieldSingleValue(DateTime dateTimeValue)
        {
            DateTimeValue = dateTimeValue;
            Value = dateTimeValue;
        }

        public int? Int32Value { get; init; } = null;

        public long? Int64Value { get; init; } = null;

        public float? FloatValue { get; init; } = null;

        public double? DoubleValue { get; init; } = null;

        public decimal? DecimalValue { get; init; } = null;

        public TextValue? TextValue { get; init; } = null;

        public bool? BooleanValue { get; init; } = null;

        public DateTime? DateTimeValue { get; init; } = null;

        public dynamic Value
        {
            get;
            init;
        }

        public override string ToString()
        {
            return (Value ?? string.Empty).ToString();
        }
    }
    public class ElementFieldMultiValue : ElementFieldValue
    {
        public ElementFieldMultiValue()
        {
        }

        public ElementFieldMultiValue(params int[] int32Value)
        {
            Int32Value = int32Value;
            Value = int32Value;
        }

        public ElementFieldMultiValue(params long[] int64Value)
        {
            Int64Value = int64Value;
            Value = int64Value;
        }

        public ElementFieldMultiValue(params float[] floatValue)
        {
            FloatValue = floatValue;
            Value = floatValue;
        }

        public ElementFieldMultiValue(params double[] doubleValue)
        {
            DoubleValue = doubleValue;
            Value = doubleValue;
        }

        public ElementFieldMultiValue(params decimal[] decimalValue)
        {
            DecimalValue = decimalValue;
            Value = decimalValue;
        }

        public ElementFieldMultiValue(params TextValue[] textValue)
        {
            TextValue = textValue;
            Value = textValue;
        }

        public ElementFieldMultiValue(params bool[] booleanValue)
        {
            BooleanValue = booleanValue;
            Value = booleanValue;
        }

        public ElementFieldMultiValue(params DateTime[] dateTimeValue)
        {
            DateTimeValue = dateTimeValue;
            Value = dateTimeValue;
        }

        public int[]? Int32Value { get; init; } = null;

        public long[]? Int64Value { get; init; } = null;

        public float[]? FloatValue { get; init; } = null;

        public double[]? DoubleValue { get; init; } = null;

        public decimal[]? DecimalValue { get; init; } = null;

        public TextValue[]? TextValue { get; init; } = null;

        public bool[]? BooleanValue { get; init; } = null;

        public DateTime[]? DateTimeValue { get; init; } = null;

        public dynamic Value
        {
            get;
            init;
        }

        public override string ToString()
        {
            return (Value ?? string.Empty).ToString();
        }
    }
}
