using System.ComponentModel.DataAnnotations;

namespace PruebaNetCore_Modelos
{
    public class TipoAplicacion
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="El Nombre de Tipo Aplicación es Obligatorio.")]
        public string Nombre { get; set; }
    }
}
