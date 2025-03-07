using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdmonGrupo4
{
    public class DataBase
    {

        private List<Empleado> empleados = new List<Empleado>();
        private List<Departamento> departamentos = new List<Departamento>();
        private Stack<List<Empleado>> checkpointStack = new Stack<List<Empleado>>();
        private List<string> logTransacciones = new List<string>();
        private Stack<List<Departamento>> checkpointDepartamentosStack = new Stack<List<Departamento>>();
        private Stack<List<string>> checkpointLogStack = new Stack<List<string>>();

        public void InsertarEmpleado(int id, string nombre, int idDepartamento)
        {
            if (!departamentos.Any(d => d.Id == idDepartamento))
            {
                Console.WriteLine("Error: No existe el departamento.");
                return;
            }
            empleados.Add(new Empleado(id, nombre, idDepartamento));
            logTransacciones.Add($"INSERT {id} {nombre} {idDepartamento}");
        }

        public void ActualizarEmpleado(int id, string nuevoNombre)
        {
            var emp = empleados.FirstOrDefault(e => e.Id == id);
            if (emp != null)
            {
                emp.Nombre = nuevoNombre;
                logTransacciones.Add($"UPDATE {id} {nuevoNombre}");
            }
        }

        public void EliminarEmpleado(int id)
        {
            empleados.RemoveAll(e => e.Id == id);
            logTransacciones.Add($"DELETE {id}");
        }

        public void MostrarEmpleados()
        {
            Console.WriteLine("\nEmpleados:");
            empleados.ForEach(Console.WriteLine);
        }

        public void InsertarDepartamento(int id, string nombre)
        {
            var nuevoDepartamento = new Departamento(id, nombre);
            departamentos.Add(nuevoDepartamento);
            logTransacciones.Add($"INSERT DEPARTAMENTO {id} {nombre}");
            Console.WriteLine($"Departamento agregado: {nuevoDepartamento}");
        }

        public void ActualizarDepartamento(int id, string nuevoNombre)
        {
            var dep = departamentos.FirstOrDefault(e => e.Id == id);
            if (dep != null)
            {
                dep.Nombre = nuevoNombre;
                logTransacciones.Add($"UPDATE {id} {nuevoNombre}");
            }
        }

        public void EliminarDepartamento(int id)
        {
            departamentos.RemoveAll(e => e.Id == id);
            logTransacciones.Add($"DELETE {id}");
        }

        public void MostrarDepartamentos()
        {
            Console.WriteLine("\nDepartamentos:");
            foreach (var dep in departamentos)
            {
                Console.WriteLine(dep);
            }
        }

        public void MostrarLogTransacciones()
        {
            Console.WriteLine("\nLog de Transacciones:");
            foreach (var log in logTransacciones)
            {
                Console.WriteLine(log);
            }
        }

        public void HacerCheckpoint()
        {
            checkpointStack.Push(new List<Empleado>(empleados));
            checkpointDepartamentosStack.Push(new List<Departamento>(departamentos));
            checkpointLogStack.Push(new List<string>(logTransacciones));

            Console.WriteLine("Checkpoint guardado.");
        }

        public void Rollback()
        {
            if (checkpointStack.Count > 0 && checkpointDepartamentosStack.Count > 0 && checkpointLogStack.Count > 0)
            {
                empleados = checkpointStack.Pop();
                departamentos = checkpointDepartamentosStack.Pop();
                logTransacciones = checkpointLogStack.Pop();

                Console.WriteLine("Se restauró el último checkpoint.");
            }
            else
            {
                Console.WriteLine("No hay checkpoints guardados");
            }
        }

        public void ReaplicarTransacciones()
        {
            Console.WriteLine("Reaplicando transacciones...");
            foreach (var log in logTransacciones)
            {
                Console.WriteLine(log);
            }
        }

    }
}
