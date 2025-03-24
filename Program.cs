using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdmonGrupo4
{
    internal class Program
    {
        static void Main()
        {
            DataBase bd = new DataBase();
            Auth auth = new Auth();
            SimuladorConcurrencia simulador = new SimuladorConcurrencia();
            bool ejecutando = true;
            bd.HacerCheckpoint();

            while (ejecutando)
            {
                if (auth.GetAuthenticatedUser == null)
                {
                    Console.WriteLine("\n------ Autenticación ------");
                    Console.WriteLine("1. Iniciar sesión");
                    Console.WriteLine("2. Registrarse");

                    string opcion = Console.ReadLine();
                    switch (opcion)
                    {
                        case "1":
                            Console.WriteLine("\n------ Inicio de sesión ------");
                            Console.Write("Ingresa tu nombre: ");
                            string name = Console.ReadLine();
                            Console.Write("Ingresa tu contraseña: ");
                            string password = Console.ReadLine();

                            auth.Login(name, password);
                            break;

                        case "2":
                            Console.WriteLine("\n------ Registro ------");
                            Console.Write("Ingresa tu nombre: ");
                            string newUserName = Console.ReadLine();
                            Console.Write("Ingresa tu contraseña: ");
                            string newUserPassword = Console.ReadLine();
                            int lastId = auth.GetLastId;

                            User newUser = new User(lastId + 1, newUserName, newUserPassword);
                            auth.SignUp(newUser);
                            break;

                        default:
                            Console.WriteLine("Opción inválida");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\nSeleccione una opción:");
                    Console.WriteLine("-------EMPLEADOS-------");
                    Console.WriteLine("1. Insertar Empleado");
                    Console.WriteLine("2. Actualizar Empleado");
                    Console.WriteLine("3. Eliminar Empleado");
                    Console.WriteLine("4. Mostrar Empleados");
                    Console.WriteLine("-------DEPARTAMENTO-------");
                    Console.WriteLine("5. Insertar Departamento");
                    Console.WriteLine("6. Actualizar Departamento");
                    Console.WriteLine("7. Eliminar Departamento");
                    Console.WriteLine("8. Mostrar Departamento");
                    Console.WriteLine("-------ACCIONES-------");
                    Console.WriteLine("9. Guardar Checkpoint");
                    Console.WriteLine("10. Rollback (Deshacer/Undo)");
                    Console.WriteLine("11. Reaplicar Transacciones");
                    Console.WriteLine("12. Simular fallo");
                    Console.WriteLine("-------CONCURRENCIA-------");
                    Console.WriteLine("13. Simular Técnicas de Concurrencia");
                    Console.WriteLine("14. Salir");
                    Console.Write("Opción: ");

                    string opcion = Console.ReadLine();
                    switch (opcion)
                    {
                        case "1":
                            Console.Write("ID: "); int id = int.Parse(Console.ReadLine());
                            Console.Write("Nombre: "); string nombre = Console.ReadLine();
                            Console.Write("ID Departamento: "); int idDep = int.Parse(Console.ReadLine());
                            bd.InsertarEmpleado(id, nombre, idDep);
                            break;
                        case "2":
                            Console.Write("ID Empleado: "); int idAct = int.Parse(Console.ReadLine());
                            Console.Write("Nuevo Nombre: "); string nuevoNombre = Console.ReadLine();
                            bd.ActualizarEmpleado(idAct, nuevoNombre);
                            break;
                        case "3":
                            Console.Write("ID Empleado: "); int idEmpDel = int.Parse(Console.ReadLine());
                            bd.EliminarEmpleado(idEmpDel);
                            break;
                        case "4":
                            bd.MostrarEmpleados();
                            break;
                        case "5":
                            Console.Write("ID: "); int depId = int.Parse(Console.ReadLine());
                            Console.Write("Nombre: "); string nombreDep = Console.ReadLine();
                            bd.InsertarDepartamento(depId, nombreDep);
                            break;
                        case "6":
                            Console.Write("ID Departamento: "); int deptoId = int.Parse(Console.ReadLine());
                            Console.Write("Nuevo Nombre: "); string newDepName = Console.ReadLine();
                            bd.ActualizarDepartamento(deptoId, newDepName);
                            break;
                        case "7":
                            Console.Write("ID Departamento: "); int idDepDel = int.Parse(Console.ReadLine());
                            bd.EliminarDepartamento(idDepDel);
                            break;
                        case "8":
                            bd.MostrarDepartamentos();
                            break;
                        case "9":
                            bd.HacerCheckpoint();
                            break;
                        case "10":
                            bd.Rollback();
                            break;
                        case "11":
                            bd.ReaplicarTransacciones();
                            break;
                        case "12":
                            bd.SimularFallo();
                            break;
                        case "13":
                            Console.WriteLine("\n------ Simulación de Técnicas de Concurrencia ------");
                            simulador.SimularTransacciones();
                            Console.WriteLine("\nPresiona Enter para continuar...");
                            Console.ReadLine();
                            break;
                        case "14":
                            ejecutando = false;
                            break;
                        default:
                            Console.WriteLine("Opción inválida.");
                            break;
                    }
                }
            }
        }
    }
}