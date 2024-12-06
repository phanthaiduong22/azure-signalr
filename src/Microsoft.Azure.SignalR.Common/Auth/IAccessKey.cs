﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR;

internal interface IAccessKey
{
    public string Kid { get; }

    public byte[] KeyBytes { get; }

    public Task<string> GenerateAccessTokenAsync(string audience,
                                                 IEnumerable<Claim> claims,
                                                 TimeSpan lifetime,
                                                 AccessTokenAlgorithm algorithm,
                                                 CancellationToken ctoken = default);
}
