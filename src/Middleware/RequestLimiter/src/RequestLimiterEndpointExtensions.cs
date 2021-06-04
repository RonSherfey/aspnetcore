// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.RateLimits;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.RequestLimiter
{
    public static class RequestLimiterEndpointExtensions
    {
        public static IEndpointConventionBuilder EnforceDefaultRequestLimit(this IEndpointConventionBuilder builder)
        {
            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(new RequestLimitAttribute());
            });

            return builder;
        }

        public static IEndpointConventionBuilder EnforceRequestLimitPolicy(this IEndpointConventionBuilder builder, string policyName)
        {
            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(new RequestLimitAttribute(policyName));
            });

            return builder;
        }

        public static IEndpointConventionBuilder EnforceRequestRateLimit(this IEndpointConventionBuilder builder, int requestPerSecond)
        {
            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(
                    new RequestLimitAttribute(
                        new SimpleLimiterWrapper(
                            new TokenBucketRateLimiter(
                                new TokenBucketRateLimiterOptions
                                {
                                    PermitLimit = requestPerSecond,
                                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                                    TokensPerPeriod = requestPerSecond
                                }))));
            });

            return builder;
        }

        public static IEndpointConventionBuilder EnforceRequestConcurrencyLimit(this IEndpointConventionBuilder builder, int concurrentRequests)
        {
            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(
                    new RequestLimitAttribute(
                        new SimpleLimiterWrapper(
                            new ConcurrencyLimiter(
                                new ConcurrencyLimiterOptions { PermitLimit = concurrentRequests }))));
            });

            return builder;
        }

        public static IEndpointConventionBuilder EnforceRequestLimit(this IEndpointConventionBuilder builder, RateLimiter limiter)
        {
            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(new RequestLimitAttribute(new SimpleLimiterWrapper(limiter)));
            });

            return builder;
        }

        public static IEndpointConventionBuilder EnforceRequestLimit<TContext>(this IEndpointConventionBuilder builder, AggregatedRateLimiter<TContext> limiter, Func<HttpContext, TContext> selector) where TContext: notnull
        {
            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(new RequestLimitAttribute(new AggregatedLimiterWrapper<TContext>(limiter, selector)));
            });

            return builder;
        }
    }
}