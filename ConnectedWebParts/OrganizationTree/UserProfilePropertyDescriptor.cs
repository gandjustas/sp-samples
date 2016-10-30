// -----------------------------------------------------------------------
// <copyright file="UserProfilePropertyDescriptor.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ConnectedWebParts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using Microsoft.Office.Server.UserProfiles;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Class represents property descriptor for properties in UserProfile class.
    /// </summary>
    public class UserProfilePropertyDescriptor: PropertyDescriptor
    {
        public UserProfilePropertyDescriptor(ProfileSubtypeProperty propery)
            :base(propery.Name, new Attribute[] { new DisplayNameAttribute(propery.DisplayName)})
        {
            Contract.Requires(propery != null);
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(UserProfile); }
        }

        public override object GetValue(object component)
        {
            return Convert.ToString((component as UserProfile)[this.Name].Value);
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            (component as UserProfile)[this.Name].Value = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
