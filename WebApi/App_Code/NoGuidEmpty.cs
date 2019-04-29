using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class NoGuidEmpty : ValidationAttribute
    {
        public override bool IsValid(object value) => (Guid)value != Guid.Empty;
    }
}
