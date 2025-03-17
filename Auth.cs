using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoAdmonGrupo4
{
    internal class Auth
    {
        private List<User> users = new List<User>();
        private User UserAuthenticated { get; set; }

        public void SignUp(User user)
        {
            users.Add(user);
            Console.WriteLine("** Usuario registrado con éxito");
        }

        public void Login(string name, string password)
        {
            User user = users.SingleOrDefault(item => (item.Name == name && item.Password == password));

            if( user != null )
            {
                UserAuthenticated = user;
                Console.WriteLine("** Inicio de sesión exitoso");
            }

            if(user == null)
            {
                Console.WriteLine("** Usuario o contraseña incorrectos");
            }
        }

        public User GetAuthenticatedUser => UserAuthenticated;

        public int GetLastId => users.Count();

        
    }
}
