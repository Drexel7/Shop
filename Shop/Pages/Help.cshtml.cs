using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
// < !--Maftei Gutui Robert Mihaita-->
namespace Shop.Pages
{
    public class HelpModel : PageModel
    {
        private readonly ILogger<HelpModel> _logger;

        public HelpModel(ILogger<HelpModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}