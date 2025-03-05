namespace XChange.Data.Entities;

public class CurrencyEntity(string name, string shortName)
{
    public int Id;
    public string Name = name;
    public string ShortName = shortName;
}