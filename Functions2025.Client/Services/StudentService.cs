using System.Net.Http.Json;
using Functions2025.Models.School;

namespace Functions2025.Client.Services
{
    public class StudentService
    {
        private readonly HttpClient _http;

        public StudentService(HttpClient http)
        {
            _http = http;
            
            _http.BaseAddress = new Uri("http://localhost:7150/");
        }

        public async Task<List<Student>> GetStudentsBySchoolAsync(string school)
        {
            try
            {
                Console.WriteLine($"Calling API: api/students/school/{school}");
                var response = await _http.GetFromJsonAsync<List<Student>>($"api/students/school/{school}");
                Console.WriteLine($"Response received: {response?.Count ?? 0} students");
                return response ?? new List<Student>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting students by school: {ex.Message}");
                return new List<Student>();
            }
        }

        public async Task<List<SchoolCount>> GetStudentCountsBySchoolAsync()
        {
            try
            {
                Console.WriteLine("Calling API: api/students/count-by-school");
                var response = await _http.GetFromJsonAsync<List<SchoolCount>>($"api/students/count-by-school");
                Console.WriteLine($"Response received: {response?.Count ?? 0} school counts");
                return response ?? new List<SchoolCount>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting school counts: {ex.Message}");
                return new List<SchoolCount>();
            }
        }
        
        // Add a new student
public async Task<Student> CreateStudentAsync(Student student)
{
    try
    {
        Console.WriteLine("Calling API: POST api/students");
        var response = await _http.PostAsJsonAsync("api/students", student);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Student>() ?? new Student();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating student: {ex.Message}");
        throw;
    }
}

// Update an existing student
public async Task<Student> UpdateStudentAsync(int id, Student student)
{
    try
    {
        Console.WriteLine($"Calling API: PUT api/students/{id}");
        var response = await _http.PutAsJsonAsync($"api/students/{id}", student);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Student>() ?? new Student();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating student: {ex.Message}");
        throw;
    }
}

// Delete a student
public async Task<bool> DeleteStudentAsync(int id)
{
    try
    {
        Console.WriteLine($"Calling API: DELETE api/students/{id}");
        var response = await _http.DeleteAsync($"api/students/{id}");
        return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error deleting student: {ex.Message}");
        throw;
    }
}

    }

}