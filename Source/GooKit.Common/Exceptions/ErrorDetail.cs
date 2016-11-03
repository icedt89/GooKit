namespace JanHafner.GooKit.Common.Exceptions
{
    using System;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides more detailed information about the occured error.
    /// </summary>
    [Serializable]
    public sealed class ErrorDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDetail"/> class.
        /// </summary>
        /// <param name="domain">The problem domain.</param>
        /// <param name="reason">The reason for the error.</param>
        /// <param name="message">An additional message.</param>
        /// <param name="locationType">The type of the location.</param>
        /// <param name="location">The location.</param>
        public ErrorDetail([CanBeNull] String domain, [CanBeNull] String reason, [CanBeNull] String message,
            [CanBeNull] String locationType, [CanBeNull] String location)
        {
            this.Reason = reason;
            this.Domain = domain;
            this.Message = message;
            this.LocationType = locationType;
            this.Location = location;
        }

        /// <summary>
        /// The problem domain in which the error is occured.
        /// </summary>
        [CanBeNull, JsonProperty("domain")]
        public String Domain { get; private set; }

        /// <summary>
        /// The reason for the error.
        /// </summary>
        [CanBeNull, JsonProperty("reason")]
        public String Reason { get; private set; }

        /// <summary>
        /// An additional error message.
        /// </summary>
        [CanBeNull, JsonProperty("message")]
        public String Message { get; private set; }

        /// <summary>
        /// The type of the location of the error.
        /// </summary>
        [CanBeNull, JsonProperty("locationType")]
        public String LocationType { get; private set; }

        /// <summary>
        /// The location of the error-
        /// </summary>
        [CanBeNull, JsonProperty("location")]
        public String Location { get; private set; }
    }
}