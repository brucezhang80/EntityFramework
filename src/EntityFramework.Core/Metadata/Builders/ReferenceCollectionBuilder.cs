// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Metadata.Builders
{
    /// <summary>
    ///     <para>
    ///         Provides a simple API for configuring a one-to-many relationship.
    ///     </para>
    ///     <para>
    ///         Instances of this class are returned from methods when using the <see cref="ModelBuilder" /> API
    ///         and it is not designed to be directly constructed in your application code.
    ///     </para>
    /// </summary>
    public class ReferenceCollectionBuilder : IAccessor<Model>, IAccessor<InternalRelationshipBuilder>
    {
        private readonly InternalRelationshipBuilder _builder;

        /// <summary>
        ///     <para>
        ///         Initializes a new instance of the <see cref="ReferenceCollectionBuilder" /> class.
        ///     </para>
        ///     <para>
        ///         Instances of this class are returned from methods when using the <see cref="ModelBuilder" /> API
        ///         and it is not designed to be directly constructed in your application code.
        ///     </para>
        /// </summary>
        /// <param name="builder"> The internal builder being used to configure this relationship. </param>
        public ReferenceCollectionBuilder([NotNull] InternalRelationshipBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            _builder = builder;
        }

        /// <summary>
        ///     The foreign key that represents this relationship.
        /// </summary>
        public virtual ForeignKey Metadata => Builder.Metadata;

        /// <summary>
        ///     The model that this relationship belongs to.
        /// </summary>
        Model IAccessor<Model>.Service => Builder.ModelBuilder.Metadata;

        /// <summary>
        ///     Gets the internal builder being used to configure this relationship.
        /// </summary>
        InternalRelationshipBuilder IAccessor<InternalRelationshipBuilder>.Service => _builder;

        /// <summary>
        ///     Adds or updates an annotation on the relationship. If an annotation with the key specified in
        ///     <paramref name="annotation" />
        ///     already exists it's value will be updated.
        /// </summary>
        /// <param name="annotation"> The key of the annotation to be added or updated. </param>
        /// <param name="value"> The value to be stored in the annotation. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceCollectionBuilder Annotation([NotNull] string annotation, [NotNull] object value)
        {
            Check.NotEmpty(annotation, nameof(annotation));
            Check.NotNull(value, nameof(value));

            Builder.Annotation(annotation, value, ConfigurationSource.Explicit);

            return this;
        }

        /// <summary>
        ///     <para>
        ///         Configures the property(s) to use as the foreign key for this relationship.
        ///     </para>
        ///     <para>
        ///         If the specified property name(s) do not exist on the entity type then a new shadow state
        ///         property(s) will be added to serve as the foreign key. A shadow state property is one that does not
        ///         have a corresponding property in the entity class. The current value for the  property is stored in
        ///         the <see cref="ChangeTracker" /> rather than being stored in instances
        ///         of the entity class.
        ///     </para>
        ///     <para>
        ///         If <see cref="PrincipalKey" /> is not specified, then an attempt will be made to match
        ///         the data type and order of foreign key properties against the primary key of the principal
        ///         entity type. If they do not match, new shadow state properties that form a unique index will be
        ///         added to the principal entity type to serve as the reference key.
        ///     </para>
        /// </summary>
        /// <param name="foreignKeyPropertyNames">
        ///     The name(s) of the foreign key property(s).
        /// </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceCollectionBuilder ForeignKey([NotNull] params string[] foreignKeyPropertyNames)
        {
            Check.NotEmpty(foreignKeyPropertyNames, nameof(foreignKeyPropertyNames));

            return new ReferenceCollectionBuilder(Builder.ForeignKey(foreignKeyPropertyNames, ConfigurationSource.Explicit));
        }

        /// <summary>
        ///     Configures the unique property(s) that this relationship targets. Typically you would only call this
        ///     method if you want to use a property(s) other than the primary key as the principal property(s). If
        ///     the specified property(s) is not already a unique index (or the primary key) then a new unique index
        ///     will be introduced.
        /// </summary>
        /// <param name="keyPropertyNames"> The name(s) of the reference key property(s). </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceCollectionBuilder PrincipalKey([NotNull] params string[] keyPropertyNames)
        {
            Check.NotEmpty(keyPropertyNames, nameof(keyPropertyNames));

            return new ReferenceCollectionBuilder(Builder.PrincipalKey(keyPropertyNames, ConfigurationSource.Explicit));
        }

        /// <summary>
        ///     Configures whether this is a required relationship (i.e. whether the foreign key property(s) can
        ///     be assigned null).
        /// </summary>
        /// <param name="isRequired"> A value indicating whether this is a required relationship. </param>
        /// <returns> The same builder instance so that multiple configuration calls can be chained. </returns>
        public virtual ReferenceCollectionBuilder Required(bool isRequired = true)
            => new ReferenceCollectionBuilder(Builder.Required(isRequired, ConfigurationSource.Explicit));

        private InternalRelationshipBuilder Builder => ((IAccessor<InternalRelationshipBuilder>)this).Service;
    }
}
