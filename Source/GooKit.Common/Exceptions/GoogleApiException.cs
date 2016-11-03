namespace JanHafner.GooKit.Common.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Base class for exceptions responded from a Google Service.
    /// </summary>
    [Serializable]
    public sealed class GoogleApiException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleApiException"/> class.
        /// </summary>
        /// <param name="errors">Detail about the occured error.</param>
        /// <param name="message">The error message from the server.</param>
        /// <param name="errorCode">The HTTP-Error code.</param>
        public GoogleApiException([NotNull] IEnumerable<ErrorDetail> errors, [CanBeNull] String message, Int32 errorCode)
            : base(message)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            this.Errors = errors;
            this.HttpErrorCode = errorCode;
        }

        /// <inheritdoc />
        protected GoogleApiException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }

        /// <summary>
        /// Detailed information about the occured error.
        /// </summary>
        [NotNull]
        public IEnumerable<ErrorDetail> Errors { get; private set; }

        /// <summary>
        /// The HTTP-Error code.
        /// </summary>
        public Int32 HttpErrorCode { get; private set; }

        /// <summary>
        /// Tries to parse the supplied json response to a <see cref="GoogleApiException"/>.
        /// </summary>
        /// <param name="responseBody">The response body.</param>
        /// <param name="exception">The parsed <see cref="GoogleApiException"/>.</param>
        /// <returns>A value indicating if the parse was successfull.</returns>
        public static Boolean TryCreate([CanBeNull] String responseBody, [CanBeNull] out GoogleApiException exception)
        {
            if (String.IsNullOrEmpty(responseBody))
            {
                exception = null;
                return false;
            }

            var errorRootObject = JObject.Parse(responseBody);

            JToken jsonError;
            if (!errorRootObject.TryGetValue("error", out jsonError))
            {
                exception = null;
                return false;
            }

            var anonymousContainer = new
                                     {
                                         errors = Enumerable.Empty<ErrorDetail>(),
                                         message = String.Empty,
                                         code = -1
                                     };
            anonymousContainer = JsonConvert.DeserializeAnonymousType(jsonError.ToString(), anonymousContainer);
            exception = new GoogleApiException(anonymousContainer.errors, anonymousContainer.message, anonymousContainer.code);
            return true;
        }

        /// <summary>
        /// Tries to parse the response body to a <see cref="GoogleApiException"/> and if successfull throws it.
        /// </summary>
        /// <param name="responseBody">The response body.</param>
        /// <exception cref="GoogleApiException">The <see cref="GoogleApiException"/> if it could be created.</exception>
        public static void TryThrow([CanBeNull] String responseBody)
        {
            GoogleApiException googleApiException;
            if (GoogleApiException.TryCreate(responseBody, out googleApiException) && googleApiException != null)
            {
                throw googleApiException;
            }
        }
    }
}