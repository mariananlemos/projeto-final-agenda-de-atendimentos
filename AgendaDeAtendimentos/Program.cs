using AgendaDeAtendimentos.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AgendaDeAtendimentos.ProfissionalController;
using AgendaDeAtendimentos.ProfissionalRepository;
using AgendaDeAtendimentos.ProfissionalService;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.OpenApi;
using AgendaDeAtendimentos.Account;
using AgendaDeAtendimentos.AuthenticateService;
using AgendaDeAtendimentos.DTO;
using AgendaDeAtendimentos.Repositories;
using AgendaDeAtendimentos.Services;
using Microsoft.AspNetCore.Mvc;

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

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // padroniza o erro 400 de validação dos DTOs:
        // { "mensagem": "Dados inválidos.", "erros": [ { "campo": "nome", "mensagem": "..." } ] }
        options.InvalidModelStateResponseFactory = context =>
        {
            var resposta = new ErroResposta { Mensagem = "Dados inválidos." };

            foreach(var item in context.ModelState)
            {
                foreach(var erro in item.Value.Errors)
                {
                    // deixa o nome do campo com a primeira letra minúscula, igual ao JSON
                    var campo = item.Key;
                    if(campo.Length > 0)
                    {
                        campo = char.ToLower(campo[0]) + campo.Substring(1);
                    }

                    resposta.Erros.Add(new ErroCampo
                    {
                        Campo = campo,
                        Mensagem = string.IsNullOrEmpty(erro.ErrorMessage) ? "Valor inválido." : erro.ErrorMessage
                    });
                }
            }

            return new BadRequestObjectResult(resposta);
        };
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT atuhorization using the bearer scheme."
        });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("bearer", document)] = []
        });
    }
);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=AgendaApi.db"));

var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<IProfissionalService, ProfissionalService>();
builder.Services.AddScoped<IProfissionalRepository, ProfissionalRepository>();
builder.Services.AddScoped<IAuthenticate, AuthenticateService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IExpedienteRepository, ExpedienteRepository>();
builder.Services.AddScoped<IExpedienteService, ExpedienteService>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<IServicoService, ServicoService>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
