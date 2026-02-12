namespace StudentsApi.Dtos
{
    public record CreateStudentDto(string FirstName, string LastName, DateTime DateOfBirth);
    public record StudentResponseDto(int Id, string FirstName, string LastName, DateTime DateOfBirth);

    public record CreateQualificationDto(string Subject, decimal Grade);
    public record QualificationResponseDto(int Id, string Subject, decimal Grade, DateTime Date);
}
