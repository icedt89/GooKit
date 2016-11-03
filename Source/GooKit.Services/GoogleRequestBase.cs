namespace JanHafner.GooKit.Services
{
    using System;
    using JanHafner.Queryfy.Attributes;
    using JanHafner.Restify.Json;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    /// Base request for all Google API based requests.
    /// </summary>
    [UseOnlyAttributes]
    public abstract class GoogleRequestBase : JsonRequest, ICanHaveApiKey
    {
        /// <summary>
        /// The Api key used for the request.
        /// </summary>
        [JsonIgnore]
        [QueryParameter("key")]
        public String ApiKey { get; set; }

        /// <summary>
        /// Only fields in this expression will be returned be the server in the response.
        /// </summary>
        [JsonIgnore]
        [QueryParameter("fields")]
        [CanBeNull]
        public String Fields { get; set; }
    }
}