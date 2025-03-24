using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using Newtonsoft.Json;

namespace ProyectoAdmonGrupo4
{
    public class DataBase
    {
        private List<Empleado> empleados = new List<Empleado>();
        private List<Departamento> departamentos = new List<Departamento>();
        private Stack<List<Empleado>> checkpoinEmpleadotStack = new Stack<List<Empleado>>();
        private Stack<List<Departamento>> checkpointDepartamentosStack = new Stack<List<Departamento>>();
        private HashSet<int> idsActualizados = new HashSet<int>(); //MAPEAR LOS REGISTROS QUE FUERON ACTUALIZADO
        private Stack<List<string>> checkpointLogStack = new Stack<List<string>>();
        private List<TransactionLog> logTransacciones = new List<TransactionLog>();
        //private List<string> transacciones;//VARIABLE PARA TRANSACCIONES LOCALES
        private string empleadosJSON = "empleados_checkpoint.json";
        private string departamentosJSON = "departamentos_checkpoint.json";
        private string logFilePath = "logTransacciones.json";
        private Auth auth;

        public DataBase(Auth authInstance)
        {
            this.auth = authInstance;
            empleados = CargarDatos<List<Empleado>>(empleadosJSON) ?? new List<Empleado>();
            departamentos = CargarDatos<List<Departamento>>(departamentosJSON) ?? new List<Departamento>();
        }
        private string ObtenerTimestamp() => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        private T CargarDatos<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default(T);
        }
        private void GuardarDatos<T>(string filePath, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public void InsertarEmpleado(int id, string nombre, int idDepartamento)
        {
            string usuario = auth.GetAuthenticatedUserName();
            if (!departamentos.Any(d => d.Id == idDepartamento))
            {
                Console.WriteLine("Error: No existe el departamento.");
                return;
            }
            empleados.Add(new Empleado(id, nombre, idDepartamento));
            //transacciones.Add($"INSERT {id} {nombre} {idDepartamento}");
            var log = new TransactionLog(ObtenerTimestamp(), usuario, "INSERT", $"Empleado: {id}, {nombre}, {idDepartamento}");
            logTransacciones.Add(log);
            GuardarLogEnArchivo();
        }
        private List<Empleado> ClonarListaEmpleados(List<Empleado> lista)
        {
            return lista.Select(e => new Empleado(e.Id, e.Nombre, e.IdDepartamento)).ToList();
        }

        private List<Departamento> ClonarListaDepartamentos(List<Departamento> lista)
        {
            return lista.Select(d => new Departamento(d.Id, d.Nombre)).ToList();
        }

        public void ActualizarEmpleado(int id, string nuevoNombre)
        {
            string usuario = auth.GetAuthenticatedUserName();
            var emp = empleados.FirstOrDefault(e => e.Id == id);
            if (emp != null)
            {
                emp.Nombre = nuevoNombre;
                //transacciones.Add($"UPDATE {id} {nuevoNombre}");
                Console.WriteLine($"Empleado actualizado...");
                var log = new TransactionLog(ObtenerTimestamp(), usuario, "UPDATE", $"Empleado: {id}, {nuevoNombre}");
                logTransacciones.Add(log);
                GuardarLogEnArchivo();
            }
        }

        public void EliminarEmpleado(int id)
        {
            string usuario = auth.GetAuthenticatedUserName();
            empleados.RemoveAll(e => e.Id == id);
            //transacciones.Add($"DELETE {id}");
            Console.WriteLine($"Empleado eliminado...");
            var log = new TransactionLog(ObtenerTimestamp(), usuario, "DELETE", $"Empleado: {id}");
            logTransacciones.Add(log);
            GuardarLogEnArchivo();
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
            string usuario = auth.GetAuthenticatedUserName();

            var nuevoDepartamento = new Departamento(id, nombre);
            departamentos.Add(nuevoDepartamento);
            //transacciones.Add($"INSERT DEPARTAMENTO {id} {nombre}");
            Console.WriteLine($"Departamento agregado: {nuevoDepartamento}");
            var log = new TransactionLog(ObtenerTimestamp(), usuario, "INSERT", $"Departamento: {id}, {nombre}");
            logTransacciones.Add(log);
            GuardarLogEnArchivo();
        }
        public void ActualizarDepartamento(int id, string nuevoNombre)
        {
            string usuario = auth.GetAuthenticatedUserName();
            var dep = departamentos.FirstOrDefault(d => d.Id == id);
            if (dep != null)
            {
                dep.Nombre = nuevoNombre;
                //transacciones.Add($"UPDATE {id} {nuevoNombre}");
                Console.WriteLine($"Departamento actualizado...");
                var log = new TransactionLog(ObtenerTimestamp(), usuario, "UPDATE", $"Departamento: {id}, {nuevoNombre}");
                logTransacciones.Add(log);
                GuardarLogEnArchivo();
            }
        }

        public void EliminarDepartamento(int id)
        {
            string usuario = auth.GetAuthenticatedUserName();
            departamentos.RemoveAll(d => d.Id == id);
            //transacciones.Add($"DELETE {id}");
            Console.WriteLine($"Departamento eliminado...");
            var log = new TransactionLog(ObtenerTimestamp(), usuario, "DELETE", $"Departamento: {id}");
            logTransacciones.Add(log);
            GuardarLogEnArchivo();
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
            checkpoinEmpleadotStack.Push(ClonarListaEmpleados(empleados));
            checkpointDepartamentosStack.Push(ClonarListaDepartamentos(departamentos));
            //checkpointLogStack.Push(new List<string>(transacciones));
            GuardarDatos("empleados_checkpoint.json", empleados);
            GuardarDatos("departamentos_checkpoint.json", departamentos);
            Console.WriteLine("Checkpoint guardado correctamente.");
        }

        public void Rollback()
        {
            if (checkpoinEmpleadotStack.Count > 0 && checkpointDepartamentosStack.Count > 0 && checkpointLogStack.Count > 0)
            {
                empleados = ClonarListaEmpleados(checkpoinEmpleadotStack.Pop());
                departamentos = ClonarListaDepartamentos(checkpointDepartamentosStack.Pop());
                //transacciones = new List<string>(checkpointLogStack.Pop());
                GuardarDatos(empleadosJSON, empleados);
                GuardarDatos(departamentosJSON, departamentos);
                //GuardarDatos(logFilePath, transacciones);
                Console.WriteLine("Se restauró el último checkpoint correctamente.");
            }
            else
            {
                Console.WriteLine("No hay checkpoints guardados.");
            }
        }
        public void ReaplicarTransacciones()
        {
            Console.WriteLine("Reaplicando transacciones...");
            Console.WriteLine("Reaplicando empleados...");
            ReaplicarEmpleados();
            Console.WriteLine("Reaplicando departamentos...");
            ReaplicarDepartamentos();
            Console.WriteLine("Reaplicación completa.");
        }

        private void ReaplicarEmpleados()
        {
            // Verifica los empleados en el checkpoint
            var empleadosCheckpoint = ClonarListaEmpleados(checkpoinEmpleadotStack.Peek());

            // Reaplica los empleados insertados
            foreach (var empleado in empleadosCheckpoint)
            {
                var empExistente = empleados.FirstOrDefault(e => e.Id == empleado.Id);
                if (empExistente == null)
                {
                    // Si el empleado no existe en el estado actual, lo agregamos
                    empleados.Add(empleado);
                    Console.WriteLine($"Reinsertado Empleado: {empleado.Id}, {empleado.Nombre}");
                }
                else if (empExistente.Nombre != empleado.Nombre)
                {
                    // Si el empleado existe pero su nombre ha cambiado, lo actualizamos
                    empExistente.Nombre = empleado.Nombre;
                    Console.WriteLine($"Actualizado Empleado: {empleado.Id}, {empleado.Nombre}");
                }
            }

            // Eliminar empleados que estaban en el checkpoint pero no existen en el estado actual
            var empleadosParaEliminar = empleados.Where(e => !empleadosCheckpoint.Any(ee => ee.Id == e.Id)).ToList();
            foreach (var emp in empleadosParaEliminar)
            {
                empleados.Remove(emp);
                Console.WriteLine($"Eliminado Empleado: {emp.Id}");
            }
        }

        private void ReaplicarDepartamentos()
        {
            // Verifica los departamentos en el checkpoint
            var departamentosCheckpoint = ClonarListaDepartamentos(checkpointDepartamentosStack.Peek());

            // Reaplica los departamentos insertados
            foreach (var departamento in departamentosCheckpoint)
            {
                var depExistente = departamentos.FirstOrDefault(d => d.Id == departamento.Id);
                if (depExistente == null)
                {
                    // Si el departamento no existe en el estado actual, lo agregamos
                    departamentos.Add(departamento);
                    Console.WriteLine($"Reinsertado Departamento: {departamento.Id}, {departamento.Nombre}");
                }
                else if (depExistente.Nombre != departamento.Nombre)
                {
                    // Si el departamento existe pero su nombre ha cambiado, lo actualizamos
                    depExistente.Nombre = departamento.Nombre;
                    Console.WriteLine($"Actualizado Departamento: {departamento.Id}, {departamento.Nombre}");
                }
            }

            // Eliminar departamentos que estaban en el checkpoint pero no existen en el estado actual
            var departamentosParaEliminar = departamentos.Where(d => !departamentosCheckpoint.Any(dd => dd.Id == d.Id)).ToList();
            foreach (var dep in departamentosParaEliminar)
            {
                departamentos.Remove(dep);
                Console.WriteLine($"Eliminado Departamento: {dep.Id}");
            }
        }

        public void SimularFallo()
        {
            Console.WriteLine("Simulando un fallo del sistema...");
            empleados.Clear();
            departamentos.Clear();
            Console.WriteLine("Se ha producido un fallo y los datos en memoria han sido eliminados.");
        }
        private void GuardarLogEnArchivo()
        {
            string json = System.Text.Json.JsonSerializer.Serialize(logTransacciones, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(logFilePath, json);
        }

    }
}
