namespace JanHafner.GooKit.Common.FieldSelection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using JanHafner.Toolkit.Common.ExtensionMethods;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    /// The main class for building the selector expression.
    /// </summary>
    /// <typeparam name="TRoot">The <see cref="Type"/> of the type.</typeparam>
    public sealed class JsonFieldSelectorBuilder<TRoot>
    {
        [NotNull]
        private readonly IPartNameExtractor partNameExtractor;

        [NotNull]
        private readonly ICollection<FieldSelectorTree> rootFieldSelectorParts = new List<FieldSelectorTree>();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFieldSelectorBuilder{TRoot}"/> class.
        /// </summary>
        /// <param name="partNameExtractor">The <see cref="IPartNameExtractor"/> that extracts the name of the part from the <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="partNameExtractor"/>' cannot be null. </exception>
        public JsonFieldSelectorBuilder([NotNull] IPartNameExtractor partNameExtractor)
        {
            if (partNameExtractor == null)
            {
                throw new ArgumentNullException(nameof(partNameExtractor));
            }

            this.partNameExtractor = partNameExtractor;
        }

        /// <summary>
        /// Optimizes the whole tree.
        /// </summary>
        /// <param name="unoptimizedPart">The unoptimized tree on which the optimization is applied.</param>
        /// <param name="optimizeWith">The full tree which is used to process the optimization.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="optimizeWith"/>' and '<paramref name="unoptimizedPart"/>' cannot be null. </exception>
        private void Optimize([NotNull] FieldSelectorTree unoptimizedPart, [NotNull] FieldSelectorTree optimizeWith)
        {
            if (unoptimizedPart == null)
            {
                throw new ArgumentNullException(nameof(unoptimizedPart));
            }

            if (optimizeWith == null)
            {
                throw new ArgumentNullException(nameof(optimizeWith));
            }

            foreach (var combinedPart in unoptimizedPart.ChildParts.Select(m => new OptimizableFieldSelectorPart(m, optimizeWith.ChildParts.Single(n => n.PartName == m.PartName))))
            {
                this.OptimizePart(combinedPart);
            }
        }

        /// <summary>
        /// Optimizes a single part of the tree.
        /// </summary>
        /// <param name="selectedPart">The current part to optimize.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="selectedPart"/>' cannot be null. </exception>
        private void OptimizePart([NotNull] OptimizableFieldSelectorPart selectedPart)
        {
            if (selectedPart == null)
            {
                throw new ArgumentNullException(nameof(selectedPart));
            }

            foreach (var childPart in selectedPart.OptimizablePart.ChildParts.Select(m => new OptimizableFieldSelectorPart(m, selectedPart.FullPart.ChildParts.Single(n => n.PartName == m.PartName))))
            {
                this.OptimizePart(childPart);
            }

            if (selectedPart.OptimizablePart.ChildParts.Count() == selectedPart.FullPart.ChildParts.Count())
            {
                selectedPart.OptimizablePart.RemoveAllParts();
            }
        }

        /// <summary>
        /// Merges all part trees into one single tree.
        /// </summary>
        /// <param name="rootParts">A list of <see cref="FieldSelectorTree"/>.</param>
        /// <returns>A single <see cref="FieldSelectorTree"/> for further processing.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="rootParts"/>' cannot be null. </exception>
        [NotNull]
        private FieldSelectorTree MergeRootParts([NotNull] IEnumerable<FieldSelectorTree> rootParts)
        {
            if (rootParts == null)
            {
                throw new ArgumentNullException(nameof(rootParts));
            }

            var result = new FieldSelectorTree();
            foreach (var firstLevel in rootParts.SelectMany(m => m.ChildParts).GroupBy(m => m.PartName))
            {
                var childgroup = new FieldSelectorPart(firstLevel.Key);
                foreach (var part in this.MergeChildPart(firstLevel.SelectMany(m => m.ChildParts).GroupBy(m => m.PartName)))
                {
                    childgroup.AddPart(part);
                }

                result.AddPart(childgroup);
            }

            return result;
        }

        /// <summary>
        /// Merges a single group of parts into one part.
        /// </summary>
        /// <param name="groupedPart">A grouped list of all parts on the same level.</param>
        /// <returns>A list of merged parts.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="groupedPart"/>' cannot be null. </exception>
        [NotNull]
        private IEnumerable<FieldSelectorPart> MergeChildPart([NotNull] IEnumerable<IGrouping<String, FieldSelectorPart>> groupedPart)
        {
            if (groupedPart == null)
            {
                throw new ArgumentNullException(nameof(groupedPart));
            }

            foreach (var group in groupedPart)
            {
                var mergedChildPart = new FieldSelectorPart(group.Key);
                foreach (var childPart in this.MergeChildPart(group.SelectMany(m => m.ChildParts).GroupBy(m => m.PartName)))
                {
                    mergedChildPart.AddPart(childPart);
                }

                yield return mergedChildPart;
            }
        }

        /// <summary>
        /// Adds a new selector expression.
        /// </summary>
        /// <typeparam name="TProperty">The <see cref="Type"/> of the selected property.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="selector"/>' cannot be null. </exception>
        public void SelectProperty<TProperty>([NotNull] Expression<Func<TRoot, TProperty>> selector)
        {
            this.ThrowIfPropertyDontParticipateInResponse(selector);

            var selectedTree = new FieldSelectorTreeCreator<TRoot>(this.partNameExtractor).GenerateFromLambdaExpression(selector);

            this.rootFieldSelectorParts.Add(selectedTree);
        }

        /// <summary>
        /// Adds a new selector expression by first selecting a list and then by selecting properties of the items.
        /// </summary>
        /// <typeparam name="TItem">The <see cref="Type"/> of the list item.</typeparam>
        /// <typeparam name="TSubProperty">The <see cref="Type"/> of the sub property path.</typeparam>
        /// <param name="selector">The selector that selects the <see cref="IEnumerable{TItem}"/>.</param>
        /// <param name="subSelector">The selector that selects the property path of the item.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="selector"/>' and '<paramref name="subSelector"/>' cannot be null. </exception>
        public void SelectProperty<TItem, TSubProperty>([NotNull] Expression<Func<TRoot, IEnumerable<TItem>>> selector, [NotNull] Expression<Func<TItem, TSubProperty>> subSelector)
        {
            this.ThrowIfPropertyDontParticipateInResponse(selector);
            this.ThrowIfPropertyDontParticipateInResponse(subSelector);

            var collectionTree = new FieldSelectorTreeCreator<TRoot>(this.partNameExtractor).GenerateFromLambdaExpression(selector);
            var subTree = new FieldSelectorTreeCreator<TItem>(this.partNameExtractor).GenerateFromLambdaExpression(subSelector);

            var lastChild = collectionTree.ChildParts.WhereRecursive(part => part.ChildParts, (part, deep) => !part.ChildParts.Any()).Single();
            lastChild.AddPart(subTree.ChildParts.First());

            this.rootFieldSelectorParts.Add(collectionTree);
        }

        /// <summary>
        /// Builds the while selector expression that can be used be the request.
        /// It first, all <see cref="FieldSelectorTree"/> will be merged into one <see cref="FieldSelectorTree"/>.
        /// Next, the full tree for <typeparam name="TRoot"></typeparam> is generated.
        /// Than, the tree will be optimized, and last but not least: Build() is called on the optimized <see cref="FieldSelectorTree"/> and the value returned.
        /// </summary>
        /// <returns></returns>
        public String BuildSelectorExpression()
        {
            var mergedFieldSelectorPart = this.MergeRootParts(this.rootFieldSelectorParts);
            var maximumDeep = mergedFieldSelectorPart.GetDeep();
            var fullFieldSelectorTree = new FieldSelectorTreeCreator<TRoot>(this.partNameExtractor).GenerateFullFieldSelectorTree(maximumDeep);
            this.Optimize(mergedFieldSelectorPart, fullFieldSelectorTree);

            return mergedFieldSelectorPart.Build();
        }

        /// <summary>
        /// Checks if the selected property is annotated with the <see cref="JsonPropertyAttribute"/> and throws an <see cref="ArgumentException"/> if its not present.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="selector"></param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="selector"/>' cannot be null. </exception>
        private void ThrowIfPropertyDontParticipateInResponse<TNewRoot, TProperty>([NotNull] Expression<Func<TNewRoot, TProperty>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            var propertyInfo = selector.GetPropertyInfo();
            if (!propertyInfo.HasAttribute<JsonPropertyAttribute>())
            {
                throw new InvalidOperationException(String.Format(Properties.Exceptions.PropertyDoesNotParticipateInResponseExceptionMessage, propertyInfo.Name, propertyInfo.DeclaringType.Name));
            }
        }
    }
}