using AgendaDeAtendimentos.Data;
using Microsoft.EntityFrameworkCore;
using AgendaDeAtendimentos.ProfissionalController;
using AgendaDeAtendimentos.ProfissionalRepository;
using AgendaDeAtendimentos.ProfissionalService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=AgendaApi.db"));

builder.Services.AddScoped<IProfissionalService, ProfissionalService>();
builder.Services.AddScoped<IProfissionalRepository, ProfissionalRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
