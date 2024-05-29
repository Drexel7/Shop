/**************************************************************************
 *                                                                        *
 *  File:        User.cs                                                  *
 *  Copyright:   (c) 2024, Maftei Gutui Robert, Branici Radu              *
 *                                                                        *
 *  E-mail:      robert-mihaita.maftei-gutui@student.tuiasi.ro,           *
 *               radu.branici@student.tuiasi.ro                           *
 *  Description: Book Store Online Web Application                        *  
 *               Defines the User class with additional properties.       *
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/


using Microsoft.AspNetCore.Identity;
// nu mai functioneaza
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
