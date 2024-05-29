/**************************************************************************
 *                                                                        *
 *  File:        User.cshtml.cs                                               *
 *  Copyright:   (c) 2024, Maftei Gutui Robert, Branici Radu              *
 *                                                                        *
 *  E-mail:      robert-mihaita.maftei-gutui@student.tuiasi.ro,           *
 *               radu.branici@student.tuiasi.ro                           *
 *  Description:  Book Store Online Web Application                       *
 *                Main function for application.                          *
 *                                                                        *
 *  This code and information is provided "as is" without warranty of     *
 *  any kind, either expressed or implied, including but not limited      *
 *  to the implied warranties of merchantability or fitness for a         *
 *  particular purpose. You are free to use this source code in your      *
 *  applications as long as the original copyright notice is included.    *
 *                                                                        *
 **************************************************************************/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Models;

namespace Shop.Pages
{
    [Authorize]
    public class UserModel : PageModel
    {
    

        private readonly UserManager<User> userManager;

        public User? appUser;
        public UserModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        } 
        public void OnGet()
        {
            var task = userManager.GetUserAsync(User);
            task.Wait();
            appUser=task.Result;
        }
    }
}
