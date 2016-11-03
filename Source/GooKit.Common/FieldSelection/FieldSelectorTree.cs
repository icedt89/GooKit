namespace JanHafner.GooKit.Common.FieldSelection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines the root part of the tree.
    /// </summary>
    internal sealed class FieldSelectorTree
    {
        [NotNull]
        private readonly ICollection<FieldSelectorPart> childParts = new List<FieldSelectorPart>();
        
        /// <summary>
        /// All dependant parts of this part.
        /// </summary>
        [NotNull]
        public IEnumerable<FieldSelectorPart> ChildParts
        {
            get { return this.childParts; }
        }

        /// <summary>
        /// Adds a new part.
        /// </summary>
        /// <param name="childPart">The new part to be added.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="childPart"/>' cannot be null. </exception>
        public void AddPart([NotNull] FieldSelectorPart childPart)
        {
            if (childPart == null)
            {
                throw new ArgumentNullException(nameof(childPart));
            }

            this.childParts.Add(childPart);
        }

        /// <summary>
        /// Returns the overall deep of the complete structure.
        /// </summary>
        /// <returns></returns>
        public Int32 GetDeep()
        {
            if (this.childParts.Any())
            {
                return 1 + this.childParts.Max(m => m.GetDeep());
            }

            return 0;
        }

        /// <summary>
        /// Builds the selector expression as a <see cref="String"/> by calling the same method on each child part and concatenating the results.
        /// </summary>
        /// <returns>The built selector.</returns>
        [NotNull]
        public String Build()
        {
            return String.Format("{0}", String.Join(",", this.ChildParts.Select(m => m.Build())));
        }
    }
}