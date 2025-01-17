using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Modelos;
using Newtonsoft.Json;

namespace Modelos
{
    public class ComentarioViewModel
    {
        private static readonly MD5 md5 = MD5.Create();
        public ComentarioViewModel(ComentarioModel comentario)
        {
            this.Contenido = comentario.Contenido;
            this.Id = comentario.Id;
            this.Creacion = comentario.Creacion;
            this.Media = comentario.Media;
            this.Nombre = comentario?.Nombre ?? "";
            this.Rango = comentario.Rango;
            this.Audio = comentario.Audio;
            this.Sticky = comentario.Sticky;
            this.Ignorado = comentario.Ignorado;
        }

        public ComentarioViewModel(ComentarioModel comentario, HiloModel hilo = null, string requestUsuarioId = null)
        {
            this.Contenido = comentario.Contenido;
            this.Id = comentario.Id;
            this.Creacion = comentario.Creacion;
            this.Media = comentario.Media;
            this.Nombre = comentario?.Nombre ?? "";
            this.Rango = comentario.Rango;
            this.Propio = requestUsuarioId == comentario.UsuarioId;
            this.Audio = comentario.Audio;
            this.Sticky = comentario.Sticky;
            this.Ignorado = comentario.Ignorado;

            if (hilo != null)
            {
                this.EsOp = comentario.UsuarioId == hilo.UsuarioId;
                this.OP = requestUsuarioId == hilo.UsuarioId;

                if (hilo.Flags.Contains("d"))
                {
                    var random = new Random(comentario.Creacion.Millisecond + Creacion.Second);
                    this.Dados = random.Next(10);
                }
                if (hilo.Flags.Contains("i") || hilo.Flags.Contains("s"))
                {
                    IdUnico = GenerarIdUnico(hilo.Id, comentario.UsuarioId);
                }
                if (!hilo.Flags.Contains("b") && !hilo.Flags.Contains("s"))
                {
                    this.Banderita = comentario.Pais;
                    if (!string.IsNullOrEmpty(comentario.Nombre) || comentario.Rango != CreacionRango.Anon)
                    {
                        this.Banderita = null;
                    }
                }
            }
            Color = CalcularColor(hilo);

            var creadorId = comentario.UsuarioId;
            // var r = new Random(BitConverter.GetBytes(creadorId[0])[0]);
            // Banderita = "fi";
            // // Banderita =  r.Next(7) switch
            // //     {
            // //         0  => "ar",
            // //         1 => "bo",
            // //         2 => "cl",  
            // //         3 => "mx",
            // //         4 => "il",
            // //         5 => "uy",
            // //         6 => "ve",
            // //         _ => "",
            // //     };
            // if(r.Next(10) == 5) Banderita = r.Next(4) switch
            //     {
            //         0  => "il",
            //         1 => "il",
            //         2 => "il",  
            //         3 => "il",
            //         _ => "",
            //     };
        }
        public ComentarioViewModel() { }

        public string Id { get; set; }
        public string Contenido { get; set; }
        public DateTimeOffset Creacion { get; set; }
        public bool EsOp { get; set; }
        public MediaModel Media { get; set; }
        public AudioModel Audio { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue("")]
        public string Nombre { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(0)]
        public CreacionRango Rango { get; set; }

        public string IdUnico { get; set; } = "";

        public string Color { get; set; } = "naranja";
        public bool Sticky { get; set; } = false;
        public bool OP { get; set; } = false;
        public bool Ignorado { get; set; } = false;

        private string CalcularColor(HiloModel hilo = null)
        {
            var r = new Random(Creacion.Millisecond + Creacion.Second * 60 + Creacion.Minute * 60 * 60 + Creacion.Hour * 60 * 60 * 60 + +Creacion.Day * 60 * 60 * 60 * 24);

            //Serio
            if (hilo != null && hilo.Flags.Contains("s")) return "serio";

            // Black
            const int categoriaParanormal = 15;
            if (hilo != null && hilo.CategoriaId == categoriaParanormal)
            {
                // var paraguayTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Paraguay Standard Time");
                var horaParaguay = Creacion.ToUniversalTime().AddHours(-3);

                if (horaParaguay.Hour < 7 && horaParaguay.Hour >= 0 && r.Next(1000) == 666)
                    return "negro";
            }

            if (r.Next(10000) == 9)
            {
                r.Next();
                return r.Next(4) switch
                {
                    0 => "rose-violeta",
                    1 => "rose-castaña",
                    2 => "rose-azul",
                    3 => "rose-rubia",
                    _ => "",
                };
            }
            if (r.Next(10000) == 10) return "navideño";
            if (r.Next(5000) == 13) return "blanco";
            if (r.Next(2000) == 11) return "ario";

            if (r.Next(200) == 2)
            {
                return r.Next(3) switch
                {
                    0 => "marron",
                    1 => "rosa",
                    2 => "invertido",
                    _ => "",
                };
            }

            if (r.Next(20) == 10) return "multi";


            return r.Next(4) switch
            {
                0 => "amarillo",
                1 => "azul",
                2 => "rojo",
                3 => "verde",
                _ => "",
            };
        }

        public int Dados { get; set; } = -1;
        public string Banderita { get; set; }
        public bool Propio { get; set; }

        static protected string GenerarIdUnico(string hiloId, string usuarioId)
        {
            var random = new Random((hiloId + usuarioId).GetHashCode());
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 3)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private int HashString(string str)
        {
            var hashed = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var ivalue = BitConverter.ToInt32(hashed, 0);
            return ivalue;
        }

    }
    public class ComentarioViewModelMod : ComentarioViewModel
    {
        public ComentarioViewModelMod() { }
        public ComentarioViewModelMod(ComentarioModel comentario) : base(comentario)
        {
            UsuarioId = comentario.UsuarioId;
        }
        public ComentarioViewModelMod(ComentarioModel comentario, HiloModel hilo, string clientUsuarioId = null) : base(comentario, hilo, clientUsuarioId)
        {
            UsuarioId = comentario.UsuarioId;
            Estado = comentario.Estado;
        }

        public string HiloId { get; set; }
        public string UsuarioId { get; set; }
        public ComentarioEstado Estado { get; set; }
        public string Username { get; set; }
        public UsuarioModel Usuario { get; set; }
    }
}