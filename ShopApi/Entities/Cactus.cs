
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Entities;

public class Cactus
{

    public int Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string CareInstructions { get; set; }

    public int? CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; }
    public ICollection<CactusPhoto> CactusPhotos { get; set; }
}
