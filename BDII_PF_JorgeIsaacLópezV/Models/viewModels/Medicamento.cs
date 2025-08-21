using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    [Table("Medicamentos")]
    public class Medicamento
    {
        [Key]
        [Column("id_medicamento")]
        public int Id_Medicamento { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        [Column("nombre")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        [Column("stock")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El costo por unidad es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo por unidad debe ser mayor a cero")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [Column("costo_unidad", TypeName = "decimal")]
        public decimal Costo_Unidad { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un hospital")]
        [Column("id_hospital")]
        public int Id_Hospital { get; set; }

        [Column("fecha_registro")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime Fecha_Registro { get; set; }

        [Required]
        [Column("activo")]
        public bool Activo { get; set; }


        [ForeignKey("Id_Hospital")]
        public virtual Hospital Hospital { get; set; }

        public virtual ICollection<TratamientoMedicamento> TratamientoMedicamentos { get; set; }

        public Medicamento()
        {
            TratamientoMedicamentos = new HashSet<TratamientoMedicamento>();
            Fecha_Registro = DateTime.Now;
            Activo = true;
            Stock = 0;
        }


        [NotMapped]
        public decimal ValorTotalStock => Stock * Costo_Unidad;

        [NotMapped]
        public bool StockBajo => Stock < 10; 

        [NotMapped]
        public string EstadoStock => StockBajo ? "Stock Bajo" : "Stock Normal";
    }


}