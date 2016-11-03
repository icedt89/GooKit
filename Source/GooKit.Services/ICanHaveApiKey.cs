namespace JanHafner.GooKit.Services
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides an Api Key.
    /// </summary>
    public interface ICanHaveApiKey
    {
        /// <summary>
        /// The Api key used for the request.
        /// </summary>
        [CanBeNull]
        String ApiKey { get; set; }
    }
}