﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.IntegrationTests.Data.SqlLite.Fixtures;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AVAIntegrationModeler.IntegrationTests.Data.SqlLite.Scenarios;
public class ScenarioDeleteTest(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture) : TestBed<EfSqlClientTestFixture>(testOutputHelper, fixture)
{
  [Fact]
  public async Task Delete_Single_Scenario_Not_Fails_Test()
  {
    var repository = _fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IRepository<Core.ScenarioAggregate.Scenario>>();
    var context = _fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<AppDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    await repository.AddAsync(SeedData.Scenario1, CancellationToken.None);
    var result = await repository.GetByIdAsync(SeedData.Scenario1.Id, CancellationToken.None);
    result!.Id.ShouldBe(SeedData.Scenario1.Id);
    await repository.DeleteAsync(result, CancellationToken.None);
    var deletedResult = await repository.GetByIdAsync(SeedData.Scenario1.Id, CancellationToken.None);
    deletedResult.ShouldBeNull();
  }
}
