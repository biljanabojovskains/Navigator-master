using PublicNavigator.Models.Concrete;


namespace PublicNavigator.Models.Abstract
{
    public interface IUser
    {
        int UserId { get; set; }
        string UserName { get; set; }
        string FullName { get; set; }
        string Phone { get; set; }
        string Email { get; set; }
    }
}