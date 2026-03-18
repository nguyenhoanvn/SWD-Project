namespace SWD.Interfaces
{
    public interface IAcademicService
    {
        Task<bool> ValidatePrerequisite(string studentId, string classId);
        Task<bool> ValidateSchedule(string studentId, string classId);
        Task<bool> ValidateCapacity(string classId);
    }
}
