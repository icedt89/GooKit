namespace JanHafner.GooKit.Services.X509
{
    using System.Security.Cryptography.X509Certificates;
    using JetBrains.Annotations;

    /// <summary>
    /// Provides methods for creating a <see cref="X509Certificate2"/>.
    /// </summary>
    public interface IX509Certificate2Factory
    {
        /// <summary>
        /// Creates the certificate.
        /// </summary>
        /// <returns>The <see cref="X509Certificate2"/> certificate.</returns>
        [NotNull]
        X509Certificate2 CreateCertificate();
    }
}