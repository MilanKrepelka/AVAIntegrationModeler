using System;
using System.Collections.Generic;
using System.Linq;
using ASOL.DataService.Contracts;

namespace ASOL.DataIntegrationExternalAgent.API.Options
{
    /// <summary>
    /// Represents data-service querying options.
    /// </summary>
    public class DataServiceQueryOptions
    {
        /// <summary>
        /// AgentCode of ASOL.DataIntegrationExternalAgent
        /// DON'T USE THIS CONSTANT IN YOUR PROJECT! Create your unique identifier like '«organization».«solution»'.
        /// </summary>
        public string AgentCode { get; set; } = "ASOL.DataIntegrationExternalAgent"; //TODO - unique well-known agent-code or application name

        /// <summary>
        /// The integration-agent identifier.
        /// </summary>
        public IntegrationAgentId AgentId { get; set; } = new(); //alt: new IntegrationAgentId { SourceId = new Guid("use-your-existing-data-source-id-to-keep-compatibility") };

        #region Identifiers of unified virtual models published or subscribed in data-service 

        /// <summary>
        /// ExampleBook modelId
        /// </summary>
        public Guid ExampleBookModelId { get; set; }

        /// <summary>
        /// ExampleAuthor modelId
        /// </summary>
        public Guid ExampleAuthorModelId { get; set; }

        /// <summary>
        /// ExampleGenre modelId
        /// </summary>
        public Guid ExampleGenreModelId { get; set; }

        // ... more modelIds here ...

        /// <summary>
        /// Get subscribed data service model identifiers - is used in handlers for event subscription check
        /// </summary>
        public IEnumerable<Guid> GetSubscribedModelIds()
        {
            return new[]
            {
                ExampleBookModelId,
                ExampleAuthorModelId,
                // ... more modelIds here ...

            }.Where(m => m != Guid.Empty);
        }

        /// <summary>
        /// Is data service model subscribed - is used in handlers for event subscription check
        /// </summary>
        public bool IsSubscribedModel(Guid modelId)
        {
            return GetSubscribedModelIds().Any(m => m == modelId);
        }

        #endregion
    }
}
