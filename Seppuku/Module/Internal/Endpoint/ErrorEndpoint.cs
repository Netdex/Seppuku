using Nancy;
using Nancy.ErrorHandling;

namespace Seppuku.Module.Internal.Endpoint
{
    public class ErrorEndpoint : IStatusCodeHandler
    {

        public ErrorEndpoint()
        {
            
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var response = (Response) "404 Endpoint not found";
            response.StatusCode = statusCode;
            context.Response = response;
        }
    }
}
