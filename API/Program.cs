using Core.Interfaces;
using Infrastructure.DataAccess;
using Infrastructure.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

builder.Services.AddScoped<DatabaseFeeder>();
builder.Services.AddScoped<DatabaseInitializer>();

SQLitePCL.Batteries_V2.Init();

var dataDir = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDir);

var dbPath = Path.Combine(dataDir, "uefa.db");
if (string.IsNullOrEmpty(dbPath))
{
    throw new InvalidOperationException("Connection string is not set.");
}

var connectionString = $"Data Source={dbPath}";
builder.Configuration["ConnectionStrings:Default"] = connectionString;

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new() { Title = "UEFA API Endpoints", Version = "v1" });
    opt.DocInclusionPredicate((docName, apiDesc) => true);

    opt.OrderActionsBy(apiDesc =>
    {
        var group = apiDesc.GroupName ?? apiDesc.ActionDescriptor.RouteValues["controller"];
        return group switch
        {
            "Countries" => "1-",
            "Teams" => "2-",
            "Matches" => "3-",
            _ => "-",
        };
    });

});

var app = builder.Build();
app.Logger.LogInformation($"SQLite DB path is: {dbPath}");

using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    var databaseFeeder = scope.ServiceProvider.GetRequiredService<DatabaseFeeder>();

    databaseInitializer.EnsureCountryTableExists(connectionString);
    var recordsExist = databaseFeeder.EnsureRecordsExist(connectionString);
    if (!recordsExist)
    {
        await databaseFeeder.SeedCountriesAndTeams();
    }
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();