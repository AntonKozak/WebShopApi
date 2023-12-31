
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Entities;
public class UsersPhoto
{
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserModel User { get; set; }
    }
