using Microsoft.AspNetCore.Identity;
//Nu mai functioneaza
namespace Shop.Models
{
    public class User :IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; }= "";

        public string Address { get; set; } = "";
        
        public DateTime CreatedAt { get; set; }
    }
}
