using System;
using System.Collections.Generic;
using System.Linq;
using AVAIntegrationModeler.Domain.ValueObjects;

namespace AVAIntegrationModeler.Domain.FeatureAggregate;

/// <summary>
/// Aggregate root pro integrační feature.
/// </summary>
public class Feature : EntityBase<Guid>, IAggregateRoot
{
  // Privátní konstruktor pro EF Core
  private Feature() { }

  /// <summary>
  /// Konstruktor pro vytvoření nové integrační feature.
  /// </summary>
  /// <param name="id">Identifikátor feature.</param>
  /// <param name="code">Kód feature.</param>
  public Feature(Guid id, string code)
  {
    Id = id;
    SetCode(code);
  }

  /// <summary>
  /// Technický kód feature, který slouží k jeho jednoznačné identifikaci v rámci systému.
  /// </summary>
  public string Code { get; private set; } = string.Empty;

  /// <summary>
  /// Název integrační feature (lokalizovaný).
  /// </summary>
  public LocalizedValue Name { get; private set; } = new();

  /// <summary>
  /// Popis integrační feature (lokalizovaný).
  /// </summary>
  public LocalizedValue Description { get; private set; } = new();

  /// <summary>
  /// Privátní kolekce zahrnutých features.
  /// </summary>
  private readonly List<IncludedFeature> _includedFeatures = new();

  /// <summary>
  /// Read-only kolekce zahrnutých features.
  /// </summary>
  public IReadOnlyCollection<IncludedFeature> IncludedFeatures => _includedFeatures.AsReadOnly();

  /// <summary>
  /// Privátní kolekce zahrnutých modelů.
  /// </summary>
  private readonly List<IncludedModel> _includedModels = new();

  /// <summary>
  /// Read-only kolekce zahrnutých modelů.
  /// </summary>
  public IReadOnlyCollection<IncludedModel> IncludedModels => _includedModels.AsReadOnly();

  /// <summary>
  /// Nastaví kód feature.
  /// </summary>
  /// <param name="code">Nový kód.</param>
  public Feature SetCode(string code)
  {
    Code = Guard.Against.NullOrEmpty(code, nameof(code));
    return this;
  }

  /// <summary>
  /// Nastaví název feature.
  /// </summary>
  /// <param name="name">Nový lokalizovaný název.</param>
  public Feature SetName(LocalizedValue name)
  {
    Name = Guard.Against.Null(name, nameof(name));
    return this;
  }

  /// <summary>
  /// Nastaví popis feature.
  /// </summary>
  /// <param name="description">Nový lokalizovaný popis.</param>
  public Feature SetDescription(LocalizedValue description)
  {
    Description = Guard.Against.Null(description, nameof(description));
    return this;
  }

  /// <summary>
  /// Přidá zahrnutou feature.
  /// </summary>
  /// <param name="featureId">ID feature k zahrnutí.</param>
  /// <param name="consumeOnly">Příznak pouze pro konzumaci.</param>
  public Feature AddIncludedFeature(Guid featureId, bool consumeOnly = false)
  {
    Guard.Against.Default(featureId, nameof(featureId));

    // Business rule: nelze zahrnout sám sebe
    if (featureId == this.Id)
    {
      throw new InvalidOperationException("Feature cannot include itself.");
    }

    // Business rule: nelze přidat duplicitní feature
    if (_includedFeatures.Any(f => f.FeatureId == featureId))
    {
      return this; // již existuje
    }

    var includedFeature = new IncludedFeature(featureId, consumeOnly);
    _includedFeatures.Add(includedFeature);
    
    return this;
  }

  /// <summary>
  /// Odebere zahrnutou feature.
  /// </summary>
  /// <param name="featureId">ID feature k odebrání.</param>
  public Feature RemoveIncludedFeature(Guid featureId)
  {
    var feature = _includedFeatures.FirstOrDefault(f => f.FeatureId == featureId);
    if (feature != null)
    {
      _includedFeatures.Remove(feature);
    }
    
    return this;
  }

  /// <summary>
  /// Přidá zahrnutý model.
  /// </summary>
  /// <param name="modelId">ID modelu k zahrnutí.</param>
  /// <param name="consumeOnly">Příznak pouze pro konzumaci.</param>
  public Feature AddIncludedModel(Guid modelId, bool consumeOnly = false)
  {
    Guard.Against.Default(modelId, nameof(modelId));

    // Business rule: nelze přidat duplicitní model
    if (_includedModels.Any(m => m.ModelId == modelId))
    {
      return this; // již existuje
    }

    var includedModel = new IncludedModel(modelId)
    {
      ReadOnly = consumeOnly
    };
    
    _includedModels.Add(includedModel);
    
    return this;
  }

  /// <summary>
  /// Odebere zahrnutý model.
  /// </summary>
  /// <param name="modelId">ID modelu k odebrání.</param>
  public Feature RemoveIncludedModel(Guid modelId)
  {
    var model = _includedModels.FirstOrDefault(m => m.ModelId == modelId);
    if (model != null)
    {
      _includedModels.Remove(model);
    }
    
    return this;
  }

  /// <summary>
  /// Najde zahrnutou feature podle ID.
  /// </summary>
  /// <param name="featureId">ID feature.</param>
  /// <returns>IncludedFeature nebo null.</returns>
  public IncludedFeature? GetIncludedFeature(Guid featureId)
  {
    return _includedFeatures.FirstOrDefault(f => f.FeatureId == featureId);
  }

  /// <summary>
  /// Najde zahrnutý model podle ID.
  /// </summary>
  /// <param name="modelId">ID modelu.</param>
  /// <returns>IncludedModel nebo null.</returns>
  public IncludedModel? GetIncludedModel(Guid modelId)
  {
    return _includedModels.FirstOrDefault(m => m.ModelId == modelId);
  }
}
