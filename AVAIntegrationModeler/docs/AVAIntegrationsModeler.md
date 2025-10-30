# AVA Integration Modeler

# Základní popis

## UI - Pøehled

### Scenarios

| Kód | Název - anglicky | Popis anglicky | Vstupní Feature| Výstupní feature | Identifikátor|
|---------|-------------|--------|||||
| ScenarioCode| PersonConsumer| Description|OutputFeatureToScenario3Code|OutputFeatureToScenario3Code|28364ca9-a73a-4669-8645-fa8c9933fc80

#### Co potøebuje navíc
- FeatureSummaryDTO, pro zobrazení 

### Feaures

| Kód | Název - anglicky | Popis anglicky | Included features| Included models | Identifikátor|
|---------|-------------|--------|||||
| BankingClient| Bankink client| Description of banking klient|Activities, BankingProvider |Organization, Person, Organization Unit, |28364ca9-a73a-4669-8645-fa8c9933fc80
| ContactsPersons |Contact persons| Synchronization of contact persons| Organizations, Person |PersonRelation, PersonRelationType | 97371f43-9d3b-41d6-bf08-79aa3db14ed6

#### Co potøebuje navíc
- IncludedFeatureDTO(FeatureSummaryDTO), pro zobrazení Included features
- IncludedModelDTO(DataModelSummaryDTO), pro zobrazení Included models

- 
## API

### Endpoints

### Gets

| Název pøehledu | Úèel | S Parameterm include ||
|---------|-------------|--------||
| Area | Základní pøehled v UI | NE ||
| Scenarios | Základní pøehled v UI | Active ||
| Feature | Description 1 | Active ||
| Models | Description 2 | Pending ||
| Feature 3 | Description 3 | Completed ||



