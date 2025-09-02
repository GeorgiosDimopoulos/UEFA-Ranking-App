using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
                                  
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
