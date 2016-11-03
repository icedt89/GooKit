namespace JanHafner.GooKit.Services
{
    using System;
    using JanHafner.Restify.Services;
    using JanHafner.Restify.Services.OAuth2.Configuration;
    using JanHafner.Restify.Services.RequestExecutionStrategy;
    using JetBrains.Annotations;

    /// <summary>
    /// Base service for all Google services.
    /// </summary>
    public abstract class GoogleServiceBase : RestService, IGoogleService
    {
        /// <summary>
        /// The <see cref="IAuthorizationContextConfiguration"/>.
        /// </summary>
        [NotNull]
        protected readonly IAuthorizationContextConfiguration AuthorizationContextConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleServiceBase"/> class.
        /// </summary>
        /// <param name="requestExecutionStrategy">The <see cref="IRequestExecutionStrategy"/>.</param>
        /// <param name="authorizationContextConfiguration">The <see cref="IAuthorizationContextConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="authorizationContextConfiguration"/>' and '<paramref name="requestExecutionStrategy"/>' cannot be null. </exception>
        public GoogleServiceBase([NotNull] IRequestExecutionStrategy requestExecutionStrategy,
            [NotNull] IAuthorizationContextConfiguration authorizationContextConfiguration)
            : base(requestExecutionStrategy)
        {
            if (authorizationContextConfiguration == null)
            {
                throw new ArgumentNullException(nameof(authorizationContextConfiguration));
            }

            this.AuthorizationContextConfiguration = authorizationContextConfiguration;
        }

        /// <summary>
        /// Is called bevor the request is send, so extra behavior can be applied on service level.
        /// Applies the configured api-key to all requests which need an api-key.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="request">The request.</param>
        /// <returns>The request that is send.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="request"/>' cannot be null. </exception>
        protected override TRequest PreProcessRequest<TRequest>(TRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var iCanHaveApiKey = request as ICanHaveApiKey;
            if (iCanHaveApiKey != null)
            {
                iCanHaveApiKey.ApiKey = this.AuthorizationContextConfiguration.ApiKey;
            }

            return base.PreProcessRequest(request);
        }
    }
}