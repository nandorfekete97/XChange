namespace XChange.Data.Entities;

public class CurrencyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }

    public CurrencyEntity() { }

    public CurrencyEntity(int id, string name, string shortName)
    {
        Id = id;
        Name = name;
        ShortName = shortName;
    }
}