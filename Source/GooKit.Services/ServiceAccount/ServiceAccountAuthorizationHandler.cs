namespace JanHafner.GooKit.Services.ServiceAccount
{
    #region Usings

    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using JanHafner.GooKit.Services.X509;
    using JanHafner.Queryfy;
    using JanHafner.Restify.Services.OAuth2;
    using JanHafner.Restify.Services.OAuth2.Configuration;
    using JanHafner.Restify.Services.OAuth2.Handler;
    using JanHafner.Toolkit.Common.ExtensionMethods;

    #endregion

    /// <summary>
    /// Handles the authorization using service account with JWT.
    /// </summary>
    public sealed class ServiceAccountAuthorizationHandler : AuthorizationHandler
    {
        #region Fields

        private readonly IX509Certificate2Factory x509CertificateFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceAccountAuthorizationHandler"/> class.
        /// </summary>
        /// <param name="x509CertificateFactory">An implementation of the <see cref="IX509Certificate2Factory"/>.</param>
        /// <param name="authorizationContextConfiguration">An instance of the <see cref="IAuthorizationContextConfiguration"/> which contains information about the endpoints.</param>
        /// <param name="queryfyDotNet">The <see cref="IQueryfyDotNet"/>.</param>
        /// <param name="httpClient">The <see cref="HttpClient"/>.</param>
        public ServiceAccountAuthorizationHandler(
            IX509Certificate2Factory x509CertificateFactory,
            IAuthorizationContextConfiguration authorizationContextConfiguration, 
            IQueryfyDotNet queryfyDotNet, 
            HttpClient httpClient)
            : base(authorizationContextConfiguration, queryfyDotNet, httpClient)
        {
            this.x509CertificateFactory = x509CertificateFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Begins the authorization process.
        /// </summary>
        public override AuthorizationState StartAuthorization()
        {
            var issueTime = DateTime.UtcNow;
            var iat = (Int32)issueTime.Subtract(ValueTypeExtensions.UnixTimestampOrigin).TotalSeconds;
            var exp = (Int32)issueTime.AddMinutes(55).Subtract(ValueTypeExtensions.UnixTimestampOrigin).TotalSeconds;

            var payload = new
            {
                iss = this.AuthorizationContextConfiguration.Authorization.ClientId,
                scope = String.Join(" ", this.AuthorizationContextConfiguration.Authorization.Scopes.GetGrantedScopes()),
                aud = this.AuthorizationContextConfiguration.Authorization.TokenEndPoint,
                iat,
                exp,
            };

            String jsonWebToken;
            using (var certificate = this.x509CertificateFactory.CreateCertificate().ToDisposable(cert => cert.Reset()))
            {
                jsonWebToken = JsonWebToken.Encode(payload, certificate.Indisposable);
            }

            var query = this.QueryfyDotNet.Queryfy(new
                                                 {
                                                     grant_type = Uri.EscapeDataString("urn:ietf:params:oauth:grant-type:jwt-bearer"),
                                                     assertion = jsonWebToken
                                                 });
            using (var tokenRequest = new HttpRequestMessage(HttpMethod.Post, this.AuthorizationContextConfiguration.Authorization.TokenEndPoint)
            {
                Content = new StringContent(query.QueryString, Encoding.UTF8, "application/x-www-form-urlencoded")
            })
            {
                return tokenRequest.GetAuthorizationState(this.HttpClient, issueTime, this.AuthorizationContextConfiguration.Authorization.Scopes.GetGrantedScopes().ToList());
            }
        }

        #endregion
    }
}