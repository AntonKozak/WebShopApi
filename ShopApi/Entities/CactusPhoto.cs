
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Entities;

public class CactusPhoto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public string PublicId { get; set; }
    public int CactusId { get; set; }
    [ForeignKey("CactusId")]
    public Cactus Cactus { get; set; }
}
