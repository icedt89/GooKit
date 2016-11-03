namespace JanHafner.GooKit.Common.FieldSelection
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a composition of a part that must be optimized.
    /// </summary>
    internal sealed class OptimizableFieldSelectorPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizableFieldSelectorPart"/> class.
        /// </summary>
        /// <param name="optimizablePart">The part to be optimized.</param>
        /// <param name="fullPart">The corresponding full part which is used to determine if this part can be optimized.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="fullPart"/>' and '<paramref name="optimizablePart"/>' cannot be null. </exception>
        public OptimizableFieldSelectorPart([NotNull] FieldSelectorPart optimizablePart,
            [NotNull] FieldSelectorPart fullPart)
        {
            if (optimizablePart == null)
            {
                throw new ArgumentNullException(nameof(optimizablePart));
            }

            if (fullPart == null)
            {
                throw new ArgumentNullException(nameof(fullPart));
            }

            this.OptimizablePart = optimizablePart;
            this.FullPart = fullPart;
        }

        /// <summary>
        /// The part to be optimized.
        /// </summary>
        [NotNull]
        public FieldSelectorPart OptimizablePart { get; private set; }

        /// <summary>
        /// the full part that is used to determine if the this part can be optimized.
        /// </summary>
        [NotNull]
        public FieldSelectorPart FullPart { get; private set; }
    }
}