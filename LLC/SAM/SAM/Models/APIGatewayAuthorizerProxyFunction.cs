using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace SAM.Models
{
    public abstract class APIGatewayAuthorizerProxyFunction : APIGatewayProxyFunction
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayAuthorizerProxyRequest request, ILambdaContext lambdaContext)
        {
            Console.WriteLine(JsonConvert.SerializeObject(request));
            lambdaContext.Logger.LogLine($"Incoming {request.HttpMethod} requests to {request.Path}");

            var features = new InvokeFeatures();
            MarshallRequest(features, request);

            var context = CreateContext(features);
            if (request.Headers.ContainsKey("Authorization"))
            {
                var token = new JwtSecurityToken(request.Headers["Authorization"]);
                var identity = new ClaimsIdentity(token.Claims, "AuthorizerIdentity");
                context.HttpContext.User = new ClaimsPrincipal(identity);
            }

            // Add along the Lambda objects to the HttpContext to give access to Lambda to them in the ASP.NET Core application
            context.HttpContext.Items["LAMBDA_CONTEXT"] = lambdaContext;
            context.HttpContext.Items["APIGATEWAY_REQUEST"] = request;

            var response = await ProcessRequest(lambdaContext, context, features);
            var useGzip = false;
            if (request.Headers != null && request.Headers.TryGetValue("accept-encoding", out var headerValue))
            {
                useGzip = headerValue.Contains("gzip");
            }

            if (useGzip && context.HttpContext.Request.Method != HttpMethod.Options.Method)
            {
                var buffer = Zip(response.Body);
                response.Body = Convert.ToBase64String(buffer);
                response.Headers.Add(HeaderNames.ContentEncoding, "gzip");
            }

            return response;
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

    }
}