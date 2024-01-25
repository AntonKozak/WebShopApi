
using System.Text.Json;
using ShopApi.Helpers;

namespace ShopApi.Extensions;

public static class HttpExtensions
{
    // extension method for adding pagination info to response header
    // extension method to extend HttpResponse class
    public static void AddPaginationHeader(this HttpResponse response, PaginationHeader paginationHeader)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}
