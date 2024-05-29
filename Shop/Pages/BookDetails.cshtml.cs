/**************************************************************************
 *                                                                        *
 *  File:        BookDetails.cshtml.cs                                    *
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
using Shop.Pages.Admin.Books;
using System.Data.SqlClient;

public interface IBookRepository
{
    BookInfo GetBookById(int id);
}

public class BookRepository : IBookRepository
{
    private readonly string _connectionString;

    public BookRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public BookInfo GetBookById(int id)
    {
        BookInfo bookInfo = null;

        try
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM BOOKS WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bookInfo = new BookInfo
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Author = reader.GetString(2),
                                NumPages = reader.GetInt32(3),
                                Price = reader.GetDecimal(4),
                                Category = reader.GetString(5),
                                Description = reader.GetString(6),
                                ImageFileName = reader.GetString(7)
                            };
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            // Handle exception (logging, rethrowing, etc.)
        }

        return bookInfo;
    }
}

namespace Shop.Pages
{
    public class BookDetailsModel : PageModel
    {
        private readonly IBookRepository _bookRepository;
        public BookInfo BookInfo { get; private set; }

        public BookDetailsModel(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null || id <= 0 || id == int.MaxValue)
            {
                return RedirectToPage("Error");
            }

            try
            {
                BookInfo = _bookRepository.GetBookById(id.Value);

                if (BookInfo == null)
                {
                    return RedirectToPage("/");
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToPage("Error");
            }

            return Page();
        }

        public IActionResult AddToCart(int? bookId)
        {
            if (bookId == null || bookId <= 0 || bookId == int.MaxValue)
            {
                return Content("Invalid book ID.");
            }

            // Logic for adding the book to the cart goes here
            // For now, let's just return a message indicating success
            return Content($"Book with ID {bookId} added to cart successfully.");
        }


    }
}
