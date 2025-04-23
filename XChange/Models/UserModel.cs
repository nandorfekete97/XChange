namespace XChange.Models;

public record UserModel(
    int Id, 
    string FirstName, 
    string LastName,
    List<UserFundModel> Funds);
