using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdmonGrupo4
{
    public class Departamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public Departamento(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Nombre: {Nombre}";
        }
    }
}
