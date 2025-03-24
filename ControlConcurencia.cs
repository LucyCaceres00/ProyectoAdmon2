using System;
using System.Collections.Generic;
using System.Threading;

namespace ProyectoAdmonGrupo4
{
    public enum TipoBloqueo
    {
        Lectura,
        Escritura
    }

    public class ControlConcurrencia
    {
    
        private Dictionary<int, List<BloqueoRecurso>> bloqueos = new Dictionary<int, List<BloqueoRecurso>>();

  
        private Dictionary<int, List<int>> transaccionesEsperando = new Dictionary<int, List<int>>();


        private readonly object lockObj = new object();


        public class BloqueoRecurso
        {
            public int IdTransaccion { get; set; }
            public TipoBloqueo Tipo { get; set; }

            public BloqueoRecurso(int idTransaccion, TipoBloqueo tipo)
            {
                IdTransaccion = idTransaccion;
                Tipo = tipo;
            }
        }


        public bool AdquirirBloqueo(int idRecurso, TipoBloqueo tipo, int idTransaccion)
        {
            lock (lockObj)
            {
                Console.WriteLine($"Transacción {idTransaccion} intentando adquirir bloqueo de {tipo} sobre recurso {idRecurso}");

        
                if (!transaccionesEsperando.ContainsKey(idTransaccion))
                {
                    transaccionesEsperando[idTransaccion] = new List<int>();
                }

        
                if (!bloqueos.ContainsKey(idRecurso))
                {
                    bloqueos[idRecurso] = new List<BloqueoRecurso>();
                }

            
                if (PuedeAdquirirBloqueo(idRecurso, tipo, idTransaccion))
                {
                
                    bloqueos[idRecurso].Add(new BloqueoRecurso(idTransaccion, tipo));

                    if (transaccionesEsperando[idTransaccion].Contains(idRecurso))
                    {
                        transaccionesEsperando[idTransaccion].Remove(idRecurso);
                    }

                    Console.WriteLine($"Transacción {idTransaccion} adquirió bloqueo de {tipo} sobre recurso {idRecurso}");
                    return true;
                }
                else
                {
       
                    if (!transaccionesEsperando[idTransaccion].Contains(idRecurso))
                    {
                        transaccionesEsperando[idTransaccion].Add(idRecurso);
                    }

                    Console.WriteLine($"Transacción {idTransaccion} debe esperar por el recurso {idRecurso}");

             
                    if (DetectarDeadlock())
                    {
                        ResolverDeadlock();
                    }

                    return false;
                }
            }
        }

  
        private bool PuedeAdquirirBloqueo(int idRecurso, TipoBloqueo tipo, int idTransaccion)
        {
           
            if (!bloqueos.ContainsKey(idRecurso) || bloqueos[idRecurso].Count == 0)
                return true;

     
            if (bloqueos[idRecurso].Exists(b => b.IdTransaccion == idTransaccion && b.Tipo == TipoBloqueo.Escritura))
                return true;

  
            if (tipo == TipoBloqueo.Lectura)
            {
      
                return !bloqueos[idRecurso].Exists(b => b.Tipo == TipoBloqueo.Escritura);
            }
         
            else
            {
               
                return bloqueos[idRecurso].TrueForAll(b => b.IdTransaccion == idTransaccion);
            }
        }

  
        public void LiberarBloqueo(int idRecurso, int idTransaccion)
        {
            lock (lockObj)
            {
                if (bloqueos.ContainsKey(idRecurso))
                {
                    bloqueos[idRecurso].RemoveAll(b => b.IdTransaccion == idTransaccion);
                    Console.WriteLine($"Transacción {idTransaccion} liberó bloqueos sobre recurso {idRecurso}");
                }
            }
        }

      
        public void LiberarTodosBloqueos(int idTransaccion)
        {
            lock (lockObj)
            {
                foreach (var lista in bloqueos.Values)
                {
                    lista.RemoveAll(b => b.IdTransaccion == idTransaccion);
                }

                if (transaccionesEsperando.ContainsKey(idTransaccion))
                {
                    transaccionesEsperando.Remove(idTransaccion);
                }

                Console.WriteLine($"Todos los bloqueos de la transacción {idTransaccion} han sido liberados");
            }
        }

       
        private bool DetectarDeadlock()
        {
        
            foreach (var t1 in transaccionesEsperando)
            {
                foreach (var r1 in t1.Value)
                {
               
                    if (bloqueos.ContainsKey(r1))
                    {
                        foreach (var b in bloqueos[r1])
                        {
                
                            if (transaccionesEsperando.ContainsKey(b.IdTransaccion))
                            {
                                foreach (var r2 in transaccionesEsperando[b.IdTransaccion])
                                {
                                    if (bloqueos.ContainsKey(r2) && bloqueos[r2].Exists(x => x.IdTransaccion == t1.Key))
                                    {
                                        Console.WriteLine($"DEADLOCK detectado: Transacción {t1.Key} espera a {b.IdTransaccion} y viceversa");
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

 
        private void ResolverDeadlock()
        {
     
            int maxId = -1;
            foreach (var t in transaccionesEsperando.Keys)
            {
                if (t > maxId) maxId = t;
            }

            if (maxId > 0)
            {
                Console.WriteLine($"Resolviendo deadlock abortando la transacción {maxId}");
                LiberarTodosBloqueos(maxId);
            }
        }

        public void MostrarEstadoBloqueos()
        {
            lock (lockObj)
            {
                Console.WriteLine("\n--- Estado de Bloqueos ---");
                foreach (var par in bloqueos)
                {
                    if (par.Value.Count > 0)
                    {
                        Console.WriteLine($"Recurso {par.Key}:");
                        foreach (var bloqueo in par.Value)
                        {
                            Console.WriteLine($"  - Transacción {bloqueo.IdTransaccion}: {bloqueo.Tipo}");
                        }
                    }
                }

                Console.WriteLine("\n--- Transacciones Esperando ---");
                foreach (var par in transaccionesEsperando)
                {
                    if (par.Value.Count > 0)
                    {
                        Console.WriteLine($"Transacción {par.Key} espera por: {string.Join(", ", par.Value)}");
                    }
                }
            }
        }
    }


    public class SimuladorConcurrencia
    {
        private ControlConcurrencia controlConcurrencia = new ControlConcurrencia();


        public void SimularTransacciones()
        {
            Console.WriteLine("Iniciando simulación de transacciones concurrentes...");

 
            int t1 = 1;
            int t2 = 2;


            Console.WriteLine("\nEscenario 1: Dos transacciones accediendo a diferentes recursos");
            controlConcurrencia.AdquirirBloqueo(101, TipoBloqueo.Lectura, t1);
            controlConcurrencia.AdquirirBloqueo(102, TipoBloqueo.Escritura, t2);
            controlConcurrencia.MostrarEstadoBloqueos();
            controlConcurrencia.LiberarTodosBloqueos(t1);
            controlConcurrencia.LiberarTodosBloqueos(t2);

            Console.WriteLine("\nEscenario 2: Dos transacciones con bloqueos de lectura sobre el mismo recurso");
            controlConcurrencia.AdquirirBloqueo(201, TipoBloqueo.Lectura, t1);
            controlConcurrencia.AdquirirBloqueo(201, TipoBloqueo.Lectura, t2);
            controlConcurrencia.MostrarEstadoBloqueos();
            controlConcurrencia.LiberarTodosBloqueos(t1);
            controlConcurrencia.LiberarTodosBloqueos(t2);

            Console.WriteLine("\nEscenario 3: Bloqueo de escritura impide bloqueo de lectura");
            controlConcurrencia.AdquirirBloqueo(301, TipoBloqueo.Escritura, t1);
            bool resultado = controlConcurrencia.AdquirirBloqueo(301, TipoBloqueo.Lectura, t2);
            Console.WriteLine($"T2 pudo adquirir bloqueo de lectura: {resultado}");
            controlConcurrencia.MostrarEstadoBloqueos();
            controlConcurrencia.LiberarTodosBloqueos(t1);
            controlConcurrencia.LiberarTodosBloqueos(t2);

            Console.WriteLine("\nEscenario 4: Simulación de deadlock");
            controlConcurrencia.AdquirirBloqueo(401, TipoBloqueo.Escritura, t1);
            controlConcurrencia.AdquirirBloqueo(402, TipoBloqueo.Escritura, t2);

       
            Thread thread1 = new Thread(() => {
                controlConcurrencia.AdquirirBloqueo(402, TipoBloqueo.Escritura, t1);
            });

            Thread thread2 = new Thread(() => {
                controlConcurrencia.AdquirirBloqueo(401, TipoBloqueo.Escritura, t2);
            });

            thread1.Start();
            thread2.Start();

            Thread.Sleep(1000); 

            controlConcurrencia.MostrarEstadoBloqueos();


            controlConcurrencia.LiberarTodosBloqueos(t1);
            controlConcurrencia.LiberarTodosBloqueos(t2);

            Console.WriteLine("\nSimulación completada.");
        }
    }
}