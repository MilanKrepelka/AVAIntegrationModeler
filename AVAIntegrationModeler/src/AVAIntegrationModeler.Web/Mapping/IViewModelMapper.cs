namespace AVAIntegrationModeler.Web.Mapping;

public interface IViewModelMapper<DTO, ViewModel, TSelf>
    where TSelf : IViewModelMapper<DTO, ViewModel, TSelf>
{
  /// <summary>
  /// Namapuje datový přenosový objekt (DTO) na ViewModel.
  /// </summary>
  /// <param name="sourceDto">Zdrojový DTO</param>
  /// <returns>Výsledný <typeparamref name="ViewModel"/></returns>
  static abstract void MapToViewModel(DTO sourceDto, out ViewModel result);
}
