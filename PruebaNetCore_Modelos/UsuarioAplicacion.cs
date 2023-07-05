using Microsoft.AspNetCore.Identity;

namespace PruebaNetCore_Modelos
{
    public class UsuarioAplicacion :IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}
