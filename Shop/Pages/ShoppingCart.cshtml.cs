/**************************************************************************
 *                                                                        *
 *  File:        ShoppingCart.cshtml.cs                                   *
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
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Pages;
using Shop.Pages.Admin.Books;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Transactions;



public interface ICartObserver
{
    void AddressChanged(string newAddress);
    void PaymentMethodChanged(string newPaymentMethod);
}
public static class Globals
{
    public static int addressChangeCount = 0; // Unmodifiable
    public static int paymentMethodChangeCount = 0;
}




public class CartLogger : ICartObserver
{


    public void AddressChanged(string newAddress)
    {
        Globals.addressChangeCount++;
        Console.WriteLine($"Address changed to: {newAddress}. Total address changes: {Globals.addressChangeCount}");
    }

    public void PaymentMethodChanged(string newPaymentMethod)
    {
        Globals.paymentMethodChangeCount++;
        Console.WriteLine($"Payment method changed to: {newPaymentMethod}. Total payment method changes: {Globals.paymentMethodChangeCount}");
    }
}



namespace Shop.Pages
{
    [BindProperties]
    public class ShoppingCartModel : PageModel
    {
        [Required(ErrorMessage = "The address is required")]
        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                if (address != value)
                {
                    address = value;
                    NotifyAddressChanged();
                }
            }
        }

        [Required(ErrorMessage = "The payment method is required")]
        private string paymentMethod;
        public string PaymentMethod
        {
            get { return paymentMethod; }
            set
            {
                if (paymentMethod != value)
                {
                    paymentMethod = value;
                    NotifyPaymentMethodChanged();
                }
            }
        }

        private List<ICartObserver> observers = new List<ICartObserver>();
        private CartLogger cartLogger = new CartLogger();
        public ShoppingCartModel()
        {
            Attach(cartLogger);
        }

        public void Attach(ICartObserver observer)
        {
            observers.Add(observer);
        }

        public void NotifyAddressChanged()
        {
            foreach (var observer in observers)
            {
                observer.AddressChanged(Address);
            }
        }

        public void NotifyPaymentMethodChanged()
        {
            foreach (var observer in observers)
            {
                observer.PaymentMethodChanged(PaymentMethod);
            }
        }




        public List<OrderItem> items = new List<OrderItem>();

        private Dictionary<String, int> getBookDictionary()
        {
            var bookDictionary = new Dictionary<String, int>();
            string cookieValue = Request.Cookies["shopping_cart"] ?? "";

            if (cookieValue.Length > 0)
            {
                string[] bookIdArray = cookieValue.Split('-');
                Console.WriteLine(bookIdArray);
                for (int i = 0; i < bookIdArray.Length; i++) // Fixed the termination condition here
                {
                    string bookId = bookIdArray[i];
                    if (bookDictionary.ContainsKey(bookId))
                    {
                        bookDictionary[bookId]++;
                    }
                    else
                    {
                        bookDictionary.Add(bookId, 1);
                    }
                }
            }
            return bookDictionary;
        }



        public void OnGet()
        {
            var bookDictionary = getBookDictionary();
            string? action = Request.Query["action"];
            string? id = Request.Query["id"];
            if (action != null && id != null && bookDictionary.ContainsKey(id))
            {



                if (action.Equals("add"))
                {
                    bookDictionary[id] += 1;
                }
                else if (action.Equals("sub"))
                {
                    if (bookDictionary[id] > 1)
                        bookDictionary[id] -= 1;
                }
                else if (action.Equals("delete"))
                {
                    bookDictionary.Remove(id);
                }
                string newCookie = "";
                foreach (var keyValuePair in bookDictionary)
                {
                    for (int i = 0; i < keyValuePair.Value; i++)
                    {
                        newCookie += "-" + keyValuePair.Key;
                    }
                }

                if (newCookie.Length > 0)
                {
                    newCookie = newCookie.Substring(1);

                }

                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(365);
                cookieOptions.Path = "/";
                Response.Cookies.Append("shopping_cart", newCookie, cookieOptions);
                Response.Redirect(Request.Path.ToString());

            }
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Shop;Integrated Security=True";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sql = "SELECT * FROM BOOKS WHERE id=@id ";
                foreach (var keyValuePair in getBookDictionary())
                {
                    string bookId = keyValuePair.Key;
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@id", bookId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        OrderItem item = new();

                        item.bookInfo.Id = reader.GetInt32(0);
                        item.bookInfo.Title = reader.GetString(1);
                        item.bookInfo.Author = reader.GetString(2);
                        item.bookInfo.NumPages = reader.GetInt32(3);
                        item.bookInfo.Price = reader.GetDecimal(4);
                        item.bookInfo.Category = reader.GetString(5);
                        item.bookInfo.Description = reader.GetString(6);
                        item.bookInfo.ImageFileName = reader.GetString(7);
                        item.bookInfo.CreatedAt = reader.GetDateTime(8).ToString("MM/dd/yyyy");
                        item.numCopies = keyValuePair.Value;
                        item.totalPrice = item.numCopies * item.bookInfo.Price;
                        items.Add(item);

                    }
                    reader.Close();
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public string ErrorMessage = "";
        public string SuccessMessage = "";
        public void OnPost()
        {
            if (!ModelState.IsValid)
            {

            }
            var bookDictionary = getBookDictionary();
            if (bookDictionary.Count < 1)
            {
                ErrorMessage = "Cart is empty";
                return;
            }
            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Shop;Integrated Security=True";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sql = "INSERT INTO orders (order_date, " +
             "delivery_adress, payment_method, payment_status, order_status) " +
             "VALUES (CURRENT_TIMESTAMP, " +
             "@delivery_adress, @payment_method, 'pending', 'created'); " +
             "SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(sql, connection);
                Address = Request.Form["deliveryAddress"];
                cmd.Parameters.AddWithValue("@delivery_adress", Address);
                cmd.Parameters.AddWithValue("@payment_method", PaymentMethod);

                // Use ExecuteScalar to retrieve the generated order ID
                int orderId = Convert.ToInt32(cmd.ExecuteScalar());


                string order_item_sql = "INSERT INTO order_items (order_id,book_id,quantity,unit_price) " +
                    "VALUES (@order_id,@book_id,@quantity,@unit_price)";





                foreach (var item in bookDictionary)
                {
                    string bookID = item.Key;
                    int quantity = item.Value;
                    decimal unitPrice = getbookPrice(bookID);

                    SqlCommand cmd2 = new SqlCommand(order_item_sql, connection);
                    cmd2.Parameters.AddWithValue("@order_id", orderId);
                    cmd2.Parameters.AddWithValue("@book_id", bookID);
                    cmd2.Parameters.AddWithValue("@quantity", quantity);
                    cmd2.Parameters.AddWithValue("unit_price", unitPrice);
                    cmd2.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message; return;

            }
            Response.Cookies.Delete("shopping_cart");
            SuccessMessage = "Order Created";

        }

        private decimal getbookPrice(string bookID)
        {
            decimal price = 0;

            try
            {
                string connectionString = "Data Source=.\\sqlexpress;Initial Catalog=Shop;Integrated Security=True";
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string sql = "SELECT price from BOOKS WHERE id=@id";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@id", bookID);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    price = reader.GetDecimal(0);
                }

                reader.Dispose();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return price;
        }
    }

    public class OrderItem
    {
        public BookInfo bookInfo = new();
        public int numCopies = 0;
        public decimal totalPrice = 0;
    }
}


