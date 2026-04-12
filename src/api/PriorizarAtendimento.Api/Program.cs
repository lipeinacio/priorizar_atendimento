using PriorizarAtendimento.Api.Services;
using PriorizarAtendimento.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton<DecisaoResposta>();
builder.Services.AddSingleton<DadosTeste>();
builder.Services.AddSingleton<LeitorLegado>();
builder.Services.AddSingleton<RepositorioDecisao>();
builder.Services.AddSingleton<DatabaseConnectionFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.MapControllers();

app.Run();




