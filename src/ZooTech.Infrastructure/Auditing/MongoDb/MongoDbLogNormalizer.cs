using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using ZooTech.Application.Common.Gateway.Identity;

namespace ZooTech.Infrastructure.Auditing.MongoDb
{
    public class MongoDbLogNormalizer
    {
        private readonly IJwtService _jwtService;

        private static readonly HashSet<string> SensitiveKeys =
        [
            "password",
            "passwordhash",
            "token",
            "refreshtoken",
            "accesstoken",
            "jwt_id",
            "jti",
            "last_login_at",
            "secret",
            "apikey",
            "authorization",
            "cookie"
        ];
        private int MaxArrayItems;
        private int MaxStringLength;
        private int MaxDepth;

        public MongoDbLogNormalizer(
            IJwtService jwtService,

            IConfiguration config
        )
        {
            _jwtService = jwtService;

            // Asi obtienes un valor de un archivo de configuracion pero especificando el tipo de valor
            // y el valor que se dara por defecto
            MaxArrayItems = config.GetValue<int>("Auditing:MaxArrayItems", 20);
            MaxStringLength = config.GetValue<int>("Auditing:MaxStringLength", 1000);
            MaxDepth = config.GetValue<int>("Auditing:MaxDepth", 3);
        }

        public BsonValue Normalize(
            string key,
            BsonValue value,
            int depth)
        {
            if (IsSensitive(key))
                return "[HIDDEN]";

            return Normalize(value, depth);
        }

        public BsonValue Normalize(
            BsonValue value, 
            int depth = 0
        )
        {
            if (depth >= MaxDepth)
                return new BsonDocument
                {
                    ["_truncated"] = "Maximum depth reached"
                };

            if (value.IsBsonDocument)
            {
                var result = new BsonDocument();

                foreach (var element in value.AsBsonDocument)
                {
                    result[element.Name] = Normalize(
                        element.Name,
                        element.Value,
                        depth + 1);
                }

                return result;
            }

            if (value.IsBsonArray)
            {
                return NormalizeArray(
                    value.AsBsonArray,
                    depth + 1);
            }

            if (value.IsString)
            {
                var text = value.AsString;

                if (_jwtService.IsJwt(text) || LooksLikeToken(text))
                    return "[TOKEN HIDDEN]";

                if (text.Length > MaxStringLength)
                    return text[..MaxStringLength] + "...";

                return value;
            }

            return value;
        }

        private BsonArray NormalizeArray(
            BsonArray source,
            int depth)
        {
            var result = new BsonArray();

            foreach (var item in source.Take(MaxArrayItems))
            {
                result.Add(Normalize(item, depth));
            }

            if (source.Count > MaxArrayItems)
            {
                result.Add(new BsonDocument
                {
                    ["_truncated"] = true,
                    ["_remaining"] = source.Count - MaxArrayItems
                });
            }

            return result;
        }
        private static bool IsSensitive(string key)
        {
            key = key.Replace("_", "")
                    .Replace("-", "")
                    .ToLowerInvariant();

            return SensitiveKeys.Any(key.Contains);
        }

        private static bool LooksLikeToken(string value)
        {
            return value.Length > 80 &&
                value.All(c =>
                    char.IsLetterOrDigit(c) ||
                    c == '.' ||
                    c == '-' ||
                    c == '_' ||
                    c == '=');
        }
    }
}