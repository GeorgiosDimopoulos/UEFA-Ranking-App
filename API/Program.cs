using Core.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var connectionString = builder.Configuration.GetConnectionString("Default") 
    ?? throw new InvalidOperationException("Connection string is not set.");
DatabaseInitializer.EnsureCountryTableExists(connectionString);

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();

SQLitePCL.Batteries_V2.Init();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Welcome to UEFA Ranking API");

app.Run();

//void CreateDbPath()
//{
//    var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
//    Directory.CreateDirectory(dataDir);
//    var dbPath = Path.Combine(dataDir, "uefa.db");
    
//    var csb = new SqliteConnectionStringBuilder
//    {
//        DataSource = dbPath,
//        Mode = SqliteOpenMode.ReadWriteCreate,
//        Cache = SqliteCacheMode.Shared
//    };

//    var cs = csb.ToString();

//    builder.Configuration["ConnectionStrings:Default"] = cs;
//}