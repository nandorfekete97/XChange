namespace XChange.Models;

public record UserModel(
    int Id, 
    string Name, 
    List<UserFundModel> Funds);
