using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Helpers.FilterParams;

public class MessageParams: PaginationParams
{
    public string Username { get; set; }
    public string Container { get; set; } = "Unread";
}
