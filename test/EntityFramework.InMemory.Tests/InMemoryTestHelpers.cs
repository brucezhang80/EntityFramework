// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.Data.Entity.Tests
{
    public class InMemoryTestHelpers : TestHelpers
    {
        protected InMemoryTestHelpers()
        {
        }

        public new static InMemoryTestHelpers Instance { get; } = new InMemoryTestHelpers();

        protected override EntityFrameworkServicesBuilder AddProviderServices(EntityFrameworkServicesBuilder builder)
        {
            return builder.AddInMemoryStore();
        }

        protected override void UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
