/**************************************************************************
 *                                                                        *
 *  File:        Books.cshtml.cs                                          *
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
// <!-- Maftei Gutui Robert Mihaita-->
//Pagina Filter. Se folosesc multe QUERY-uri SQL pentru a filtra cartile din baza de date si a le afisa in web


namespace Shop.Pages
{
    [BindProperties(SupportsGet = true)]
    public class BooksModel : PageModel
    {   // any sunt declarate ca sa putem avea niste valori default la cele 2 filtre
        public string? Search { get; set; }
        public string PriceRange { get; set; } = "any";
        public string Category { get; set; } = "any";

        public int page = 1;
        public int totalPages = 0;
        private readonly int pageSize = 4;

        public List<BookInfo> listBooks=new List<BookInfo>();
        public void OnGet()
        {
            page = 1;
            string requestPage = Request.Query["page"]; 
            if(requestPage != null)
            {
                try
                {
                    page = int.Parse(requestPage);
                }
                catch(Exception ex)
                {
                    page = 1;
                }
            }
            try
            {


                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Shop;Integrated Security=True";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sqlCount = "SELECT COUNT(*) FROM BOOKS ";
                sqlCount += " WHERE (title LIKE @search OR authors LIKE @search) ";

                if (PriceRange.Equals("0_100"))
                {
                    sqlCount += " AND price <= 100 ";
                    Console.WriteLine("Da");
                }

                if (PriceRange.Equals("100_200"))
                {
                    sqlCount += " AND price <= 200 AND PRICE >= 100 ";
                }


                if (PriceRange.Equals("above200"))
                {
                    sqlCount += "AND price > 200 ";
                }


                if (!Category.Equals("any"))
                {
                    sqlCount += " AND category=@category";
                    Console.WriteLine("Da");
                }

                SqlCommand cmd2 = new SqlCommand(sqlCount, connection);
                cmd2.Parameters.AddWithValue("@search", "%" + Search + "%");
                cmd2.Parameters.AddWithValue("@category", Category);
                decimal count=(int)cmd2.ExecuteScalar();
                totalPages=(int)Math.Ceiling(count/pageSize);


                string sql = "SELECT * FROM BOOKS ";
                sql += " WHERE (title LIKE @search OR authors LIKE @search) ";
                Console.WriteLine(PriceRange);
                Console.WriteLine(Category);

                // aici egalarea este facuta case sensitive
                if (PriceRange.Equals("0_100"))
                {
                    sql += " AND price <= 100 ";
                    Console.WriteLine("Da");
                }

                if (PriceRange.Equals("100_200"))
                {
                    sql += " AND price <= 200 AND PRICE >= 100 ";
                }


                if (PriceRange.Equals("above200"))
                {
                    sql += "AND price > 200 ";
                }


                if (!Category.Equals("any")) 
                {
                    sql += " AND category=@category";
                    Console.WriteLine("Da");
                }
                // FETCH NExt este folosit pentru a implementa paginarea . @pagesize extrage numarul specific pe care il dorim de iteme 
                sql += " ORDER BY id ASC ";
                sql += "OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY ";


                // Procentele astea 2 inainte de search sunt folosite pentru a cauta ce este pus in textbox-ul la form oriunde in string
                // Ca exemplu, Daca cautam ih si noi avem Mihai. Ne apare Corect
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@search", "%" + Search + "%"); 
                cmd.Parameters.AddWithValue("@category",Category ); 
                cmd.Parameters.AddWithValue("@skip",(page-1)*pageSize);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                
             
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
                    listBooks.Add(bookInfo);


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }
    }
}
    