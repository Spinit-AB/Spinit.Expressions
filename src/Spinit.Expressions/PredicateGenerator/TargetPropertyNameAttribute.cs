using System;

namespace Spinit.Expressions
{
    /// <summary>
    /// Attribute for specifying the name of the target property name.
    /// <para>Used by <see cref="PredicateGenerator{TSource, TTarget}"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TargetPropertyNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetPropertyNameAttribute"/> class.
        /// </summary>
        /// <param name="name">Name of the target property, recommendation is to use <see langword="nameof"/></param>
        public TargetPropertyNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Name of the target property
        /// </summary>
        public string Name { get; set; }
    }
}
