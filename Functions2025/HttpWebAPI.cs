using System;
using System.Net;
using System.Linq;
using Functions2025.Models.School;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Snoopy.Function;

public class HttpWebAPI
{
    private readonly ILogger<HttpWebAPI> _logger;
    private readonly SchoolContext _context;

    public HttpWebAPI(ILogger<HttpWebAPI> logger, SchoolContext context)
    {
        _logger = logger;
        _context = context;
    }

    [Function("HttpWebAPI")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain");
        response.WriteString("Welcome to Azure Functions!");

        return response;
    }

    [Function("GetStudents")]
    public HttpResponseData GetStudents(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP GET trigger function processed a request in GetStudents().");

        var students = _context.Students.ToArray();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteString(JsonConvert.SerializeObject(students));

        return response;
    }

    [Function("GetStudentById")]
    public HttpResponseData GetStudentById(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/{id}")] HttpRequestData req,
    string id)  // Change from int to string
    {
        _logger.LogInformation("C# HTTP GET trigger function processed a request.");

        // Try to parse the ID
        if (!int.TryParse(id, out int studentId))
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            errorResponse.Headers.Add("Content-Type", "application/json");
            errorResponse.WriteString(JsonConvert.SerializeObject(new { error = $"Invalid student ID: {id}" }));
            return errorResponse;
        }

        var student = _context.Students.FindAsync(studentId).Result;
        if (student == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "text/plain");
            response.WriteString("Not Found");
            return response;
        }

        var response2 = req.CreateResponse(HttpStatusCode.OK);
        response2.Headers.Add("Content-Type", "application/json");
        response2.WriteString(JsonConvert.SerializeObject(student));

        return response2;
    }

    [Function("CreateStudent")]
    public async Task<HttpResponseData> CreateStudent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "students")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP POST trigger function processed a request.");

        string requestBody = await req.ReadAsStringAsync();
        var student = JsonConvert.DeserializeObject<Student>(requestBody);

        _context.Students.Add(student);
        _context.SaveChanges();

        var response = req.CreateResponse(HttpStatusCode.Created);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteString(JsonConvert.SerializeObject(student));

        return response;
    }

    [Function("UpdateStudent")]
    public async Task<HttpResponseData> UpdateStudent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "students/{id}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation("C# HTTP PUT trigger function processed a request.");

        var student = _context.Students.FindAsync(id).Result;
        if (student == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "text/plain");
            response.WriteString("Not Found");
            return response;
        }

        string requestBody = await req.ReadAsStringAsync();
        var student2 = JsonConvert.DeserializeObject<Student>(requestBody);

        student.FirstName = student2.FirstName;
        student.LastName = student2.LastName;
        student.School = student2.School;
        _context.SaveChanges();

        var response2 = req.CreateResponse(HttpStatusCode.OK);
        response2.Headers.Add("Content-Type", "application/json");
        response2.WriteString(JsonConvert.SerializeObject(student));

        return response2;
    }

    [Function("DeleteStudent")]
    public HttpResponseData DeleteStudent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "students/{id}")] HttpRequestData req,
        int id)
    {
        _logger.LogInformation("C# HTTP DELETE trigger function processed a request.");

        var student = _context.Students.FindAsync(id).Result;
        if (student == null)
        {
            var response = req.CreateResponse(HttpStatusCode.NotFound);
            response.Headers.Add("Content-Type", "text/plain");
            response.WriteString("Not Found");
            return response;
        }

        _context.Students.Remove(student);
        _context.SaveChanges();

        var response2 = req.CreateResponse(HttpStatusCode.OK);
        response2.Headers.Add("Content-Type", "application/json");
        response2.WriteString(JsonConvert.SerializeObject(student));

        return response2;
    }

    [Function("GetStudentsBySchool")]
    public HttpResponseData GetStudentsBySchool(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/school/{school}")] HttpRequestData req,
    string school)
    {
        _logger.LogInformation($"C# HTTP GET trigger function processed a request to get students from school: {school}");

        // Filter students by school name - use ToLower() which EF Core can translate
        var students = _context.Students
            .Where(s => s.School != null && s.School.ToLower() == school.ToLower())
            .ToArray();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteString(JsonConvert.SerializeObject(students));

        return response;
    }

    [Function("GetStudentCountBySchool")]
    public HttpResponseData GetStudentCountBySchool(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/count-by-school")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP GET trigger function processed a request to count students by school");

        // Group students by school and count them
        var schoolCounts = _context.Students
            .Where(s => s.School != null)
            .GroupBy(s => s.School)
            .Select(g => new
            {
                School = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToArray();

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteString(JsonConvert.SerializeObject(schoolCounts));

        return response;
    }
}