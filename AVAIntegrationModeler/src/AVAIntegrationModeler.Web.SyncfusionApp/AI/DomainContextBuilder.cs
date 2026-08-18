using System.Text;
using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Web.SyncfusionApp.AI;

/// <summary>
/// Načítá doménová data z API a sestavuje Markdown systémový prompt pro Claude.
/// </summary>
public class DomainContextBuilder
{
    private readonly IAVAIntegrationModelerApiClient _apiClient;

    /// <summary>
    /// Inicializuje novou instanci <see cref="DomainContextBuilder"/>.
    /// </summary>
    public DomainContextBuilder(IAVAIntegrationModelerApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    /// <summary>
    /// Načte aktuální doménová data pro zadaný datový zdroj a vrátí systémový prompt.
    /// </summary>
    public async Task<string> BuildAsync(Datasource datasource, CancellationToken ct)
    {
        var areasTask = _apiClient.GetAreas(datasource, ct);
        var modelsTask = _apiClient.GetDataModels(datasource, ct);
        var scenariosTask = _apiClient.GetScenarios(datasource, ct);
        var featuresTask = _apiClient.GetFeatures(datasource, ct);

        await Task.WhenAll(areasTask, modelsTask, scenariosTask, featuresTask);

        var areas = areasTask.Result.Areas;
        var models = modelsTask.Result.DataModels;
        var scenarios = scenariosTask.Result.Scenarios;
        var features = featuresTask.Result.Features;

        var areaById = areas.ToDictionary(a => a.Id);
        var modelById = models.ToDictionary(m => m.Id);
        var featureById = features.ToDictionary(f => f.Id);

        var sb = new StringBuilder();

        sb.AppendLine("Jsi asistent pro analytiky integrační platformy AVA Integration Modeler.");
        sb.AppendLine("Pomáháš analytikům pochopit a analyzovat doménové modely, integrační scénáře a features.");
        sb.AppendLine("Odpovídej v češtině, pokud se tě uživatel neptá v jiném jazyce.");
        sb.AppendLine($"Datový zdroj: {datasource}.");
        sb.AppendLine();

        AppendAreas(sb, areas);
        AppendDataModels(sb, models, areaById, modelById);
        AppendScenarios(sb, scenarios, featureById);
        AppendFeatures(sb, features);

        return sb.ToString();
    }

    private static void AppendAreas(StringBuilder sb, List<AreaDTO> areas)
    {
        sb.AppendLine("## Oblasti (Areas)");
        if (areas.Count == 0)
        {
            sb.AppendLine("*(žádné oblasti)*");
        }
        else
        {
            foreach (var area in areas.OrderBy(a => a.Code))
                sb.AppendLine($"- **{area.Name}** (kód: `{area.Code}`)");
        }
        sb.AppendLine();
    }

    private static void AppendDataModels(
        StringBuilder sb,
        List<DataModelDTO> models,
        Dictionary<Guid, AreaDTO> areaById,
        Dictionary<Guid, DataModelDTO> modelById)
    {
        sb.AppendLine("## DataModely");
        sb.AppendLine();

        if (models.Count == 0)
        {
            sb.AppendLine("*(žádné datové modely)*");
            sb.AppendLine();
            return;
        }

        foreach (var model in models.OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase))
        {
            var areaCode = model.AreaId.HasValue && areaById.TryGetValue(model.AreaId.Value, out var area)
                ? area.Code
                : "bez oblasti";

            sb.AppendLine($"### {model.Name} (`{model.Code}`)");

            if (!string.IsNullOrWhiteSpace(model.Description))
                sb.AppendLine($"Popis: {model.Description}");

            if (!string.IsNullOrWhiteSpace(model.Notes))
                sb.AppendLine($"Poznámky: {model.Notes}");

            sb.AppendLine($"Oblast: {areaCode} | Agregátní root: {(model.IsAggregateRoot ? "ano" : "ne")}");

            if (model.Fields.Count > 0)
            {
                sb.AppendLine("Pole:");
                foreach (var field in model.Fields.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase))
                {
                    var flags = new List<string>();
                    if (field.IsNullable) flags.Add("nullable");
                    if (field.IsCollection) flags.Add("kolekce");
                    if (field.IsLocalized) flags.Add("lokalizované");
                    if (field.IsPublishedForLookup) flags.Add("lookup");

                    var flagStr = flags.Count > 0 ? $" [{string.Join(", ", flags)}]" : "";
                    var label = !string.IsNullOrWhiteSpace(field.Label) && field.Label != field.Name
                        ? $" / {field.Label}"
                        : "";
                    var desc = !string.IsNullOrWhiteSpace(field.Description)
                        ? $" — {field.Description}"
                        : "";

                    sb.AppendLine($"  - **{field.Name}**{label} ({field.FieldType}){flagStr}{desc}");

                    if (field.ReferencedEntityTypeIds.Count > 0)
                    {
                        var refs = field.ReferencedEntityTypeIds
                            .Select(id => modelById.TryGetValue(id, out var m) ? m.Name : id.ToString())
                            .ToList();
                        sb.AppendLine($"    Reference na: {string.Join(", ", refs)}");
                    }
                }
            }
            else
            {
                sb.AppendLine("Pole: *(žádná)*");
            }

            sb.AppendLine();
        }
    }

    private static void AppendScenarios(
        StringBuilder sb,
        List<ScenarioDTO> scenarios,
        Dictionary<Guid, FeatureDTO> featureById)
    {
        sb.AppendLine("## Integrační scénáře");
        sb.AppendLine();

        if (scenarios.Count == 0)
        {
            sb.AppendLine("*(žádné scénáře)*");
            sb.AppendLine();
            return;
        }

        foreach (var s in scenarios.OrderBy(s => s.Code))
        {
            var name = s.Name?.CzechValue ?? s.Code;
            sb.AppendLine($"### {name} (`{s.Code}`)");

            if (!string.IsNullOrWhiteSpace(s.Description?.CzechValue))
                sb.AppendLine($"Popis: {s.Description.CzechValue}");

            if (s.InputFeatureId.HasValue && featureById.TryGetValue(s.InputFeatureId.Value, out var inFeat))
                sb.AppendLine($"Vstupní feature: {inFeat.Name?.CzechValue ?? inFeat.Code} (`{inFeat.Code}`)");

            if (s.OutputFeatureId.HasValue && featureById.TryGetValue(s.OutputFeatureId.Value, out var outFeat))
                sb.AppendLine($"Výstupní feature: {outFeat.Name?.CzechValue ?? outFeat.Code} (`{outFeat.Code}`)");

            sb.AppendLine();
        }
    }

    private static void AppendFeatures(StringBuilder sb, List<FeatureDTO> features)
    {
        sb.AppendLine("## Integrační features");
        sb.AppendLine();

        if (features.Count == 0)
        {
            sb.AppendLine("*(žádné features)*");
            sb.AppendLine();
            return;
        }

        foreach (var f in features.OrderBy(f => f.Code))
        {
            var name = f.Name?.CzechValue ?? f.Code;
            sb.AppendLine($"### {name} (`{f.Code}`)");

            if (!string.IsNullOrWhiteSpace(f.Description?.CzechValue))
                sb.AppendLine($"Popis: {f.Description.CzechValue}");

            if (f.IncludedModels.Count > 0)
            {
                var modelNames = f.IncludedModels
                    .Select(m => m.DataModel.Name + (m.ReadOnly ? " (read-only)" : ""))
                    .ToList();
                sb.AppendLine($"Modely: {string.Join(", ", modelNames)}");
            }

            if (f.IncludedFeatures.Count > 0)
            {
                var featureRefs = f.IncludedFeatures
                    .Select(fi => fi.Feature.Code + (fi.ConsumeOnly ? " (consume-only)" : ""))
                    .ToList();
                sb.AppendLine($"Zahrnuté features: {string.Join(", ", featureRefs)}");
            }

            sb.AppendLine();
        }
    }
}
