namespace JanHafner.GooKit.Services.ServiceAccount
{
    /// <summary>
    /// The hash algorithm that should be used to create the signature.
    /// </summary>
    public enum JsonWebTokenHashAlgorithm
    {
        /// <summary>
        /// RSA 256.
        /// </summary>
        RS256,

        /// <summary>
        /// HMAC 256.
        /// </summary>
        HS256,

        /// <summary>
        /// HMAC 384.
        /// </summary>
        HS384,

        /// <summary>
        /// HMAC 512.
        /// </summary>
        HS512
    }
}