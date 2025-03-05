namespace XChange.Models;

public class UserModel(int id, string name, List<UserFundModel> funds)
{
    public int Id = id;
    public string Name = name;
    public List<UserFundModel> Funds = funds;
}