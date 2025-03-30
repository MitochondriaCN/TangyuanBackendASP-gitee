/*
 * �ڿ�ѧ��û��ƽ̹�Ĵ����
 * ֻ�в�η�Ϳ����Ŷ���ɽ·�ʵǵ��ˣ�
 * ����ϣ���ﵽ��ԵĶ��㡣
 * �������������˼
 */

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TangyuanBackendASP.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    // ���� Swagger ֧�� JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "������ JWT ���ƣ���ʽ��Bearer {token}��",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//JWT��֤����
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<AuthService>();

//���������£�����Զ��MySQL
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<TangyuanDbContext>(
        options => options.UseMySQL(builder.Configuration.GetConnectionString("Remote")));
}
//���������£����ӱ���MySQL
else
{
    builder.Services.AddDbContext<TangyuanDbContext>(
        options => options.UseMySQL(builder.Configuration.GetConnectionString("Local")));
}


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TangyuanDbContext>();
    dbContext.Database.EnsureCreated(); // Alternatively, use this to ensure the database is created
}

app.UseSwagger();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

//��ԭ�����޵�

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//�����Ŀ¼�����ڣ�����
if (!Directory.Exists("wwwroot"))
{
    Directory.CreateDirectory("wwwroot");
}
//���images�ļ��в����ڣ�����
if (!Directory.Exists("wwwroot/images"))
{
    Directory.CreateDirectory("wwwroot/images");
}

app.Run();
