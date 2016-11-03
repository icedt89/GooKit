namespace JanHafner.GooKit.Common.FieldSelection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>
    /// Defines a part in the hierarchy of the selector expression.
    /// </summary>
    internal sealed class FieldSelectorPart
    {
        [NotNull]
        private readonly ICollection<FieldSelectorPart> childParts = new List<FieldSelectorPart>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldSelectorPart"/> class with the specified part name.
        /// This instance will become a child.
        /// </summary>
        /// <param name="partName">The name of the part.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="partName"/>' cannot be null. </exception>
        public FieldSelectorPart([NotNull] String partName)
        {
            if (String.IsNullOrEmpty(partName))
            {
                throw new ArgumentNullException(nameof(partName));
            }

            if (String.IsNullOrEmpty(partName))
            {
                throw new ArgumentNullException(nameof(partName));
            }

            this.PartName = partName;
        }

        /// <summary>
        /// All dependants parts of this part.
        /// </summary>
        [NotNull]
        public IEnumerable<FieldSelectorPart> ChildParts
        {
            get { return this.childParts; }
        }

        /// <summary>
        /// The name of this part.
        /// </summary>
        [NotNull]
        public String PartName { get; private set; }

        /// <summary>
        /// Removes all dependant parts of this instance.
        /// </summary>
        public void RemoveAllParts()
        {
            this.childParts.Clear();
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
        /// Gets the deep of this part.
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
            var result = this.PartName;
            if (this.ChildParts.Any())
            {
                var evaluatedChildren = String.Join(",", this.ChildParts.Select(m => m.Build()));
                result = String.Format(this.ChildParts.Count() > 1 ? "{0}({1})" : "{0}/{1}", this.PartName, evaluatedChildren);
            }

            return result;
        }
    }
}