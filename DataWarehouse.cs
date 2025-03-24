using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace ProyectoAdmonGrupo4
{
    public class DataWarehouse
    {
        private string empleadosCheckpointFile = "empleados_checkpoint.json";
        private string departamentosCheckpointFile = "departamentos_checkpoint.json";
        private string dataWarehouseFile = "datawarehouse.json";
        private string logDataWarehouseFile = "logDataWarehouse.json";

        public void GenerarDataWarehouse(string usuario)
        {

            List<Empleado> empleados = CargarDatos<List<Empleado>>(empleadosCheckpointFile) ?? new List<Empleado>();
            List<Departamento> departamentos = CargarDatos<List<Departamento>>(departamentosCheckpointFile) ?? new List<Departamento>();

            var warehouseData = (from emp in empleados
                                 join dep in departamentos on emp.IdDepartamento equals dep.Id
                                 select new
                                 {
                                     EmpleadoId = emp.Id,
                                     EmpleadoNombre = emp.Nombre,
                                     DepartamentoId = dep.Id,
                                     DepartamentoNombre = dep.Nombre
                                 }).ToList();

            GuardarDatos(dataWarehouseFile, warehouseData);

            string logEntry = $"DataWarehouse generado por {usuario} en {DateTime.Now}";
            List<string> logEntries = CargarDatos<List<string>>(logDataWarehouseFile) ?? new List<string>();
            logEntries.Add(logEntry);
            GuardarDatos(logDataWarehouseFile, logEntries);

            Console.WriteLine("DataWarehouse generado exitosamente.");
        }

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
    }
}
