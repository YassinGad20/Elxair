using Elxair.Models;

public class Favorite
{
    public int Id { get; set; }
    public string UserId { get; set; } // Id المستخدم من السيشين أو Identity
    public int PerfumeId { get; set; }

    // Navigation Properties
    public virtual Perfume Perfume { get; set; }
}