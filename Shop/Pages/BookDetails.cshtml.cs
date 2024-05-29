using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Pages.Admin.Books;
using System.Data.SqlClient;
///Branici Radu
/// Utilizeaza SQL server pentru a obtine detalii despre carti

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
        public BookInfo bookInfo { get; private set; }

        public BookDetailsModel(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("Error");
            }

            bookInfo = _bookRepository.GetBookById(id.Value);

            if (bookInfo == null)
            {
                return RedirectToPage("/");
            }

            return Page();
        }
    }

}