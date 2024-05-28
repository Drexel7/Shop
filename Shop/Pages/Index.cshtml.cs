﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Pages.Admin.Books;
using System.Data.SqlClient;
//Pagina principala.
//Clasa PageModel mosteneste IndexModel. In general toate clasele o  sa mosteneasca PageModel. Motivul pentru care
//se face asta este ca asa se recunosc metodele OnGet si OnPost in Front-end, precum si legatura intre back-end si front-end
// cu @Model.<Proprietate>. Toate paginile folosesc un connectionString (trebuia declarata global sau o metoda mai buna decat initializarea ei de fiecare data)
//Si folosind metoda using se face automat clean-up la readere, connexiuni, etc. (Uneori am uitat sa folosesc using)
//Foloseste 2 QUERY-uri SQL pentru a determina cele mai recente carti si cele mai vandute
namespace Shop.Pages
{
    public class IndexModel : PageModel
    {
        public List<BookInfo> NewestBooks = new List<BookInfo>(); 
        public List<BookInfo> TopBooks = new List<BookInfo>(); 
        public void OnGet()
        {
            try
            {
                // Cele mai recente carti introduse. Functioneaza
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Shop;Integrated Security=True";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sql = "SELECT TOP 4 * FROM BOOKS ORDER by created_at DESC ";
                   
                SqlCommand cmd = new SqlCommand(sql, connection);
                
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {


                        BookInfo bookInfo = new BookInfo();
                        bookInfo.Id = reader.GetInt32(0);
                        bookInfo.Title = reader.GetString(1);
                        bookInfo.Author = reader.GetString(2);
                        bookInfo.NumPages = reader.GetInt32(3);
                        bookInfo.Price = reader.GetDecimal(4);
                        bookInfo.Category = reader.GetString(5);
                        bookInfo.Description = reader.GetString(6);
                        bookInfo.ImageFileName = reader.GetString(7);
                        bookInfo.CreatedAt = reader.GetDateTime(8).ToString("MM/dd/yyyy");
                        NewestBooks.Add(bookInfo);
                    }
                reader.Close();
                // cartile cele mai vandute. se foloseste baza de date order_items. Pentru a calcula. Nu cred ca functioneaza
                sql = "SELECT TOP 4 BOOKS.*, (" +
                    "SELECT SUM(order_items.quantity) FROM order_items WHERE books.id = order_items.book_id" +
                    ") AS total_sales " +
                    "FROM BOOKS " +
                    "ORDER BY total_sales desc";

                cmd = new SqlCommand(sql, connection);
                reader = cmd.ExecuteReader();
          
                while (reader.Read())
                {
    

                    BookInfo bookInfo = new BookInfo();
                    bookInfo.Id = reader.GetInt32(0);
                    bookInfo.Title = reader.GetString(1);
                    bookInfo.Author = reader.GetString(2);
                    bookInfo.NumPages = reader.GetInt32(3);
                    bookInfo.Price = reader.GetDecimal(4);
                    bookInfo.Category = reader.GetString(5);
                    bookInfo.Description = reader.GetString(6);
                    bookInfo.ImageFileName = reader.GetString(7);
                    bookInfo.CreatedAt = reader.GetDateTime(8).ToString("MM/dd/yyyy");
                    Console.Write(bookInfo);
                    TopBooks.Add(bookInfo);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

        }
    }
}