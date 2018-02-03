using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;
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
            return response;
        }
    }
}