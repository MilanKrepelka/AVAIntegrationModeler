using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ASOL.Core.ApiConnector;
using ASOL.DataService.Connector;
using ASOL.DataService.Connector.Options;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.AVAPlace;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pathoschild.Http.Client;

namespace ASOL.DataIntegrationExternalAgent.API.Connectors;

  /// <inheritdoc cref="ICustomDataServiceClient"/>
  public class CustomDataServiceClient : DataServiceClient, ICustomDataServiceClient
  {
      protected override string ApiVersionPrefix => "api/v1";

      /// <summary>
      /// Initializes the instance.
      /// </summary>
      /// <param name="options">Client options</param>
      /// <param name="logger">Logger</param>
      /// <param name="tokenProvider">Token provider</param>
      /// <param name="requestHeaderProviders">Request headers providers</param>
      public CustomDataServiceClient(
          IOptions<DataServiceClientOptions> options,
          ILogger<CustomDataServiceClient> logger,
          IConnectorTokenProvider tokenProvider,
          IEnumerable<IRequestHeaderProvider> requestHeaderProviders)
          : this(options?.Value!, logger, tokenProvider, requestHeaderProviders, baseClient: null)
      {
      }

      /// .ctor
      protected CustomDataServiceClient(
          DataServiceClientOptions options,
          ILogger<CustomDataServiceClient> logger,
          IConnectorTokenProvider tokenProvider,
          IEnumerable<IRequestHeaderProvider> requestHeaderProviders,
          HttpClient? baseClient = null,
          bool manageBaseClient = false)
          : base(options, logger, tokenProvider, requestHeaderProviders, baseClient, manageBaseClient)
      {
      }

      /// <inheritdoc/>
      public async Task<DataAgentDefinition?> GetDataAgentByCodeAsync(string agentCode, bool acceptNotFound = false, CancellationToken ct = default)
      {
          if (string.IsNullOrEmpty(agentCode)) throw new ArgumentNullException(nameof(agentCode));
          ct.ThrowIfCancellationRequested();

          var resource = $"{ApiVersionPrefix}/DataAgents/{Uri.EscapeDataString(agentCode)}";
          Logger.LogDebug($"Retrieving data-agent by code from {CombineUri(resource)}");

          var request = (await AddAuthentication(Client.GetAsync(resource), ct))
              .WithCancellationToken(ct);

          var result = await GetSingleResultAsync<DataAgentDefinition>(request, acceptNotFound, ct);
          return result;
      }

      /// <inheritdoc/>
      public async Task<bool> SwitchEnabledDataAgentAsync(string dataAgentId, bool enabled, CancellationToken ct = default)
      {
          if (string.IsNullOrEmpty(dataAgentId)) throw new ArgumentNullException(nameof(dataAgentId));
          ct.ThrowIfCancellationRequested();

          var resource = $"{ApiVersionPrefix}/DataAgents/{Uri.EscapeDataString(dataAgentId)}/SwitchEnabled";
          Logger.LogDebug($"Switching enabled for data-agent on {CombineUri(resource)}");

          var request = (await AddAuthentication(Client.PostAsync(resource), ct))
              .WithArgument(nameof(enabled), enabled)
              .WithOptions(ignoreHttpErrors: true)
              .WithCancellationToken(ct);

          var result = await request.AsMessage();
          switch (result.StatusCode)
          {
              case HttpStatusCode.NotFound:
                  return false;

              default:
                  result.EnsureSuccessStatusCode();
                  return true;
          }
      }

      /// <inheritdoc/>
      public async Task<bool> SwitchEnabledDataSourceAsync(string dataSourceId, bool enabled, CancellationToken ct = default)
      {
          if (string.IsNullOrEmpty(dataSourceId)) throw new ArgumentNullException(nameof(dataSourceId));
          ct.ThrowIfCancellationRequested();

          var resource = $"{ApiVersionPrefix}/DataSources/{Uri.EscapeDataString(dataSourceId)}/SwitchEnabled";
          Logger.LogDebug($"Switching enabled for data-source on {CombineUri(resource)}");

          var request = (await AddAuthentication(Client.PostAsync(resource), ct))
              .WithArgument(nameof(enabled), enabled)
              .WithOptions(ignoreHttpErrors: true)
              .WithCancellationToken(ct);

          var result = await request.AsMessage();
          switch (result.StatusCode)
          {
              case HttpStatusCode.NotFound:
                  return false;

              default:
                  result.EnsureSuccessStatusCode();
                  return true;
          }
      }
  }
