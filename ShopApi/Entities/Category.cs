

namespace ShopApi.Entities;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Cactus>? Cacti { get; set; }
}
