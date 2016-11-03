namespace JanHafner.GooKit.Common.FieldSelection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Uses the name of the <see cref="PropertyInfo"/> as name for the part.
    /// </summary>
    public sealed class PropertyNamePartNameExtractor : IPartNameExtractor
    {
        /// <summary>
        /// Extracts teh name of a part from the supplied <see cref="PropertyInfo"/>.
        /// </summary>
        /// <returns>The name of the part.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfo"/>' cannot be null. </exception>
        public String ExtractPartName(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return propertyInfo.Name;
        }
    }
}