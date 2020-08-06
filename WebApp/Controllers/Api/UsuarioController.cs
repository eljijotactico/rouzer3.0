using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Logging;
using Servicios;
using System.Collections.Generic;
using Modelos;
using System.Threading.Tasks;
using System.Net;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers
{
    [ApiController, Route("api/Usuario/{action}/{id?}")]
    public class UsuarioController : ControllerBase 
    {
        private readonly UserManager<UsuarioModel> userManager;
        private readonly SignInManager<UsuarioModel> signInManager;

        #region constructor
        public UsuarioController(
            UserManager<UsuarioModel> userManager,
            SignInManager<UsuarioModel> signInManager
        )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        #endregion

        // [HttpPost]
        // public async Task<ActionResult<Controllers.ApiResponse>> CrearAnonimo()
        // {
        //     var (usuarioResult, id) = await usuarioService.GenerarUsuarioAnonimo();
        //     if(usuarioResult.Succeeded) 
        //     {
        //         bool logueado = await usuarioService.LoguearUsuarioAnonimo(id);
        //         if(logueado)
        //         {
        //             return new ApiResponse("logueado", true, id);
                    
        //         }
        //     }
        //     return new ApiResponse("error", false, usuarioResult.Errors);
        // }
        [HttpPost]
        public async Task<ActionResult> Registro( RegistroVM model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            if(await userManager.Users.AnyAsync(u => u.UserName == model.Nick))
            {
                ModelState.AddModelError("Nick", "El nombre de usuario ya existe");
            }

            UsuarioModel user = new UsuarioModel
            {
                UserName = model.Nick,
            };
            var createResult = await userManager.CreateAsync(user, model.Contraseña);

            if(createResult.Succeeded) 
            {
                await signInManager.SignInAsync(user, true);
                return Redirect("/");

            }
            else {
                return BadRequest(createResult.Errors);
            }
        }
    }
    
    public class RegistroVM
    {
        [MinLength(4, ErrorMessage="Minimo 4 letras")]
        [Required(ErrorMessage="Tienes que escribir un nick padre")]
        [MaxLength(30, ErrorMessage="para la mano")]

        public string Nick { get; set; }
        [MinLength(6, ErrorMessage="Minimo 6 letras")]
        [Required(ErrorMessage="Contraseña requerida")]
        [MaxLength(30, ErrorMessage="para la mano")]
        public string Contraseña { get; set; }
    }
}