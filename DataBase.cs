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
            if (!(empleados.Count() > 0))
            {
                Console.WriteLine("\nNo hay empleados guardados.");
            }
            else
            {
                Console.WriteLine("\nEmpleados:");
                empleados.ForEach(Console.WriteLine);
            }
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
            if (!(departamentos.Count() > 0))
            {
                Console.WriteLine("\nNo hay departamentos guardados.");
            }
            else
            {
                Console.WriteLine("\nDepartamentos:");
                foreach (var dep in departamentos)
                {
                    Console.WriteLine(dep);
                }
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
                string[] partes = log.Split(' ');
                string operacion = partes[0];

                switch (operacion)
                {
                    case "INSERT":
                        if (partes[1] == "DEPARTAMENTO")
                        {
                            int idDep = int.Parse(partes[2]);
                            string nombreDep = partes[3];
                            if (!departamentos.Any(d => d.Id == idDep))
                            {
                                departamentos.Add(new Departamento(idDep, nombreDep));
                                Console.WriteLine($"Reinsertado Departamento: {idDep}, {nombreDep}");
                            }
                        }
                        else
                        {
                            int idEmp = int.Parse(partes[1]);
                            string nombreEmp = partes[2];
                            int idDep = int.Parse(partes[3]);
                            if (!empleados.Any(e => e.Id == idEmp))
                            {
                                empleados.Add(new Empleado(idEmp, nombreEmp, idDep));
                                Console.WriteLine($"Reinsertado Empleado: {idEmp}, {nombreEmp}, {idDep}");
                            }
                        }
                        break;

                    case "UPDATE":
                        int idUpdate = int.Parse(partes[1]);
                        string nuevoNombre = partes[2];
                        var empleado = empleados.FirstOrDefault(e => e.Id == idUpdate);
                        if (empleado != null)
                        {
                            empleado.Nombre = nuevoNombre;
                            Console.WriteLine($"Actualizado Empleado: {idUpdate}, {nuevoNombre}");
                        }
                        else
                        {
                            var departamento = departamentos.FirstOrDefault(d => d.Id == idUpdate);
                            if (departamento != null)
                            {
                                departamento.Nombre = nuevoNombre;
                                Console.WriteLine($"Actualizado Departamento: {idUpdate}, {nuevoNombre}");
                            }
                        }
                        break;

                    case "DELETE":
                        int idDelete = int.Parse(partes[1]);
                        empleados.RemoveAll(e => e.Id == idDelete);
                        departamentos.RemoveAll(d => d.Id == idDelete);
                        Console.WriteLine($"Eliminado ID: {idDelete}");
                        break;
                }
            }
            Console.WriteLine("Transacciones reaplicadas correctamente.");
        }
        public void SimularFallo()
        {
            Console.WriteLine("Simulando un fallo del sistema...");
            empleados.Clear();
            departamentos.Clear();
            Console.WriteLine("Se ha producido un fallo y los datos en memoria han sido eliminados.");
        }
    }
}
