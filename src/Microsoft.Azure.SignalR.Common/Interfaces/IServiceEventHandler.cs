﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

using Microsoft.Azure.SignalR.Protocol;

namespace Microsoft.Azure.SignalR;

public interface IServiceEventHandler
{
    Task HandleAsync(string connectionId, ServiceEventMessage message);
}
