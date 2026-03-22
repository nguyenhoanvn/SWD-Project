using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SWD.Service;
using SWD.Service.Entities;

namespace SWD.Pages
{
    public class IndexModel : PageModel
    {
        public List<Student> Students { get; private set; } = [];

        public void OnGet()
        {
            Students = CECMSContext.Instance.Students;
        }

        public IActionResult OnPost(string studentId)
        {
            return RedirectToPage("/Enrollment/SelectClass", new { studentId });
        }
    }
}
