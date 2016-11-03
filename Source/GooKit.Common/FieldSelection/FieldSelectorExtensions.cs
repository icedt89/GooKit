namespace JanHafner.GooKit.Common.FieldSelection
{
    using System;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>
    /// Extension methods for the field selector utilities.
    /// </summary>
    public static class FieldSelectorExtensions
    {
        /// <summary>
        /// This factory method provides the caller with an instance of the <see cref="JsonFieldSelectorBuilder{TRoot}"/> class.
        /// Useful for anonymous types.
        /// </summary>
        /// <typeparam name="TRoot">The <see cref="Type"/> for which the instance should be created.</typeparam>
        /// <param name="type">An instance of the <see cref="JsonFieldSelectorBuilder{TRoot}"/>.</param>
        /// <param name="partNameExtractor">The <see cref="IPartNameExtractor"/> that extracts the name of the part from the <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="partNameExtractor"/>' and '<paramref name="type"/>' cannot be null.</exception>
        /// <returns>An instance of the <see cref="Type"/>.</returns>
        [NotNull]
        public static JsonFieldSelectorBuilder<TRoot> CreateFieldSelector<TRoot>([NotNull] this TRoot type,
            [NotNull] IPartNameExtractor partNameExtractor)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            
            if (partNameExtractor == null)
            {
                throw new ArgumentNullException(nameof(partNameExtractor));
            }

            return new JsonFieldSelectorBuilder<TRoot>(partNameExtractor);
        }

        /// <summary>
        /// This factory method provides the caller with an instance of the <see cref="JsonFieldSelectorBuilder{TRoot}"/> class.
        /// Useful for anonymous types.
        /// </summary>
        /// <typeparam name="TResource">The <see cref="Type"/> for which the instance should be created.</typeparam>
        /// <param name="partNameExtractor">The <see cref="IPartNameExtractor"/> that extracts the name of the part from the <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="partNameExtractor"/>' cannot be null.</exception>
        /// <returns>An instance of the <see cref="Type"/>.</returns>
        [NotNull]
        public static JsonFieldSelectorBuilder<TResource> CreateFieldSelector<TResource>(
            [NotNull] IPartNameExtractor partNameExtractor)
        {
            if (partNameExtractor == null)
            {
                throw new ArgumentNullException(nameof(partNameExtractor));
            }

            return new JsonFieldSelectorBuilder<TResource>(partNameExtractor);
        }
    }
}