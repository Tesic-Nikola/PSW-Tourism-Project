using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class Interest : Entity
{
    public string Name { get; init; }

    public Interest(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Interest Name");
        Name = name;
    }

    // Private constructor for EF
    private Interest() { }
}