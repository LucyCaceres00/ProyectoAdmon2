using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdmonGrupo4
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdDepartamento { get; set; } // Clave foránea

        public Empleado(int id, string nombre, int idDepartamento)
        {
            Id = id;
            Nombre = nombre;
            IdDepartamento = idDepartamento;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Nombre: {Nombre}, Departamento ID: {IdDepartamento}";
        }
    }
}
