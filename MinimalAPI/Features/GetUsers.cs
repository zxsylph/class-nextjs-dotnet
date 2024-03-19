namespace WebApi.Features;

using WebApi.Models;

class Functions
{
    public static List<User> GetUsers()
    {
        var users = new List<User>
        {
            new User {
                Id = 1,
                Username = "john",
                Email = "john@email.com",
                Fullname = "John Doe"
            },
            new User {
                Id = 1,
                Username = "samit",
                Email = "samit@email.com",
                Fullname = "Samit Koyom"
            },
        };

        return users;
    }
}