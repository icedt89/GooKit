namespace JanHafner.GooKit.Services
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines a page able list received from a google api.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ItemList<T> : GoogleSchemaEntity
        where T : GoogleSchemaEntity
    {
        /// <summary>
        /// The token to the next page with results.
        /// </summary>
        [JsonProperty("nextPageToken")]
        [CanBeNull]
        public String NextPageToken { get; set; }

        /// <summary>
        /// The items.
        /// </summary>
        [JsonProperty("items")]
        [CanBeNull]
        public IEnumerable<T> Items { get; set; }
    }
}