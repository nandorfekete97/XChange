namespace XChange.Data.Entities;

public class UserEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public UserEntity()
    {
    }

    public UserEntity(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
}