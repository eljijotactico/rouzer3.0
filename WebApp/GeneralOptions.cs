using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebApp
{
    public class GeneralOptions : GeneralPublicOptions {
        public int LimiteBump { get; set; }

        public int HilosMaximosPorCategoria { get; set; }
        public bool ModoPrivado { get; set; }

        public async Task Guardar(string ubicacion) {
            var configActualizado = new {
                General = this
            };
            await File.WriteAllTextAsync(ubicacion, JsonSerializer.Serialize(configActualizado, new JsonSerializerOptions{
                WriteIndented = true,
            }));
        }
    }

    public class GeneralPublicOptions
    {
        public int TiempoEntreComentarios { get; set; }
        public int TiempoEntreHilos { get; set; }

        public int LimiteArchivo { get; set; }

        public bool RegistroAbierto { get; set; }

        public bool CaptchaHilo { get; set; }
        public bool CaptchaComentario { get; set; }
        public bool CaptchaRegistro { get; set; }

    }

}