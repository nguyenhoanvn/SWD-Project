// Pages/Payment/Result.cshtml.cs
// UC18 Step 7 + msg 2.6/2.7: statusResponse — hiển thị kết quả cho student

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SWD.Pages.Payment
{
    public class ResultModel : PageModel
    {
        public string  StudentId { get; set; } = "";
        public bool    Success        { get; set; }
        public string  Message        { get; set; } = "";
        public string  TxnRef         { get; set; } = "";

        public void OnGet(string studentId, bool success, string message, string txnRef)
        {
            StudentId = studentId;
            Success        = success;
            Message        = message;
            TxnRef         = txnRef;
        }
    }
}
