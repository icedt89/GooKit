namespace JanHafner.GooKit.Services
{
    using System;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    /// <summary>
    /// The base class for all by Google provided "resources".
    /// </summary>
    public abstract class GoogleSchemaEntity
    {
        /// <summary>
        /// The kind of the resource.
        /// </summary>
        [NotNull]
        public abstract String Kind { get; }

        /// <summary>
        /// The E-Tag of the resource.
        /// </summary>
        [JsonProperty("etag")]
        [CanBeNull]
        public String ETag { get; set; }

        /// <summary>
        /// Bestimmt, ob das angegebene <see cref="T:System.Object"/> und das aktuelle <see cref="T:System.Object"/> gleich sind.
        /// </summary>
        /// <returns>
        /// true, wenn das angegebene <see cref="T:System.Object"/> gleich dem aktuellen <see cref="T:System.Object"/> ist, andernfalls false.
        /// </returns>
        /// <param name="compareMe">Das <see cref="T:System.Object"/>, das mit dem aktuellen <see cref="T:System.Object"/> verglichen werden soll. </param><filterpriority>2</filterpriority>
        public override Boolean Equals([CanBeNull] Object compareMe)
        {
            var comparingGoogleResource = compareMe as GoogleSchemaEntity;
            if (comparingGoogleResource != null && !String.IsNullOrWhiteSpace(this.ETag) && !String.IsNullOrWhiteSpace(comparingGoogleResource.ETag))
            {
                return this.ETag == comparingGoogleResource.ETag;
            }

            return base.Equals(compareMe);
        }

        /// <summary>
        /// Fungiert als Hashfunktion für einen bestimmten Typ. 
        /// </summary>
        /// <returns>
        /// Ein Hashcode für das aktuelle <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override Int32 GetHashCode()
        {
            if (!String.IsNullOrWhiteSpace(this.ETag))
            {
                return this.ETag.GetHashCode();
            }

            return base.GetHashCode();
        }
    }
}