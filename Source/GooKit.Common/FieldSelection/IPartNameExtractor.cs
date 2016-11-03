namespace JanHafner.GooKit.Common.FieldSelection
{
    using System;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides methods for extracting the name of a part from the <see cref="PropertyInfo"/>.
    /// </summary>
    public interface IPartNameExtractor
    {
        /// <summary>
        /// Extracts teh name of a part from the supplied <see cref="PropertyInfo"/>.
        /// </summary>
        /// <returns>The name of the part.</returns>
        [NotNull]
        String ExtractPartName([NotNull] PropertyInfo propertyInfo);
    }
}