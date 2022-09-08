using FINN.CORE.Models;

namespace FINN.COST.Models;

public class Formula : BaseEntity
{
    public string Type { get; set; } = string.Empty;
    public string Expression { get; set; } = string.Empty;
}