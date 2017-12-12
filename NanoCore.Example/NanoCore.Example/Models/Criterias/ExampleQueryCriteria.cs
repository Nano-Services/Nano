﻿using DynamicExpression;
using Nano.Models.Criterias;

namespace NanoCore.Example.Models.Criterias
{
    /// <inheritdoc />
    public class ExampleQueryCriteria : DefaultQueryCriteria
    {
        /// <summary>
        /// Property One.
        /// </summary>
        public virtual string PropertyOne { get; set; }

        /// <inheritdoc />
        public override CriteriaExpression GetExpression<TEntity>()
        {
            var filter = base.GetExpression<TEntity>();

            if (this.PropertyOne != null)
                filter.StartsWith("PropertyOne", this.PropertyOne);

            return filter;
        }
    }
}