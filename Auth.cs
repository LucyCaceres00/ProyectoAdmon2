using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdmonGrupo4
{
    public class Auth
    {
        private List<User> users;
        private User UserAuthenticated { get; set; }
        private List<string> logAutenticacion;
        private string logAutenticacionFilePath = "logAutenticacion.json";
        private string usersFilePath = "users.json";

        public Auth()
        {
            users = CargarDatos<List<User>>(usersFilePath) ?? new List<User>();
            logAutenticacion = CargarDatos<List<string>>(logAutenticacionFilePath) ?? new List<string>();
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

        public void SignUp(User user)
        {
            if (users.Any(u => u.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("** El nombre de usuario ya está en uso.");
                return;
            }
            users.Add(user);
            GuardarDatos(usersFilePath, users);

            logAutenticacion.Add($"Registro exitoso: {user.Name} - {DateTime.Now}");
            GuardarDatos(logAutenticacionFilePath, logAutenticacion);
            Console.WriteLine("** Usuario registrado con éxito");
        }

        public void Login(string name, string password)
        {
            User user = users.SingleOrDefault(item => item.Name == name);

            if (user != null && user.Password == password)
            {
                UserAuthenticated = user;
                Console.WriteLine("** Inicio de sesión exitoso");
            }
            else
            {
                logAutenticacion.Add($"Intento fallido de inicio de sesión: {name} - {DateTime.Now}");
                GuardarDatos(logAutenticacionFilePath, logAutenticacion);
                Console.WriteLine("** Usuario o contraseña incorrectos");
            }
        }

        public User GetAuthenticatedUser => UserAuthenticated;

        public string GetAuthenticatedUserName()
        {
            return UserAuthenticated?.Name ?? "Desconocido";
        }
        public int GetLastId => users.Any() ? users.Max(u => u.Id) : 0;
    }
}
