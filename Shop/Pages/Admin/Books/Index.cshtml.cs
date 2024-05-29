/**************************************************************************
 *                                                                        *
 *  File:        Index.cshtml.cs                                          *
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
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Shop.Pages.Admin.Books
{
    public class IndexModel : PageModel
    {
        public List<BookInfo> listBooks = new List<BookInfo>();
        public string search = "";
        public int page = 1;
        private readonly int pageSize = 2;
        public int totalPages;
        public string column = "id";
        public string order = "desc";

        public void OnGet()
        {
            string requestPage = Request.Query["page"];
            if (requestPage != null && int.TryParse(requestPage, out int requestedPage))
            {
                page = requestedPage;
            }
            else
            {
                page = 1;
            }

            string[] columnsForSort = { "id", "title", "authors", "num_pages", "price", "category", "description", "image_filename", "created_at" };
            column = Request.Query["column"];
            if (string.IsNullOrEmpty(column) || !Array.Exists(columnsForSort, col => col.Equals(column, StringComparison.OrdinalIgnoreCase)))
            {
                column = "id";
            }

            order = Request.Query["order"];
            if (string.IsNullOrEmpty(order) || !order.Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                order = "desc";
            }

            try
            {
                search = Request.Query["search"];
                if (search == null)
                {
                    search = "";
                }

                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Shop;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM BOOKS ";
                    string sqlCount = "SELECT COUNT(*) FROM BOOKS ";

                    if (search.Length > 0)
                    {
                        sqlCount += " WHERE title LIKE @search OR authors LIKE @search OR CONVERT(VARCHAR, id) LIKE @search ";
                        sql += " WHERE title LIKE @search OR authors LIKE @search OR CONVERT(VARCHAR, id) LIKE @search ";
                    }

                    SqlCommand cmdCount = new SqlCommand(sqlCount, connection);
                    cmdCount.Parameters.AddWithValue("@search", "%" + search + "%");
                    decimal count = (int)cmdCount.ExecuteScalar();
                    totalPages = (int)Math.Ceiling(count / (decimal)pageSize);

                    sql += $" ORDER BY {column} {order} OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY ";

                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                    cmd.Parameters.AddWithValue("@skip", (page - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BookInfo bookInfo = new BookInfo
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Author = reader.GetString(2),
                                NumPages = reader.GetInt32(3),
                                Price = reader.GetDecimal(4),
                                Category = reader.GetString(5),
                                Description = reader.GetString(6),
                                ImageFileName = reader.GetString(7),
                                CreatedAt = reader.GetDateTime(8).ToString("MM/dd/yyyy")
                            };

                            listBooks.Add(bookInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    public class BookInfo
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int NumPages { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageFileName { get; set; } = "";
        public string CreatedAt { get; set; } = "";
    }
}
