namespace SWD.Interfaces
{
    public interface IEnrollmentManager
    {
        Task<bool> EnrollRequest(string studentId, string classId);
    }
}
