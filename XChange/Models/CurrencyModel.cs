namespace XChange.Models;

public class CurrencyModel(int id, string name, string shortName)
{
    public int Id = id;
    public string Name = name;
    public string ShortName = shortName;
}