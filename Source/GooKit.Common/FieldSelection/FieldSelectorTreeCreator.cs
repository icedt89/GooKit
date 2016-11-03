namespace JanHafner.GooKit.Common.FieldSelection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using JanHafner.Queryfy.Extensions;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides methods for traversing the LambdaExpression and creating the part tree.
    /// </summary>
    internal sealed class FieldSelectorTreeCreator<TRoot> : ExpressionVisitor
    {
        [NotNull]
        private readonly IPartNameExtractor partNameExtractor;

        [CanBeNull]
        private FieldSelectorTree rootPart;

        [NotNull]
        private readonly Stack<FieldSelectorPart> fieldSelectorParts = new Stack<FieldSelectorPart>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldSelectorTreeCreator{TRoot}"/> class.
        /// </summary>
        /// <param name="partNameExtractor">An implementation of the <see cref="IPartNameExtractor"/> interface which extracts the name of the part from the <see cref="PropertyInfo"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="partNameExtractor"/>' cannot be null. </exception>
        public FieldSelectorTreeCreator([NotNull] IPartNameExtractor partNameExtractor)
        {
            if (partNameExtractor == null)
            {
                throw new ArgumentNullException(nameof(partNameExtractor));
            }

            this.partNameExtractor = partNameExtractor;
        }

        /// <summary>
        /// Generates the full part tree for the <typeparam name="TRoot"> type.</typeparam>
        /// </summary>
        /// <param name="maximumDeep">The maximum recursion deep to prevent a <see cref="StackOverflowException" /> on endless nested types.</param>
        /// <returns>The part tree.</returns>
        [NotNull]
        public FieldSelectorTree GenerateFullFieldSelectorTree(Int32 maximumDeep)
        {
            var result = new FieldSelectorTree();

            var childParts = this.GenerateChildTreeFromProperties(typeof (TRoot).GetProperties(), 0, maximumDeep);
            foreach (var childPart in childParts)
            {
                result.AddPart(childPart);
            }

            return result;
        }

        /// <summary>
        /// Generates a list of <see cref="FieldSelectorPart"/> for each <see cref="PropertyInfo"/> in the supplied list.
        /// </summary>
        /// <param name="propertyInfos">The <see cref="PropertyInfo"/> instances.</param>
        /// <param name="currentDeep">The current deep of the recursion.</param>
        /// <param name="maximumDeep">The maximum recursion deep to prevent a <see cref="StackOverflowException" /> on endless nested types.</param>
        /// <returns>The list of <see cref="FieldSelectorPart"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="propertyInfos"/>' cannot be null. </exception>
        private IEnumerable<FieldSelectorPart> GenerateChildTreeFromProperties([NotNull] IEnumerable<PropertyInfo> propertyInfos, Int32 currentDeep, Int32 maximumDeep)
        {
            if (propertyInfos == null)
            {
                throw new ArgumentNullException(nameof(propertyInfos));
            }

            if (currentDeep == maximumDeep)
            {
                yield break;
            }

            foreach (var propertyInfo in propertyInfos)
            {
                var partName = this.partNameExtractor.ExtractPartName(propertyInfo);
                var childPart = new FieldSelectorPart(partName);

                var nextType = propertyInfo.PropertyType;

                if (!nextType.IsPrimitive())
                {
                    if (propertyInfo.PropertyType.IsIEnumerable() && !propertyInfo.PropertyType.IsPrimitive())
                    {
                        nextType = propertyInfo.PropertyType.GetSingleGenericParameter();
                    }

                    foreach (var childPartOfChildPart in this.GenerateChildTreeFromProperties(nextType.GetProperties(), currentDeep + 1, maximumDeep))
                    {
                        childPart.AddPart(childPartOfChildPart);
                    }
                }

                yield return childPart;
            }
        }

        /// <summary>
        /// Generates the part tree based on a <see cref="Expression{Func{TRoot, TProperty}}"/>.
        /// </summary>
        /// <typeparam name="TProperty">The <see cref="Type"/> of the selected property.</typeparam>
        /// <param name="selector">The selector.</param>
        /// <returns>The part tree.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="selector"/>' cannot be null. </exception>
        [NotNull]
        public FieldSelectorTree GenerateFromLambdaExpression<TProperty>([NotNull] Expression<Func<TRoot, TProperty>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            this.rootPart = new FieldSelectorTree();
            this.Visit(selector);

            FieldSelectorPart addCurrentToMe = null;
            foreach (var childPart in this.fieldSelectorParts)
            {
                if (!this.rootPart.ChildParts.Any())
                {
                    addCurrentToMe = childPart;
                    this.rootPart.AddPart(childPart);
                }
                else if (addCurrentToMe != null)
                {
                    addCurrentToMe.AddPart(childPart);
                    addCurrentToMe = childPart;
                }
            }

            var result = this.rootPart;
            this.rootPart = null;
            return result;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <param name="node">The expression to visit.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="node"/>' cannot be null. </exception>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var partName = this.partNameExtractor.ExtractPartName((PropertyInfo) node.Member);
            this.fieldSelectorParts.Push(new FieldSelectorPart(partName));

            return base.VisitMember(node);
        }
    }
}