# Academic Management System API

A RESTful ASP.NET Core Web API for managing the core records of a college or training institute: departments, instructors, courses, students and course enrollments. It was built as an individual project for the Software Development diploma program using N-Tier architecture, Entity Framework Core and SQL Server.

## Theme

Small institutes often keep academic information in separate spreadsheets or paper files. The same student or course gets typed in several places, nothing stops duplicate emails or duplicate enrollments, and searching long lists is slow.

This API replaces that with one relational database behind a validated, documented HTTP interface. The database enforces relationships and unique values, the business layer enforces rules (such as "a student can only be enrolled in a course once"), and clients only ever see DTOs, never the raw database entities.

**Target users:** registrar and administrative staff, department heads and instructors, and front-end developers who want to build student portals or dashboards on top of the API.

## Features

- Full CRUD for students (create, read, replace with `PUT`, partial update with `PATCH`, delete)
- Complex search endpoint: filter students by name, email or department, with pagination
- Departments (list, get by ID, create) and courses (list with department and instructor details)
- Enroll an existing student in an existing course; duplicate enrollments return `409 Conflict`
- DTOs with manual mapping (entities are never exposed to clients)
- Input validation with FluentValidation and structured `400 Bad Request` responses
- Global exception handling that returns RFC 7807 `ProblemDetails` JSON
- `async`/`await` end-to-end, including all Entity Framework Core queries
- Swagger / OpenAPI documentation with XML comments

## Tech stack

| Area | Technology |
|---|---|
| Language / framework | C#, ASP.NET Core Web API (.NET 9) |
| Data access | Entity Framework Core 8 (code-first migrations) |
| Database | Microsoft SQL Server |
| Validation | FluentValidation |
| Documentation | Swagger (Swashbuckle) |

## Architecture

The solution is split into three projects, each with one responsibility.

| Project | Layer | Responsibility |
|---|---|---|
| `AcademicManagementAPI` | API | Controllers only: routing, receiving input and returning status codes. Also `Program.cs` (dependency injection, Swagger, global error handling). |
| `AcademicManagement.BLL` | Business | Services with the business rules, DTOs, mapping extension methods and FluentValidation validators. |
| `AcademicManagement.DAL` | Data | `AcademicDbContext`, entity classes, migrations and repositories. |

```
AcademicManagementAPI/
├── AcademicManagementAPI/          API project
│   ├── Controllers/                Students, Departments, Course
│   └── Program.cs
├── InstituteManagement.BLL/        Business layer (AcademicManagement.BLL project)
│   ├── DTOs/
│   ├── Mapping/
│   ├── Services/
│   └── Validation/
└── AcademicManagement.DAL/         Data layer
    ├── Data/                       AcademicDbContext
    ├── Entities/
    ├── Migrations/
    └── Repository/
```

## Database schema

Five tables. Primary keys are marked **PK**, foreign keys **FK**, and unique columns **UQ**.

| Table | Columns |
|---|---|
| `Departments` | **DepartmentId** (PK), Name, Code (UQ) |
| `Instructors` | **InstructorId** (PK), FullName, Email (UQ), DepartmentId (FK) |
| `Students` | **StudentId** (PK), FirstName, LastName, Email (UQ), DepartmentId (FK) |
| `Courses` | **CourseId** (PK), CourseCode (UQ), Title, Credits, DepartmentId (FK), InstructorId (FK) |
| `Enrollments` | **StudentId** (PK, FK), **CourseId** (PK, FK), EnrolledOn, Grade (nullable) |

Relationships:

- Department → Students, Instructors and Courses (one-to-many)
- Instructor → Courses (one-to-many)
- Student ↔ Course (many-to-many, through `Enrollments`)

Deleting a department or instructor that still has students or courses is blocked (`Restrict`). Deleting a student or a course also removes its enrollment rows (`Cascade`).

## Getting started

### Prerequisites

- Visual Studio 2022 (or later) with the ASP.NET and web development workload, or the .NET 9 SDK
- SQL Server (LocalDB or SQL Server Express) and SQL Server Management Studio (optional, for viewing data)

### Setup

1. Clone the repository and open `AcademicManagementAPI.slnx` in Visual Studio.
2. In `AcademicManagementAPI/appsettings.json`, set the `Server=` value in the connection string to your own SQL Server instance, for example:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AcademicManagementDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
   }
   ```

3. Create the database and seed data. In **Tools → NuGet Package Manager → Package Manager Console**, run:

   ```powershell
   Update-Database -Project AcademicManagement.DAL -StartupProject AcademicManagementApi
   ```

4. Set `AcademicManagementApi` as the startup project and press **F5**. Swagger opens at `https://localhost:7265/swagger` (or `http://localhost:5216/swagger`).

The migration seeds two departments, three instructors, three courses, three students and four enrollments so you can try the endpoints straight away.

## Authentication

This version of the API does **not** use authentication, so there are no credentials to log in with. All endpoints are open. Adding authentication and role-based authorization is listed under future improvements.

## API endpoints

| Method | Route | Description | Success | Errors |
|---|---|---|---|---|
| GET | `/api/departments` | List departments with student counts | 200 | 500 |
| GET | `/api/departments/{id}` | Get one department | 200 | 404 |
| POST | `/api/departments` | Create a department | 201 | 400, 409 |
| GET | `/api/course` | List courses with department and instructor | 200 | 500 |
| GET | `/api/students` | Search, filter and paginate students | 200 | 400, 404 |
| GET | `/api/students/{id}` | Get one student | 200 | 404 |
| POST | `/api/students` | Create a student | 201 | 400, 404, 409 |
| PUT | `/api/students/{id}` | Replace all editable fields of a student | 204 | 400, 404, 409 |
| PATCH | `/api/students/{id}` | Update one or more fields of a student | 204 | 400, 404, 409 |
| DELETE | `/api/students/{id}` | Delete a student | 204 | 404 |
| POST | `/api/students/{studentId}/enrollments` | Enroll a student in a course | 201 | 400, 404, 409 |

### Search and pagination

`GET /api/students` accepts these query parameters:

| Parameter | Description | Default |
|---|---|---|
| `search` | Matches first name, last name or email | none |
| `departmentId` | Only students in this department | none |
| `pageNumber` | Page to return (minimum 1) | 1 |
| `pageSize` | Items per page (1 to 100) | 10 |

The total number of matching students is returned in the `X-Total-Count` response header.

Example: `GET /api/students?search=John&departmentId=1&pageNumber=1&pageSize=10`

### Example request

`POST /api/students`

```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane.doe@student.ca",
  "departmentId": 1
}
```

Responses: `201 Created` with the new student, `400` if validation fails, `404` if the department doesn't exist, and `409` if the email is already used.

## Validation and error handling

- Every input DTO is validated with FluentValidation (required fields, maximum lengths, email format, positive IDs). Failures return `400 Bad Request` with errors listed per field.
- Missing records return `404 Not Found`, and duplicates (email, department code, enrollment) return `409 Conflict`.
- Any unexpected exception is logged and returned as a generic `500` `ProblemDetails` response, without exposing internal details.

## Future improvements

- Authentication and role-based authorization (admin, instructor, student)
- Grade entry and transcript reports
- Update and delete endpoints for instructors, courses and departments
- Course scheduling and academic terms
- Un-enrolling students and enrollment history