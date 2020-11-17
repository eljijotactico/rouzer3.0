using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Modelos;

namespace WebApp
{
    public class BanMiddleware
    {
        private readonly RequestDelegate next;

        public BanMiddleware(RequestDelegate next)
        {
            this.next = next;
 
        }

        public async Task Invoke(HttpContext ctx,  RChanContext context, 
            SignInManager<UsuarioModel> sm)
        {
            if(Regex.IsMatch(ctx.Request.Path, @"^/Domado")
) 
            {
                await next(ctx);
                return;
            }
            if(ctx.User != null)
            {
                var banNoVisto = await context.Bans
                    .OrderByDescending(b => b.Expiracion)
                    .Where(b => !b.Visto)
                    .FirstOrDefaultAsync(b => b.UsuarioId == ctx.User.GetId());

                var ahora = DateTime.Now;
                var banActivo = await context.Bans
                    .OrderByDescending(b => b.Expiracion)
                    .Where(b => b.Visto)
                    .Where(b => b.Expiracion > ahora)
                    .FirstOrDefaultAsync(b => b.UsuarioId == ctx.User.GetId());

                if(banNoVisto != null)
                {
                    ctx.Response.Redirect("/Domado");
                }

                if(banActivo != null)
                {
                    await sm.SignOutAsync();
                }
            }
            await next(ctx);
        }
    }
    public static class RozedMiddlwaresExtension
    {
        public static IApplicationBuilder UseBanMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BanMiddleware>();
        }
    }
    
}