using Microsoft.AspNetCore.Http;
using Rehapp.Mobile.Models.Enums;

namespace Rehapp.Mobile.Models;

public class UserRegistrationModel
{
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string AvatarUrl { get; set; }
    public bool Verified { get; set; }
    public bool Blocked { get; set; }
    public bool Deleted { get; set; }
    public City City { get; set; }
    public AccountType Type { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
    public List<IFormFile> Files { get; set; }
}
