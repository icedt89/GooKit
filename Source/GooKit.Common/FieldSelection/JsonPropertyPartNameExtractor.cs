namespace JanHafner.GooKit.Common.FieldSelection
{
    using System;
    using System.Reflection;

    using JanHafner.Toolkit.Common.ExtensionMethods;
    using Newtonsoft.Json;

    /// <summary>
    /// Uses the json property name (specified with JSON.NET) as name and falls back to the property name if not present.
    /// </summary>
    public sealed class JsonPropertyPartNameExtractor : IPartNameExtractor
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

            var jsonPropertyAttribute = propertyInfo.GetAttribute<JsonPropertyAttribute>();
            if (jsonPropertyAttribute == null)
            {
                return propertyInfo.Name;
            }

            return jsonPropertyAttribute.PropertyName;
        }
    }
}