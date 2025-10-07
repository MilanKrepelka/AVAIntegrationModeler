using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Web.ViewModels.List;

/// <summary>
/// ViewModel pro Feature v Scenario přehledu.
/// </summary>
public class FeatureViewModel
{
  public static FeatureViewModel Empty => new FeatureViewModel
  {
    Id = Guid.Empty,
    Name = LocalizedValue.Empty,
    Description = LocalizedValue.Empty,
    Code = string.Empty
  };

  public Guid Id { get; set; } = Guid.Empty;

  public string Code { get; set; } = string.Empty;
  public LocalizedValue Name { get; set; } = LocalizedValue.Empty;
  public LocalizedValue Description { get; set; } = LocalizedValue.Empty;

}
