using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Seppuku.Module.Standard
{
    public enum StatusCode
    {
        Unauthorized,
        Success,
        Expired,
        Pending
    }
}
