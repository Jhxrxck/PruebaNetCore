using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PruebaNetCore_Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Nombre del Producto es requerido")]
        public string NombreProducto { get; set; }

        [Required(ErrorMessage = "Descripcion Corta es requerido")]
        public string DescripcionCorta { get; set; }

        [Required(ErrorMessage = "Descripcion del Producto es requerido")]
        public string DescripcionProducto { get; set; }

        [Required(ErrorMessage = "El Precio del Producto es requerido")]
        [Range(1, int.MaxValue, ErrorMessage ="El precio debe ser mayor a cero.")]
        public double Precio { get; set; }

        public string? ImagenUrl { get; set; }
        //Foreign Key
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        public int TipoAplicacionId { get; set; }
        [ForeignKey("TipoAplicacionId")]
        public virtual TipoAplicacion? TipoAplicacion { get; set; }

       
    }

}
