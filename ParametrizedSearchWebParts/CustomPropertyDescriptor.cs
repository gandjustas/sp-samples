// -----------------------------------------------------------------------
// <copyright file="ObjectArrayMappingPropertyDescriptor.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ParametrizedSearchWebParts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Collections;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class CustomPropertyDescriptor: PropertyDescriptor
    {
        private Type type;

        public CustomPropertyDescriptor(string name, Type type)
            :base(name, new Attribute[0])
        {
            this.type = type; ;
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(IDictionary); }
        }

        public override object GetValue(object component)
        {
            return ((IDictionary)component)[this.Name];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return type; }
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            if (value != null)
            {
                ((IDictionary)component)[this.Name] = Convert.ChangeType(value, type);
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
