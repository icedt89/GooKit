namespace JanHafner.GooKit.Services.ServiceAccount
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using JanHafner.GooKit.Services.Properties;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Creates Json Web Tokens. Customized by Jan Hafner to handle Google Service Account authorization.
    /// All respect goes to John Sheehan for the original implementation: https://github.com/johnsheehan/jwt
    /// </summary>
    public static class JsonWebToken
    {
        [NotNull]
        private static readonly Dictionary<JsonWebTokenHashAlgorithm, Func<Byte[], Byte[], Byte[]>> HashAlgorithms;

        [NotNull]
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
                                                                                {
                                                                                    ContractResolver = new DefaultContractResolver()
                                                                                };

        static JsonWebToken()
        {
            HashAlgorithms = new Dictionary<JsonWebTokenHashAlgorithm, Func<Byte[], Byte[], Byte[]>>
                             {
                                 {
                                     JsonWebTokenHashAlgorithm.HS256, (key, value) =>
                                                             {
                                                                 using (var sha = new HMACSHA256(key))
                                                                 {
                                                                     return sha.ComputeHash(value);
                                                                 }
                                                             }
                                 },
                                 {
                                     JsonWebTokenHashAlgorithm.HS384, (key, value) =>
                                                             {
                                                                 using (var sha = new HMACSHA384(key))
                                                                 {
                                                                     return sha.ComputeHash(value);
                                                                 }
                                                             }
                                 },
                                 {
                                     JsonWebTokenHashAlgorithm.HS512, (key, value) =>
                                                             {
                                                                 using (var sha = new HMACSHA512(key))
                                                                 {
                                                                     return sha.ComputeHash(value);
                                                                 }
                                                             }
                                 },
                             };
        }

        /// <summary>
        /// Encodes the supplied object as a JWT.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="keyBytes">The key.</param>
        /// <param name="algorithm">The algorithm to use.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="payload"/>' and '<paramref name="keyBytes"/>' cannot be null. </exception>
        [NotNull]
        public static String Encode([NotNull] Object payload, [NotNull] Byte[] keyBytes, JsonWebTokenHashAlgorithm algorithm)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (keyBytes == null || keyBytes.Length == 0)
            {
                throw new ArgumentNullException(nameof(keyBytes));
            }

            var segments = new Collection<String>();
            var header = new { typ = "JWT", alg = algorithm.ToString() };

            var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None, JsonSerializerSettings));
            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None, JsonSerializerSettings));

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            var stringToSign = String.Join(".", segments.ToArray());

            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            var signature = HashAlgorithms[algorithm](keyBytes, bytesToSign);
            segments.Add(Base64UrlEncode(signature));


            return String.Join(".", segments.ToArray());
        }

        /// <summary>
        /// Encodes the supplied object as a JWT.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="certificate">The <see cref="X509Certificate2"/>.</param>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="payload"/>' and '<paramref name="certificate"/>' cannot be null. </exception>
        [NotNull]
        public static String Encode([NotNull] Object payload, [NotNull] X509Certificate2 certificate)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            var segments = new Collection<String>();
            var header = new { typ = "JWT", alg = JsonWebTokenHashAlgorithm.RS256.ToString() };

            var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header, Formatting.None, JsonSerializerSettings));
            var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, Formatting.None, JsonSerializerSettings));

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            var stringToSign = String.Join(".", segments.ToArray());

            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);

            var templateCryptoServiceProvider = (RSACryptoServiceProvider)certificate.PrivateKey;
            Byte[] signatureBytes;
            using (var cryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters
                                                                            {
                                                                                KeyContainerName = templateCryptoServiceProvider.CspKeyContainerInfo.KeyContainerName, 
                                                                                KeyNumber = templateCryptoServiceProvider.CspKeyContainerInfo.KeyNumber == KeyNumber.Exchange ? 1 : 2
                                                                            })
                                               {
                                                   PersistKeyInCsp = false
                                               })
            {
                signatureBytes = cryptoServiceProvider.SignData(bytesToSign, "SHA256");
            }

            segments.Add(Base64UrlEncode(signatureBytes));

            return String.Join(".", segments.ToArray());
        }

        /// <summary>
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <returns>The decoded JWT.</returns>
        [NotNull]
        public static String Decode([NotNull] String token, [NotNull] String key)
        {
            return Decode(token, key, true);
        }

        /// <summary>
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <param name="verify"></param>
        /// <returns>The decoded JWT.</returns>
        /// <exception cref="ArgumentNullException">The value of '<paramref name="token"/>' and '<paramref name="key"/>' cannot be null. </exception>
        /// <exception cref="InvalidOperationException">The expected signature does not match the computed signature. </exception>
        [NotNull]
        public static String Decode([NotNull] String token, [NotNull] String key, Boolean verify)
        {
            if (String.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(token));
            }

            var parts = token.Split('.');
            var header = parts[0];
            var payload = parts[1];
            var crypto = Base64UrlDecode(parts[2]);

            var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
            var headerData = JObject.Parse(headerJson);
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            var payloadData = JObject.Parse(payloadJson);

            if (verify)
            {
                var bytesToSign = Encoding.UTF8.GetBytes(String.Concat(header, ".", payload));
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var algorithm = (String)headerData["alg"];

                var signature = HashAlgorithms[GetHashAlgorithm(algorithm)](keyBytes, bytesToSign);
                var decodedCrypto = Convert.ToBase64String(crypto);
                var decodedSignature = Convert.ToBase64String(signature);

                if (decodedCrypto != decodedSignature)
                {
                    throw new InvalidOperationException(String.Format(ExceptionMessages.JWTInvalidSignatureExceptionMessage, decodedSignature, decodedCrypto));
                }
            }

            return payloadData.ToString();
        }

        /// <exception cref="ArgumentNullException">The value of '<paramref name="algorithm"/>' cannot be null. </exception>
        /// <exception cref="ArgumentException">Algorithm is not supported.</exception>
        private static JsonWebTokenHashAlgorithm GetHashAlgorithm([NotNull] String algorithm)
        {
            if (String.IsNullOrEmpty(algorithm))
            {
                throw new ArgumentNullException(nameof(algorithm));
            }

            switch (algorithm)
            {
                case "RS256":
                    return JsonWebTokenHashAlgorithm.RS256;
                case "HS384":
                    return JsonWebTokenHashAlgorithm.HS384;
                case "HS512":
                    return JsonWebTokenHashAlgorithm.HS512;
                case "HS256":
                    return JsonWebTokenHashAlgorithm.HS256;
                default:
                    throw new ArgumentException(String.Format(ExceptionMessages.JWTAlgorithmNotSupportedExceptionMessage, algorithm));
            }
        }

        /// <exception cref="ArgumentNullException">The value of '<paramref name="input"/>' cannot be null. </exception>
        [NotNull]
        private static String Base64UrlEncode([NotNull] Byte[] input)
        {
            if (input == null || input.Length == 0)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding
            return output;
        }

        /// <exception cref="ArgumentNullException">The value of '<paramref name="input"/>' cannot be null.</exception>
        /// <exception cref="InvalidOperationException">The resulting Base64 string was detected as illegal.</exception>
        [NotNull]
        private static Byte[] Base64UrlDecode([NotNull] String input)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0:
                    break; // No pad chars in this case
                case 2:
                    output += "==";
                    break; // Two pad chars
                case 3:
                    output += "=";
                    break; // One pad char
                default:
                    throw new InvalidOperationException(String.Format(ExceptionMessages.JWTIllegalBase64StringExceptionMessage, output));
            }

            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
    }
}