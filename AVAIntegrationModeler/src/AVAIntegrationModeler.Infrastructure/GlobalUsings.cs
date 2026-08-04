global using System.Net.Mail;
global using System.Reflection;
global using Ardalis.GuardClauses;
global using Ardalis.SharedKernel;
global using Ardalis.Specification.EntityFrameworkCore;
global using MailKit.Net.Smtp;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using MimeKit;

// Domain Aggregates
global using AVAIntegrationModeler.Domain.AreaAggregate;
global using AVAIntegrationModeler.Domain.ContributorAggregate;
global using AVAIntegrationModeler.Domain.DataModelAggregate;
global using AVAIntegrationModeler.Domain.FeatureAggregate; // ✅ PŘIDÁNO
global using AVAIntegrationModeler.Domain.ScenarioAggregate;
global using AVAIntegrationModeler.Domain.ValueObjects;
