namespace JanHafner.GooKit.Services.X509
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using JanHafner.Restify.Services.OAuth2.Configuration;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides methods for creating a <see cref="X509Certificate2"/>.
    /// </summary>
    public sealed class X509CertificateFactory : IX509Certificate2Factory
    {
        [NotNull]
        private readonly IAuthorizationConfiguration authorizationConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="X509CertificateFactory"/>.
        /// </summary>
        /// <param name="authorizationConfiguration">The <see cref="IAuthorizationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="authorizationConfiguration"/>' cannot be null. </exception>
        public X509CertificateFactory(IAuthorizationConfiguration authorizationConfiguration)
        {
            if (authorizationConfiguration == null)
            {
                throw new ArgumentNullException(nameof(authorizationConfiguration));
            }

            this.authorizationConfiguration = authorizationConfiguration;
        }

        /// <summary>
        /// Creates the certificate.
        /// </summary>
        /// <returns>The <see cref="X509Certificate2"/> certificate.</returns>
        public X509Certificate2 CreateCertificate()
        {
            if (String.IsNullOrWhiteSpace(this.authorizationConfiguration.X509Certificate.FilePath))
            {
                return new X509Certificate2(
                    this.authorizationConfiguration.X509Certificate.CertificateContent,
                    this.authorizationConfiguration.ClientSecret);
            }

            return new X509Certificate2(
                this.authorizationConfiguration.X509Certificate.FilePath,
                this.authorizationConfiguration.ClientSecret);
        }
    }
}