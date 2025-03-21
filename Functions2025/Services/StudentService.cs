using System.Net.Http.Json;
using Functions2025.Models.School;

namespace Functions2025.Services
{
    public class StudentService
    {
        private readonly HttpClient _http;

        public StudentService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Student>> GetStudentsBySchoolAsync(string school)
        {
            return await _http.GetFromJsonAsync<List<Student>>($"api/students/school/{school}") ?? new List<Student>();
        }

        public async Task<List<SchoolCount>> GetStudentCountsBySchoolAsync()
        {
            return await _http.GetFromJsonAsync<List<SchoolCount>>("api/students/count-by-school") ?? new List<SchoolCount>();
        }
    }
}