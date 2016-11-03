namespace JanHafner.GooKit.Services
{
    using System;
    using JanHafner.GooKit.Services.Properties;
    using JanHafner.Restify.Request;
    using JanHafner.Restify.Services.OAuth2;
    using JanHafner.Restify.Services.OAuth2.AuthorizationContext;
    using JanHafner.Restify.Services.OAuth2.Configuration;
    using JanHafner.Restify.Services.OAuth2.Handler;
    using JanHafner.Restify.Services.OAuth2.Storage;
    using JetBrains.Annotations;
    using Ninject;

    /// <summary>
    /// Adds Google specific tasks to the <see cref="AuthorizationContext"/>.
    /// </summary>
    public sealed class GoogleAuthorizationContext : AuthorizationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleAuthorizationContext"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        /// <param name="authorizationStore">The <see cref="IAuthorizationStore"/>.</param>
        /// <param name="authorizationHandler">The <see cref="IAuthorizationHandler"/>.</param>
        /// <param name="authorizationContextConfiguration">The <see cref="IAuthorizationContextConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="kernel" />', '<paramref name="authorizationHandler" />', '<paramref name="authorizationStore" />' and '<paramref name="authorizationContextConfiguration" />' cannot be null. </exception>
        public GoogleAuthorizationContext([NotNull] IKernel kernel, [NotNull] IAuthorizationStore authorizationStore, [NotNull] IAuthorizationHandler authorizationHandler, [NotNull] IAuthorizationContextConfiguration authorizationContextConfiguration)
            : base(kernel, authorizationStore, authorizationHandler, authorizationContextConfiguration)
        {
        }

        /// <summary>
        /// Does authorize the request by e.g. adding special headers.
        /// </summary>
        /// <param name="request">The authorizable request.</param>
        public override void EnsureAuthorization([CanBeNull] RequestBase request)
        {
            if (request is INeedAuthorization)
            {
                base.EnsureAuthorization(request);
            }
            else if (request is ICanHaveApiKey && String.IsNullOrWhiteSpace(this.AuthorizationContextConfiguration.ApiKey))
            {
                throw new InvalidOperationException(ExceptionMessages.ApiKeyAndOAuth2NotProvided);
            }
        }
    }
}