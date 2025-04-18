﻿using nguyenthithaoxuan.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Kết nối DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Cấu hình JWT Authentication (cho phép không cần chữ "Bearer")
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };

        // ✅ Cho phép sử dụng token trực tiếp mà không cần tiền tố "Bearer "
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

                // Nếu header KHÔNG bắt đầu bằng "Bearer ", thì gán token thủ công
                if (!string.IsNullOrEmpty(authHeader) && !authHeader.StartsWith("Bearer "))
                {
                    context.Token = authHeader;
                }

                return Task.CompletedTask;
            }
        };
    });

// 3. Cấu hình Swagger hỗ trợ JWT KHÔNG cần "Bearer"
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NguyenThiThaoXuan API", Version = "v1" });

    // 🔐 Định nghĩa security scheme tên "JWT"
    c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "JWT",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập vào token (KHÔNG cần chữ Bearer)."
    });

    // 🔒 Bắt buộc mọi API đều yêu cầu token này
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "JWT"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// 4. Thêm controller
builder.Services.AddControllers();

// 5. Build app
var app = builder.Build();

// 6. Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 7. Middlewares
app.UseHttpsRedirection();
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader()
);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
