// Pages/Payment/Process.cshtml.cs
// UC18: Process Online Payment
// «user interaction» StudentInteraction → selectPaymentMethod → displayPaymentForm

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SWD.Data;
using SWD.Services;
using SWD.Interfaces;
using SWD.Domain.Models;
using SWD.Application.DTOs;

namespace SWD.Pages.Payment
{
    public class ProcessModel : PageModel
    {
        private readonly IEnrollmentCoordinator _coordinator;
        private readonly IFinancialService      _financial;
        private readonly AppDbContext           _db;

        public ProcessModel(IEnrollmentCoordinator coordinator, IFinancialService financial, AppDbContext db)
        {
            _coordinator = coordinator;
            _financial   = financial;
            _db          = db;
        }

        [BindProperty(SupportsGet = true)] public string StudentId      { get; set; } = "";
        [BindProperty(SupportsGet = true)] public string ClassId { get; set; } = "";

        [BindProperty] public string SelectedMethod { get; set; } = "VNPay";
        [BindProperty] public Student CurrentStudent { get; set; } = null!;
        [BindProperty] public Class SelectedClass { get; set; } = null!;


        // UC18 Step 1–2: displayPaymentForm
        public async Task OnGetAsync()
        {
            CurrentStudent = _db.Students.FirstOrDefault(s => s.StudentId == StudentId);
            SelectedClass = _db.Classes.FirstOrDefault(c => c.ClassId == ClassId);
        }

        // UC18 Step 3→7: student chọn phương thức → hệ thống xử lý
        public async Task<IActionResult> OnPostAsync()
        {

            // UC18 msg 1.1 requestEnrollment → 1.2 initiatePayment
            var payReq = new PaymentRequest(
                StudentId,
                ClassId,
                SelectedMethod,
                SelectedClass.Fee
                
            );

            // FinancialService.initiatePayment (bao gồm gọi gateway và lưu Payment)
            var payResult = await _financial.InitiatePayment(payReq);

            // UC18 msg 2.2: processTransactionResult — coordinator cập nhật Registration + gửi thông báo
            var enrollResult = await _coordinator.ProcessTransactionResult(StudentId, payResult);

            // Chuyển sang trang kết quả (statusResponse)
            return RedirectToPage("/Payment/Result", new
            {
                studentId = StudentId,
                success        = enrollResult.IsSuccess,
                message        = enrollResult.Message,
                txnRef         = payResult.TransactionRef
            });
        }

        // UC18 Step 3a: student hủy
        public async Task<IActionResult> OnPostCancelAsync()
        {
            return RedirectToPage("/Courses/Index", new { SelectedStudentId = StudentId });
        }
    }
}
