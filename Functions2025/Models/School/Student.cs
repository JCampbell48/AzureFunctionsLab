namespace Functions2025.Models.School
{
    public class Student
    {
        public int StudentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? School { get; set; }
    }

    public class SchoolCount
    {
        public string? School { get; set; }
        public int Count { get; set; }
    }
}