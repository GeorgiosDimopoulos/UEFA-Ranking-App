using Core.Interfaces;
using Infrastructure.DataAccess;
using Infrastructure.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDir);

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();

builder.Services.AddScoped<DatabaseFeeder>();
builder.Services.AddScoped<DatabaseInitializer>();

SQLitePCL.Batteries_V2.Init();

var app = builder.Build();

var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string is not set.");
using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    var databaseFeeder = scope.ServiceProvider.GetRequiredService<DatabaseFeeder>();

    databaseInitializer.EnsureCountryTableExists(connectionString);
    var recordsExist = databaseFeeder.EnsureRecordsExist(connectionString);
    if (!recordsExist)
    {
        databaseFeeder.SeedCountriesAndTeams();
    }
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    //app.MapPost("/api/database/seed", (DatabaseFeeder feeder) =>
    //{
    //    feeder.SeedCountriesAndTeams();
    //    return Results.Ok("Database seeded successfully.");
    //});
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Welcome to UEFA Ranking API");

app.Run();