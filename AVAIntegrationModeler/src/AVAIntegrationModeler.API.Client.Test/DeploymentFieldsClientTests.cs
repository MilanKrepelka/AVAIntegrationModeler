using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Integrační testy pro atributy <c>Ticket</c> a <c>Description</c> na nasazení.
/// </summary>
public class DeploymentFieldsClientTests : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private readonly AVAIntegrationModelerAPIFactory _factory;

  public DeploymentFieldsClientTests(AVAIntegrationModelerAPIFactory factory)
  {
    _factory = factory;
  }

  private IAVAIntegrationModelerApiClient CreateClient()
  {
    var http = _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      BaseAddress = new Uri("http://0.0.0.0:5005")
    });
    return new AVAIntegrationModelerApiClient(http, new TestHttpClientFactory(http), NullLogger<AVAIntegrationModelerApiClient>.Instance);
  }

  private static DeploymentDTO NewDeployment(string? ticket = null, string? description = null) => new DeploymentDTO
  {
    Id = Guid.NewGuid(),
    Code = $"FLD-{Guid.NewGuid().ToString()[..8].ToUpper()}",
    Name = "Testovací nasazení",
    Ticket = ticket,
    Description = description,
    DataModelIds = []
  };

  /// <summary>
  /// Ticket a Description zadané při vytváření nasazení musí být přečteny zpět přes GetDeployment.
  /// </summary>
  [Fact]
  public async Task CreateDeployment_WithTicketAndDescription_FieldsPersist()
  {
    var client = CreateClient();
    var ticket = "https://jira.example.com/browse/TICKET-123";
    var description = "Popis testovacího nasazení pro ověření persistence.";
    var dto = NewDeployment(ticket, description);

    var createResult = await client.CreateDeployment(dto, CancellationToken.None);
    Assert.True(createResult.IsSuccess, $"Vytvoření selhalo: {string.Join(", ", createResult.Errors)}");

    var loaded = await client.GetDeployment(dto.Code, CancellationToken.None);

    Assert.Equal(ticket, loaded.Ticket);
    Assert.Equal(description, loaded.Description);
  }

  /// <summary>
  /// Nasazení bez Ticket a Description musí mít obě hodnoty null po načtení.
  /// </summary>
  [Fact]
  public async Task CreateDeployment_WithoutTicketAndDescription_FieldsAreNull()
  {
    var client = CreateClient();
    var dto = NewDeployment(ticket: null, description: null);

    var createResult = await client.CreateDeployment(dto, CancellationToken.None);
    Assert.True(createResult.IsSuccess, $"Vytvoření selhalo: {string.Join(", ", createResult.Errors)}");

    var loaded = await client.GetDeployment(dto.Code, CancellationToken.None);

    Assert.Null(loaded.Ticket);
    Assert.Null(loaded.Description);
  }

  /// <summary>
  /// Ticket a Description musí být aktualizovány přes UpdateDeployment a přetrvat.
  /// </summary>
  [Fact]
  public async Task UpdateDeployment_WithNewTicketAndDescription_FieldsPersist()
  {
    var client = CreateClient();
    var dto = NewDeployment();
    var createResult = await client.CreateDeployment(dto, CancellationToken.None);
    Assert.True(createResult.IsSuccess);

    var updatedTicket = "https://github.com/org/repo/issues/42";
    var updatedDescription = "Aktualizovaný popis nasazení.";
    var updateDto = dto with
    {
      Ticket = updatedTicket,
      Description = updatedDescription
    };

    var updateResult = await client.UpdateDeployment(updateDto, CancellationToken.None);
    Assert.True(updateResult.IsSuccess, $"Aktualizace selhala: {string.Join(", ", updateResult.Errors)}");

    var loaded = await client.GetDeployment(dto.Code, CancellationToken.None);
    Assert.Equal(updatedTicket, loaded.Ticket);
    Assert.Equal(updatedDescription, loaded.Description);
  }

  /// <summary>
  /// Aktualizace nasazení s null Ticket a Description musí vymazat původní hodnoty.
  /// </summary>
  [Fact]
  public async Task UpdateDeployment_ClearTicketAndDescription_FieldsBecomeNull()
  {
    var client = CreateClient();
    var dto = NewDeployment(
      ticket: "https://jira.example.com/browse/ORIG-1",
      description: "Původní popis.");
    var createResult = await client.CreateDeployment(dto, CancellationToken.None);
    Assert.True(createResult.IsSuccess);

    var clearDto = dto with { Ticket = null, Description = null };
    var updateResult = await client.UpdateDeployment(clearDto, CancellationToken.None);
    Assert.True(updateResult.IsSuccess);

    var loaded = await client.GetDeployment(dto.Code, CancellationToken.None);
    Assert.Null(loaded.Ticket);
    Assert.Null(loaded.Description);
  }

  /// <summary>
  /// Ticket a Description musí být vráceny i v seznamu nasazení (ListDeployments).
  /// </summary>
  [Fact]
  public async Task ListDeployments_AfterCreate_ContainsTicketAndDescription()
  {
    var client = CreateClient();
    var ticket = "https://azuredevops.example.com/workitems/999";
    var description = "Nasazení viditelné v seznamu.";
    var dto = NewDeployment(ticket, description);

    var createResult = await client.CreateDeployment(dto, CancellationToken.None);
    Assert.True(createResult.IsSuccess);

    var list = await client.GetDeployments(CancellationToken.None);
    var found = list.Deployments.FirstOrDefault(d => d.Code == dto.Code);

    Assert.NotNull(found);
    Assert.Equal(ticket, found.Ticket);
    Assert.Equal(description, found.Description);
  }
}
