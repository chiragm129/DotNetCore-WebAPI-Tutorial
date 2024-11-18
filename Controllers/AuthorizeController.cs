﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPIProject.Model;
using WebAPIProject.Repos;
using WebAPIProject.Service;

namespace WebAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly LearndataContext context;

        private readonly JwtSettings jwtSettings;
        private readonly IRefreshHandler refresh;
        public AuthorizeController(LearndataContext context, IOptions<JwtSettings> options, IRefreshHandler refresh)
        {
            this.context = context;
            this.jwtSettings = options.Value;
            this.refresh = refresh;
        }

        [HttpPost("GenerateToken")]
         public async Task<IActionResult> GenerateToken([FromBody] UserCred userCred)
         {
                var user = await this.context.Users.FirstOrDefaultAsync(item => item.Code == userCred.username && item.Password == userCred.password);
                if (user != null)
                {
                //genrate token
                    var tokenhandler = new JwtSecurityTokenHandler();
                    var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
                    var tokendesc = new SecurityTokenDescriptor
                    {
                        Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, user.Code),
                            new Claim(ClaimTypes.Role, user.Role)
                        }),
                        Expires = DateTime.UtcNow.AddSeconds(30),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                    };
                    var token = tokenhandler.CreateToken(tokendesc);
                    var finaltoken = tokenhandler.WriteToken(token);
                    return Ok(new TokenResponse() 
                    {
                        Token = finaltoken,
                        RefreshToken = await this.refresh.GenerateToken(userCred.username)
                    });
                }
                else
                {
                    return Unauthorized();
                }
         }

        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateToken([FromBody] TokenResponse token)
        {
            var _refreshtoken = await this.context.Refreshtokens.FirstOrDefaultAsync(item => item.Refreshtoken1 == token.RefreshToken);
            if (_refreshtoken != null)
            {
                //genrate token
                var tokenhandler = new JwtSecurityTokenHandler();
                var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
                SecurityToken securityToken;
                var principal = tokenhandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                }, out securityToken);

                var _token = securityToken as JwtSecurityToken;
                if(_token != null && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string username = principal.Identity?.Name;
                    var _existdata = await this.context.Refreshtokens.FirstOrDefaultAsync(item => item.Userid == username && item.Refreshtoken1 == token.RefreshToken);
                    if( _existdata != null )
                    {
                        var _newtoken = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.securitykey)),
                            SecurityAlgorithms.HmacSha256)
                            );
                        
                        var _finaltoken = tokenhandler.WriteToken(_newtoken);
                        return Ok(new TokenResponse()
                        {
                            Token = _finaltoken,
                            RefreshToken = await this.refresh.GenerateToken(username)
                        });
                    }
                    else
                    {
                        return Unauthorized();
                    }

                }
                else
                {
                    return Unauthorized();
                }

                //var tokendesc = new SecurityTokenDescriptor
                //{
                //    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                //    {
                //            new Claim(ClaimTypes.Name, user.Code),
                //            new Claim(ClaimTypes.Role, user.Role)
                //    }),
                //    Expires = DateTime.UtcNow.AddSeconds(30),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
                //};
                //var token = tokenhandler.CreateToken(tokendesc);
                //var finaltoken = tokenhandler.WriteToken(token);
                //return Ok(new TokenResponse()
                //{
                //    Token = finaltoken,
                //    RefreshToken = await this.refresh.GenerateToken(userCred.username)
                //});
            }
            else
            {
                return Unauthorized();
            }
        }


    }
}
