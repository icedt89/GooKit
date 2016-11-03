namespace JanHafner.GooKit.Services
{
    using System.Net.Http;
    using JanHafner.GooKit.Services.ServiceAccount;
    using JanHafner.GooKit.Services.X509;
    using JanHafner.Restify;
    using JanHafner.Restify.Caching;
    using JanHafner.Restify.Services.OAuth2.AuthorizationContext;
    using JanHafner.Restify.Services.OAuth2.Handler;
    using Ninject.Modules;

    /// <summary>
    /// Registers dependencies residing in this assembly.
    /// </summary>
    public sealed class GoogleServiceInitialization : NinjectModule
    {
        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            this.Bind<IX509Certificate2Factory>().To<X509CertificateFactory>().InSingletonScope();
            this.Rebind<IAuthorizationContext>().To<GoogleAuthorizationContext>();
            this.Rebind<IAuthorizationHandler>().To<ServiceAccountAuthorizationHandler>();
            this.Rebind<HttpClient>().To<CachingAwareHttpClient>();
            this.Rebind<IRequestProcessor>().To<GoogleRequestProcessor>();
        }
    }
}