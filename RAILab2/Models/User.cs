using Microsoft.Extensions.Primitives;

namespace RAILab2.Models
{
    public class User(string login)
    {
        public string Login { get; set; } = login;
        public DateTime DateTime { get; set; } = DateTime.Now;
        public List<User> Friends { get; set; } = [];
    }
}
