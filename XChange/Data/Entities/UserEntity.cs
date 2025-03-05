namespace XChange.Data.Entities;

public class UserEntity(string name)
{
    public int Id;
    public string Name = name;
}