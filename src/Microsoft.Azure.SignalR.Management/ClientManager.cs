﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.SignalR.Management
{
    /// TODO: make public later
    internal abstract class ClientManager
    {
        /// <summary>
        /// Close a connection asynchronously.
        /// </summary>
        /// <param name="connectionId">The connection to close</param>
        /// <param name="reason">The reason to close the connection. </param>
        /// <param name="cancellationToken"></param>
        /// <returns>The created <see cref="System.Threading.Tasks.Task{TResult}">Task</see> that represents the asynchronous operation.</returns>
        /// <remarks>To get the <paramref name="reason"/> from connection closed event, client should set <see cref="NegotiationOptions.EnableDetailedErrors"/> during negotiation.</remarks>
        public abstract Task CloseConnectionAsync(string connectionId, string reason = null, CancellationToken cancellationToken = default);
    }
}