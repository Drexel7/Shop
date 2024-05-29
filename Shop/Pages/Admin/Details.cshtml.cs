/**************************************************************************
 *                                                                        *
 *  File:        Details.cshtml.cs                                        *
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static Shop.Pages.Admin.IndexModel;
//Pagina unde putem vedea detaliie mesajului din contact.
namespace Shop.Pages.Admin
{
    public class DetailsModel : PageModel
    {
        public MessageInfo messageInfo = new MessageInfo();
        public void OnGet()
        {
            string requestId = Request.Query["id"];

            try
            {
                string connectionString = "Data Source =.\\sqlexpress; Initial Catalog = Shop; Integrated Security = True";
                using (SqlConnection connection=new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM messages where id=@id";
                    SqlCommand cmd =new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", requestId);
                    SqlDataReader reader=cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        messageInfo.Id = reader.GetInt32(0);
                        messageInfo.FirstName = reader.GetString(1);
                        messageInfo.LastName = reader.GetString(2);
                        messageInfo.Email = reader.GetString(3);
                        messageInfo.Phone = reader.GetString(4);
                        messageInfo.Subject = reader.GetString(5);
                        messageInfo.Message = reader.GetString(6);
                        messageInfo.CreatedAt = reader.GetDateTime(7).ToString("MM/dd//yyyy");

                    }
                    else
                    {
                        Response.Redirect("/Admin/Index");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Index");
            }
        }
    }
}
