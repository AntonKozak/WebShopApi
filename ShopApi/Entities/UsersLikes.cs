using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopApi.Entities;

public class UsersLikes
{
    public UserModel SourceUser { get; set; }
    public int SourceUserId { get; set; }
    public UserModel TargetUser { get; set; }
    public int TargetUserId { get; set; }
}
