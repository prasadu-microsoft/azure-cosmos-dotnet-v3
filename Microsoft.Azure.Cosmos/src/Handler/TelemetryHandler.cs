﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.Azure.Cosmos.Handlers
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Core.Trace;
    using Microsoft.Azure.Cosmos.Telemetry;

    internal class TelemetryHandler : RequestHandler
    {
        private readonly ClientTelemetry telemetry;
        private readonly CosmosClient cosmosClient;

        public TelemetryHandler(CosmosClient client, ClientTelemetry telemetry)
        {
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            this.cosmosClient = client ?? throw new ArgumentNullException(nameof(client));
        }

        public override async Task<ResponseMessage> SendAsync(
            RequestMessage request,
            CancellationToken cancellationToken)
        {
            ResponseMessage response = await base.SendAsync(request, cancellationToken);
            if (this.IsAllowed(request))
            {
                try
                {
                    ConsistencyLevel? consistencyLevel = await this.GetConsistencyLevelAsync(request);

                    this.telemetry
                        .Collect(
                                cosmosDiagnostics: response.Diagnostics,
                                statusCode: response.StatusCode,
                                responseSizeInBytes: this.GetPayloadSize(response),
                                containerId: request.ContainerId,
                                databaseId: request.DatabaseId,
                                operationType: request.OperationType,
                                resourceType: request.ResourceType,
                                consistencyLevel: consistencyLevel,
                                requestCharge: response.Headers.RequestCharge);
                }
                catch (Exception ex)
                {
                    DefaultTrace.TraceError("Error while collecting telemetry information : " + ex.Message);
                }
            }
            return response;
        }

        private bool IsAllowed(RequestMessage request)
        { 
            return ClientTelemetryOptions.AllowedResourceTypes.Equals(request.ResourceType);
        }

        private async Task<ConsistencyLevel?> GetConsistencyLevelAsync(RequestMessage request)
        {
            // Request level Consistency
           if (request.RequestOptions != null && request.RequestOptions.BaseConsistencyLevel.HasValue)
           {
                return request.RequestOptions.BaseConsistencyLevel;
           }

           // Client level Consistency
           if (this.cosmosClient.ClientOptions != null && this.cosmosClient.ClientOptions.ConsistencyLevel.HasValue)
           {
                return this.cosmosClient.ClientOptions.ConsistencyLevel;
           }
           
           // Account level Consistency
           return await this.cosmosClient.GetAccountConsistencyLevelAsync();   
        }

        /// <summary>
        /// It returns the payload size after reading it from the Response content stream. 
        /// To avoid blocking IO calls to get the stream length, it will return response content length if stream is of Memory Type
        /// otherwise it will return the content length from the response header (if it is there)
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Size of Payload</returns>
        private long GetPayloadSize(ResponseMessage response)
        {
            if (response != null)
            {
                if (response.Content != null && response.Content is MemoryStream)
                {
                    return response.Content.Length;
                }

                if (response.Headers != null && response.Headers.ContentLength != null)
                {
                    return long.Parse(response.Headers.ContentLength);
                }
            }

            return 0;
        }
    }
}
