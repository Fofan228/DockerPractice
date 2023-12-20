using System.Net;

namespace Rabbit.HttpService;

public interface IHttpService
{
    Task<string> GetStatusCodeAsync(string url);
}