﻿using System;
using System.Linq;
using System.Web.Http;
using System.Threading.Tasks;
using BasicAPIProject.Models;
using BasicAPIProject.Auth; 
using System.Data.Entity;
using wepapiAuth.Auth;

namespace BasicAPIProject.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost]
        //[Route("api/Auth/Login")]
        public async Task<AuthToken> Login()
        {
            Login login = new Models.Login
            {
                email = Request.Headers.GetValues("email").First(),
                password = Request.Headers.GetValues("password").First()
            };

            //Check for user in the Database
            using (NORTHWNDEntities DB = new NORTHWNDEntities())
            {

                try
                {
                    var data = await DB.Sec_Users.Where(x => x.email == login.email && x.password == login.password).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        var token = await DB.TokenChecks.Where(x => x.email == login.email).ToListAsync();
                        DB.TokenChecks.RemoveRange(token);
                        AuthToken auth = new AuthToken
                        {
                            email = login.email,
                            access_token = Authentication.GenerateToken(login.email)
                        };

                        TokenCheck ok = new TokenCheck
                        {
                            email = auth.email,
                            token = auth.access_token,
                            StartTime = DateTime.Now,
                            EndTime = DateTime.Now.AddMinutes(30)
                        };
                        DB.TokenChecks.Add(ok);
                        await DB.SaveChangesAsync();


                        return auth;
                    }
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


        }


        [HttpOptions]
        public string CheckLogin()
        {
            return string.Empty;
        }
    }
}
