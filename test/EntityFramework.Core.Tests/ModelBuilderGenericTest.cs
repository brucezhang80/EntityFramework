﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Metadata.ModelConventions;
using Xunit;

namespace Microsoft.Data.Entity.Tests.Metadata
{
    public class ModelBuilderGenericTest : ModelBuilderTest
    {
        [Fact]
        public void Can_create_a_model_builder_with_given_conventions_and_model()
        {
            var convention = new TestConvention();
            var conventions = new ConventionSet();
            conventions.EntityTypeAddedConventions.Add(convention);

            var model = new Model();
            var modelBuilder = new ModelBuilder(conventions, model);

            Assert.Same(model, modelBuilder.Model);

            modelBuilder.Entity<Random>();

            Assert.True(convention.Applied);
            Assert.NotNull(model.GetEntityType(typeof(Random)));
        }

        [Fact]
        public void Can_create_a_model_builder_with_given_conventions_only()
        {
            var convention = new TestConvention();
            var conventions = new ConventionSet();
            conventions.EntityTypeAddedConventions.Add(convention);

            var modelBuilder = new ModelBuilder(conventions);

            modelBuilder.Entity<Random>();

            Assert.True(convention.Applied);
            Assert.NotNull(modelBuilder.Model.GetEntityType(typeof(Random)));
        }

        private class TestConvention : IEntityTypeConvention
        {
            public bool Applied { get; set; }

            public InternalEntityTypeBuilder Apply(InternalEntityTypeBuilder entityTypeBuilder)
            {
                Applied = true;

                return entityTypeBuilder;
            }
        }

        protected override TestModelBuilder CreateTestModelBuilder(ModelBuilder modelBuilder)
        {
            return new GenericTestModelBuilder(modelBuilder);
        }

        private class GenericTestModelBuilder : TestModelBuilder
        {
            public GenericTestModelBuilder(ModelBuilder modelBuilder)
                : base(modelBuilder)
            {
            }

            public override TestEntityTypeBuilder<TEntity> Entity<TEntity>()
            {
                return new GenericTestEntityTypeBuilder<TEntity>(ModelBuilder.Entity<TEntity>());
            }

            public override TestModelBuilder Entity<TEntity>(Action<TestEntityTypeBuilder<TEntity>> buildAction)
            {
                return new GenericTestModelBuilder(ModelBuilder.Entity<TEntity>(entityTypeBuilder =>
                    buildAction(new GenericTestEntityTypeBuilder<TEntity>(entityTypeBuilder))));
            }

            public override TestModelBuilder Ignore<TEntity>()
            {
                return new GenericTestModelBuilder(ModelBuilder.Ignore<TEntity>());
            }
        }

        private class GenericTestEntityTypeBuilder<TEntity> : TestEntityTypeBuilder<TEntity>
            where TEntity : class
        {
            public GenericTestEntityTypeBuilder(EntityTypeBuilder<TEntity> entityTypeBuilder)
            {
                EntityTypeBuilder = entityTypeBuilder;
            }

            private EntityTypeBuilder<TEntity> EntityTypeBuilder { get; }
            public override EntityType Metadata => EntityTypeBuilder.Metadata;

            public override TestEntityTypeBuilder<TEntity> Annotation(string annotation, object value)
            {
                return new GenericTestEntityTypeBuilder<TEntity>(EntityTypeBuilder.Annotation(annotation, value));
            }

            public override TestKeyBuilder Key(Expression<Func<TEntity, object>> keyExpression)
            {
                return new TestKeyBuilder(EntityTypeBuilder.Key(keyExpression));
            }

            public override TestKeyBuilder Key(params string[] propertyNames)
            {
                return new TestKeyBuilder(EntityTypeBuilder.Key(propertyNames));
            }

            public override TestKeyBuilder AlternateKey(Expression<Func<TEntity, object>> keyExpression)
            {
                return new TestKeyBuilder(EntityTypeBuilder.AlternateKey(keyExpression));
            }

            public override TestKeyBuilder AlternateKey(params string[] propertyNames)
            {
                return new TestKeyBuilder(EntityTypeBuilder.AlternateKey(propertyNames));
            }

            public override TestPropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
            {
                return new GenericTestPropertyBuilder<TProperty>(EntityTypeBuilder.Property(propertyExpression));
            }

            public override TestPropertyBuilder<TProperty> Property<TProperty>(string propertyName)
            {
                return new GenericTestPropertyBuilder<TProperty>(EntityTypeBuilder.Property<TProperty>(propertyName));
            }

            public override TestEntityTypeBuilder<TEntity> Ignore(Expression<Func<TEntity, object>> propertyExpression)
            {
                return new GenericTestEntityTypeBuilder<TEntity>(EntityTypeBuilder.Ignore(propertyExpression));
            }

            public override TestEntityTypeBuilder<TEntity> Ignore(string propertyName)
            {
                return new GenericTestEntityTypeBuilder<TEntity>(EntityTypeBuilder.Ignore(propertyName));
            }

            public override TestIndexBuilder Index(Expression<Func<TEntity, object>> indexExpression)
            {
                return new TestIndexBuilder(EntityTypeBuilder.Index(indexExpression));
            }

            public override TestIndexBuilder Index(params string[] propertyNames)
            {
                return new TestIndexBuilder(EntityTypeBuilder.Index(propertyNames));
            }

            public override TestReferenceNavigationBuilder<TEntity, TRelatedEntity> Reference<TRelatedEntity>(Expression<Func<TEntity, TRelatedEntity>> reference = null)
            {
                return new GenericTestReferenceNavigationBuilder<TEntity, TRelatedEntity>(EntityTypeBuilder.Reference(reference));
            }

            public override TestCollectionNavigationBuilder<TEntity, TRelatedEntity> Collection<TRelatedEntity>(Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> collection = null)
            {
                return new GenericTestCollectionNavigationBuilder<TEntity, TRelatedEntity>(EntityTypeBuilder.Collection(collection));
            }
        }

        private class GenericTestPropertyBuilder<TProperty> : TestPropertyBuilder<TProperty>
        {
            public GenericTestPropertyBuilder(PropertyBuilder<TProperty> propertyBuilder)
            {
                PropertyBuilder = propertyBuilder;
            }

            private PropertyBuilder<TProperty> PropertyBuilder { get; }

            public override Property Metadata => PropertyBuilder.Metadata;

            public override TestPropertyBuilder<TProperty> Annotation(string annotation, object value)
            {
                return new GenericTestPropertyBuilder<TProperty>(PropertyBuilder.Annotation(annotation, value));
            }

            public override TestPropertyBuilder<TProperty> Required(bool isRequired = true)
            {
                return new GenericTestPropertyBuilder<TProperty>(PropertyBuilder.Required(isRequired));
            }

            public override TestPropertyBuilder<TProperty> MaxLength(int maxLength)
            {
                return new GenericTestPropertyBuilder<TProperty>(PropertyBuilder.MaxLength(maxLength));
            }

            public override TestPropertyBuilder<TProperty> ConcurrencyToken(bool isConcurrencyToken = true)
            {
                return new GenericTestPropertyBuilder<TProperty>(PropertyBuilder.ConcurrencyToken(isConcurrencyToken));
            }

            public override TestPropertyBuilder<TProperty> GenerateValueOnAdd(bool generateValue = true)
            {
                return new GenericTestPropertyBuilder<TProperty>(PropertyBuilder.GenerateValueOnAdd(generateValue));
            }

            public override TestPropertyBuilder<TProperty> StoreGeneratedPattern(StoreGeneratedPattern storeGeneratedPattern)
            {
                return new GenericTestPropertyBuilder<TProperty>(PropertyBuilder.StoreGeneratedPattern(storeGeneratedPattern));
            }
        }

        private class GenericTestReferenceNavigationBuilder<TEntity, TRelatedEntity> : TestReferenceNavigationBuilder<TEntity, TRelatedEntity>
            where TEntity : class
            where TRelatedEntity : class
        {
            public GenericTestReferenceNavigationBuilder(ReferenceNavigationBuilder<TEntity, TRelatedEntity> referenceNavigationBuilder)
            {
                ReferenceNavigationBuilder = referenceNavigationBuilder;
            }

            private ReferenceNavigationBuilder<TEntity, TRelatedEntity> ReferenceNavigationBuilder { get; }

            public override TestReferenceCollectionBuilder<TRelatedEntity, TEntity> InverseCollection(Expression<Func<TRelatedEntity, IEnumerable<TEntity>>> collection = null)
            {
                return new GenericTestReferenceCollectionBuilder<TRelatedEntity, TEntity>(ReferenceNavigationBuilder.InverseCollection(collection));
            }

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> InverseReference(Expression<Func<TRelatedEntity, TEntity>> reference = null)
            {
                return new GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity>(ReferenceNavigationBuilder.InverseReference(reference));
            }
        }

        private class GenericTestCollectionNavigationBuilder<TEntity, TRelatedEntity> : TestCollectionNavigationBuilder<TEntity, TRelatedEntity>
            where TEntity : class
            where TRelatedEntity : class
        {
            public GenericTestCollectionNavigationBuilder(CollectionNavigationBuilder<TEntity, TRelatedEntity> collectionNavigationBuilder)
            {
                CollectionNavigationBuilder = collectionNavigationBuilder;
            }

            private CollectionNavigationBuilder<TEntity, TRelatedEntity> CollectionNavigationBuilder { get; }

            public override TestReferenceCollectionBuilder<TEntity, TRelatedEntity> InverseReference(Expression<Func<TRelatedEntity, TEntity>> reference = null)
            {
                return new GenericTestReferenceCollectionBuilder<TEntity, TRelatedEntity>(CollectionNavigationBuilder.InverseReference(reference));
            }
        }

        private class GenericTestReferenceCollectionBuilder<TEntity, TRelatedEntity> : TestReferenceCollectionBuilder<TEntity, TRelatedEntity>
            where TEntity : class
            where TRelatedEntity : class
        {
            public GenericTestReferenceCollectionBuilder(ReferenceCollectionBuilder<TEntity, TRelatedEntity> referenceCollectionBuilder)
            {
                ReferenceCollectionBuilder = referenceCollectionBuilder;
            }

            private ReferenceCollectionBuilder<TEntity, TRelatedEntity> ReferenceCollectionBuilder { get; }

            public override ForeignKey Metadata => ReferenceCollectionBuilder.Metadata;

            public override TestReferenceCollectionBuilder<TEntity, TRelatedEntity> ForeignKey(Expression<Func<TRelatedEntity, object>> foreignKeyExpression)
            {
                return new GenericTestReferenceCollectionBuilder<TEntity, TRelatedEntity>(ReferenceCollectionBuilder.ForeignKey(foreignKeyExpression));
            }

            public override TestReferenceCollectionBuilder<TEntity, TRelatedEntity> PrincipalKey(Expression<Func<TEntity, object>> keyExpression)
            {
                return new GenericTestReferenceCollectionBuilder<TEntity, TRelatedEntity>(ReferenceCollectionBuilder.PrincipalKey(keyExpression));
            }

            public override TestReferenceCollectionBuilder<TEntity, TRelatedEntity> ForeignKey(params string[] foreignKeyPropertyNames)
            {
                return new GenericTestReferenceCollectionBuilder<TEntity, TRelatedEntity>(ReferenceCollectionBuilder.ForeignKey(foreignKeyPropertyNames));
            }

            public override TestReferenceCollectionBuilder<TEntity, TRelatedEntity> PrincipalKey(params string[] keyPropertyNames)
            {
                return new GenericTestReferenceCollectionBuilder<TEntity, TRelatedEntity>(ReferenceCollectionBuilder.PrincipalKey(keyPropertyNames));
            }

            public override TestReferenceCollectionBuilder<TEntity, TRelatedEntity> Annotation(string annotation, object value)
            {
                return new GenericTestReferenceCollectionBuilder<TEntity, TRelatedEntity>(ReferenceCollectionBuilder.Annotation(annotation, value));
            }

            public override TestReferenceCollectionBuilder<TEntity, TRelatedEntity> Required(bool isRequired = true)
            {
                return new GenericTestReferenceCollectionBuilder<TEntity, TRelatedEntity>(ReferenceCollectionBuilder.Required(isRequired));
            }
        }

        private class GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity> : TestReferenceReferenceBuilder<TEntity, TRelatedEntity>
            where TEntity : class
            where TRelatedEntity : class
        {
            public GenericTestReferenceReferenceBuilder(ReferenceReferenceBuilder<TEntity, TRelatedEntity> referenceReferenceBuilder)
            {
                ReferenceReferenceBuilder = referenceReferenceBuilder;
            }

            private ReferenceReferenceBuilder<TEntity, TRelatedEntity> ReferenceReferenceBuilder { get; }

            public override ForeignKey Metadata => ReferenceReferenceBuilder.Metadata;

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> Annotation(string annotation, object value)
            {
                return new GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity>(ReferenceReferenceBuilder.Annotation(annotation, value));
            }

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> ForeignKey<TDependentEntity>(Expression<Func<TDependentEntity, object>> foreignKeyExpression)
            {
                return new GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity>(ReferenceReferenceBuilder.ForeignKey(foreignKeyExpression));
            }

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> PrincipalKey<TPrincipalEntity>(Expression<Func<TPrincipalEntity, object>> keyExpression)
            {
                return new GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity>(ReferenceReferenceBuilder.PrincipalKey(keyExpression));
            }

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> ForeignKey(Type dependentEntityType, params string[] foreignKeyPropertyNames)
            {
                return new GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity>(ReferenceReferenceBuilder.ForeignKey(dependentEntityType, foreignKeyPropertyNames));
            }

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> PrincipalKey(Type principalEntityType, params string[] keyPropertyNames)
            {
                return new GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity>(ReferenceReferenceBuilder.PrincipalKey(principalEntityType, keyPropertyNames));
            }

            public override TestReferenceReferenceBuilder<TEntity, TRelatedEntity> Required(bool isRequired = true)
            {
                return new GenericTestReferenceReferenceBuilder<TEntity, TRelatedEntity>(ReferenceReferenceBuilder.Required(isRequired));
            }
        }
    }
}
