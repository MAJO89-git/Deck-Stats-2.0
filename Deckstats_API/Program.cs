using Deckstats_API;

Database.Initialize();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader()
);

app.MapPost("/api/users/login", (LoginDto dto) =>
{
    var result = UserService.Login(dto.Email, dto.Password);

    if (!result.IsSuccess)
        return Results.Unauthorized();

    return Results.Ok(result);
});

app.Run();