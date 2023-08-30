﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GameApp.Mvc.Middlewares
{
    public class UnauthorizedMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfiguration _configuration;

        public UnauthorizedMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("token", out string token) && context.Request.Path != "/Account/Login")
            {
                if (!AttachUserToContext(context, token))
                {
                    context.Response.Redirect("/Account/Login");
                }
            }

            if (!context.Request.Cookies.TryGetValue("token", out string token2) && context.Request.Path != "/Account/Login")
            {
                context.Response.Redirect("/Account/Login");
            }

            await next(context);
        }

        private bool AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid");

                return false;
            }
        }
    }
}