namespace JanHafner.GooKit.Services
{
    using System;
    using System.Net.Http;
    using JanHafner.GooKit.Common.Exceptions;
    using JanHafner.Restify;
    using JanHafner.Restify.Header;
    using JanHafner.Restify.Request;
    using JanHafner.Restify.Services.OAuth2.Configuration;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides additional functionality for signing the REST-Request with the OAuth2 token and handles Google specific stuff.
    /// </summary>
    internal sealed class GoogleRequestProcessor : RequestProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleRequestProcessor"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/>.</param>
        /// <param name="authorizationContextConfiguration">The <see cref="IAuthorizationContextConfiguration"/>.</param>
        /// <param name="requestMessageFactory">The <see cref="IHttpRequestMessageFactory"/> that creates <see cref="HttpRequestMessage"/> instances from an <see cref="object"/>.</param>
        /// <param name="responseHeaderMapper">The <see cref="IResponseHeaderMapper"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="authorizationContextConfiguration"/>', '<paramref name="httpClient"/>', '<paramref name="requestMessageFactory"/>' and '<paramref name="responseHeaderMapper"/>' cannot be null. </exception>
        public GoogleRequestProcessor([NotNull] HttpClient httpClient, [NotNull] IAuthorizationContextConfiguration authorizationContextConfiguration, [NotNull] IHttpRequestMessageFactory requestMessageFactory, [NotNull] IResponseHeaderMapper responseHeaderMapper)
            : base(httpClient, requestMessageFactory, responseHeaderMapper)
        {
            if (authorizationContextConfiguration == null)
            {
                throw new ArgumentNullException(nameof(authorizationContextConfiguration));
            }

            this.HttpClient.BaseAddress = authorizationContextConfiguration.BaseUrl;
        }

        /// <summary>
        /// Throws a more concrete exception if the <see cref="HttpResponseMessage"/> does not contain useful detail.
        /// </summary>
        /// <param name="restResponse">The executed <see cref="HttpResponseMessage"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="restResponse"/>' cannot be null. </exception>
        /// <exception cref="GoogleApiException">The <see cref="GoogleApiException"/> if it could be created.</exception>
        protected override void ThrowException(HttpResponseMessage restResponse)
        {
            if (restResponse == null)
            {
                throw new ArgumentNullException(nameof(restResponse));
            }

            GoogleApiException.TryThrow(restResponse.Content.ReadAsStringAsync().Result);

            base.ThrowException(restResponse);
        }
    }
}