using System;
using System.Threading.Tasks;
using ASOL.Core.Identity;
using ASOL.Core.Identity.Contexts;
using ASOL.Core.Identity.Options;
using ASOL.Core.Multitenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AVAIntegrationModeler.AVAPlace;

  /// <summary>
  /// Represents the service runtime/tenant context.
  /// </summary>
  public class ServiceRuntimeTenantContext : IDisposable
  {
      /// <summary>
      /// Executes in service context (impersonation via tenantId, clientId and clientSecret from appsettings).
      /// </summary>
      public static async Task<TResult> ExecuteInContextAsync<TProvider, TResult>(IServiceProvider serviceProvider, string tenantId, Func<TProvider, Task<TResult>> execute)
           where TProvider : notnull
      {
          TResult result;
          using (var runtimeContext = new ServiceRuntimeTenantContext(serviceProvider, tenantId))
          {
              var provider = runtimeContext.ServiceScope.ServiceProvider.GetRequiredService<TProvider>();
              result = await execute(provider);
          }
          return result;
      }

      /// <summary>
      /// Executes in service context (impersonation via tenantId, clientId and clientSecret from appsettings).
      /// </summary>
      public static async Task ExecuteInContextAsync<TProvider>(IServiceProvider serviceProvider, string tenantId, Func<TProvider, Task> execute)
           where TProvider : notnull
      {
          using (var runtimeContext = new ServiceRuntimeTenantContext(serviceProvider, tenantId))
          {
              var provider = runtimeContext.ServiceScope.ServiceProvider.GetRequiredService<TProvider>();
              await execute(provider);
          }
      }

      /// <summary>
      /// Executes in service context (impersonation via tenantId, clientId and clientSecret from appsettings).
      /// </summary>
      public static async Task<TResult> ExecuteInContextAsync<TResult>(IServiceProvider serviceProvider, string tenantId, Func<IServiceProvider, Task<TResult>> execute)
      {
          TResult result;
          using (var runtimeContext = new ServiceRuntimeTenantContext(serviceProvider, tenantId))
          {
              var provider = runtimeContext.ServiceScope.ServiceProvider;
              result = await execute(provider);
          }
          return result;
      }

      /// <summary>
      /// Executes in service context (impersonation via tenantId, clientId and clientSecret from appsettings).
      /// </summary>
      public static async Task ExecuteInContextAsync(IServiceProvider serviceProvider, string tenantId, Func<IServiceProvider, Task> execute)
      {
          using (var runtimeContext = new ServiceRuntimeTenantContext(serviceProvider, tenantId))
          {
              var provider = runtimeContext.ServiceScope.ServiceProvider;
              await execute(provider);
          }
      }

      public ServiceRuntimeTenantContext(IServiceProvider serviceProvider, string tenantId)
      {
          if (string.IsNullOrEmpty(tenantId)) throw new ArgumentNullException(nameof(tenantId));

          var ssoOptions = serviceProvider.GetService<IOptions<SsoAuthOptions>>()?.Value;
          if (ssoOptions == null)
          {
              throw new Exception($"{SsoAuthOptions.DefaultSectionName} section isn't defined.");
          }

          //runtime-context provides default values if identifiers aren't specified explicitly
          var runtimeContext = serviceProvider.GetRequiredService<IRuntimeContext>();
    // KRE var endpointData = runtimeContext.Security.EndpointData;
        var endpointData = new Dictionary<string, object>().AsReadOnly();
    //in case of service context, locale is inherited from runtime context (if specified)
    //KRE var locale = runtimeContext.Localization.LanguageCode;
    var locale = "en-US";
      if (string.IsNullOrEmpty(locale))
          {
              locale = LocalizationContext.DefaultLanguageCode;
          }
          var traceId = runtimeContext.Trace?.TraceId;
          if (traceId == null)
          {
              traceId = Guid.NewGuid().ToString();
          }

          ServiceScope = serviceProvider.CreateScope();
          ServiceScopeProvider = ServiceScope.ServiceProvider;

          var tenantContext = new TenantContext { Tenant = new TenantInfo(tenantId, null) };
          TenantId = tenantContext.TenantId;

          var tenantHolder = ServiceScope.ServiceProvider.GetRequiredService<ITenantContextHolder>();
          tenantHolder.TenantContext = tenantContext;

          var user = ClaimsPrincipalBuilder
              .CreateClientPrincipal(ssoOptions.ClientId!, tenantId)
              .AddAuthenticateType()
              .AddLocale(locale)
              .Build();

          var languageHolder = ServiceScope.ServiceProvider.GetRequiredService<ScopedContextHolder<ILocalizationContext>>();
          languageHolder.Context = new LocalizationContext(user);

          var securityHolder = ServiceScope.ServiceProvider.GetRequiredService<ScopedContextHolder<IRequestSecurityContext>>();
          securityHolder.Context = new RequestSecurityContext(user, endpointData);

          var traceHolder = ServiceScope.ServiceProvider.GetRequiredService<ScopedContextHolder<ITraceContext>>();
          traceHolder.Context = new TraceContext(traceId);
      }

      public string TenantId { get; }

      public IServiceScope ServiceScope { get; }

      public IServiceProvider ServiceScopeProvider { get; }

      /// <inheritdoc/>
      public void Dispose()
      {
          ServiceScope?.Dispose();
      }
  }
