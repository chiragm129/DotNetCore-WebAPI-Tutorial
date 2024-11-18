using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WebAPIProject.Repos;
using WebAPIProject.Repos.Models;
using WebAPIProject.Service;

namespace WebAPIProject.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly LearndataContext context;
        public RefreshHandler(LearndataContext context)
        {
            this.context = context;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using(var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var Existtoken = await this.context.Refreshtokens.FirstOrDefaultAsync(item=> item.Userid== username);
                if(Existtoken != null)
                {
                    Existtoken.Refreshtoken1 = refreshtoken;
                }
                else
                {
                    await this.context.Refreshtokens.AddAsync(new Refreshtoken
                    {
                        Userid = username,
                        Tokenid = new Random().Next().ToString(),
                        Refreshtoken1 = refreshtoken
                    });
                }
                await this.context.SaveChangesAsync();
                return refreshtoken;

            }
        }
    }
}
