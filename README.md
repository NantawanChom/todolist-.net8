# todolist-.net8

[Document](https://learn.microsoft.com/en-us/ef/core/)

1. create project
```bash
dotnet new webapi -n TodoListApi
```
2. Install the necessary packages (Example)
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package dotenv.net
```

3. Install EF Core CLI tools if you haven't already:
```bash
dotnet tool install --global dotnet-ef
```

4. Add a migration and update the database
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. Run Project
```bash
dotnet run
dotnet watch
```

6. Build Project
```bash
dotnet build
```

7. Cleaning a Project

```bash
dotnet clean
```

8. Cleaning a Specific Configuration

```bash
dotnet clean --configuration {specific configuration}

ex.

dotnet clean --configuration Release
```

9. Cleaning a Specific Framework

```bash
dotnet clean --framework net6.0
```

10. Cleaning a Solution

```bash
dotnet clean MySolution.sln
```

11. Console for debug
```
Console.WriteLine($"UserId: {userId}");
```

**Include Urls**

GET 
1. http://localhost:5244/api/todos
2. http://localhost:5244/api/todos?pageSize=2
3. http://localhost:5244/api/todos?pageSize=2&lastId=10
4. http://localhost:5244/api/todos/1

POST
1. http://localhost:5244/api/todos
2. http://localhost:5244/api/auth/register
3. http://localhost:5244/api/auth/login

PUT
1. http://localhost:5244/api/todos/1

DELETE
1. http://localhost:5244/api/todos/1



# Checklist

- [ ] Logging
- [ ] Testing
- [ ] Refactor code