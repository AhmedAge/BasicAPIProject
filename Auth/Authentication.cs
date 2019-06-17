﻿using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using BasicAPIProject.Models;

namespace BasicAPIProject.Auth
{
    public enum RequestType
    {
        POST,
        GET,
        PUT,
        DELETE
    }
    public static class Authentication
    {

        private const string Secret = "AG3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hTcMw==";

        public static string GenerateToken(string username, int expireMinutes = 20)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                        new Claim(ClaimTypes.Name, username)
                    }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }


        public static async Task<bool> IsAuthentication(HttpRequestMessage request)
        {
            string token = request.Headers.GetValues("access_token").First();
            string email = request.Headers.GetValues("email").First();
            if (token == null)
            {
                return false;
            }
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                var data = await DB.TokenChecks.Where(x => x.email == email &&
                 x.token == token && x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now).FirstOrDefaultAsync();

                if (data == null)
                    return false;
                else
                    return true;
            }
        }

        public static async Task<bool> UpdateTokenPostCheck(HttpRequestMessage request, RequestType type)
        {
            string token = request.Headers.GetValues("access_token").First();
            string email = request.Headers.GetValues("email").First();
            string guid = request.Headers.GetValues("guid").First();

            if (token == null)
            {
                return false;
            }
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                var data = await DB.TokenChecks.Where(x => x.email == email &&
                 x.token == token && x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now && x.guidValidate == guid).FirstOrDefaultAsync();

                if (data == null)
                    return false;
                else
                    return true;
            }
        }

        public static async Task<bool> UpdateTokenPost(HttpRequestMessage request,string guid)
        {
            string token = request.Headers.GetValues("access_token").First();
            string email = request.Headers.GetValues("email").First(); 

            if (token == null || email == null || guid == null)
            {
                return false;
            }
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {
                var data = await DB.TokenChecks.Where(x => x.email == email &&
                 x.token == token && x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now).FirstOrDefaultAsync();

                if (data == null)
                    return false;
                else
                {
                    data.guidValidate = guid;
                    DB.SaveChanges();

                    return true;
                }
            }
        }
    }
}