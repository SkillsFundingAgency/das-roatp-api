﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoFixture.Dsl;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Extensions;

public static class AutoFixtureBuilderExtensions
{
    public static IPostprocessComposer<T> WithValues<T, TProperty>(
        this IPostprocessComposer<T> composer,
        Expression<Func<T, TProperty>> propertyPicker,
        params TProperty[] values)
    {
        var queue = new Queue<TProperty>(values);

        return composer.With(propertyPicker, () => queue.Dequeue());
    }
}
