using System;
namespace AVAIntegrationModeler.AVAPlace.Connectors;

  public record IntegrationAgentSettings
  {
      public required string ApplicationCode { get; init; }

      public required Guid AgentId { get; init; }
      public required string AgentCode { get; init; }

      public required Guid SourceId { get; init; }
      public required string SourceCode { get; init; }

      public required string? ConsumerCode { get; init; }
      public required string? ConsumerChannel { get; init; }
  }
