namespace SWD.Interfaces
{
    public interface IAcademicManager
    {
        Task<bool> ValidatePrerequisite(string studentId, string classId);
        Task<bool> ValidateSchedule(string studentId, string classId);
        Task<bool> ValidateCapacity(string classId);
    }
}
