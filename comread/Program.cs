using System;
using System.Collections.Generic;
using RTA600_Device;
using OptimusContext;

namespace comread
{
    class Program
    {
        static void Main(string[] args)
        {

                OptimusDataContext Context = new OptimusContext.OptimusDataContext();
                List<Marcacion> Marcaciones = new List<Marcacion>();

                RTA600 rta = new RTA600();
                Marcaciones.AddRange(rta.LeerMarcaciones(1, false));
                Marcaciones.AddRange(rta.LeerMarcaciones(2, false));
                rta.CloseConnection();

                foreach (Marcacion marcacion in Marcaciones)
                {
                    OptimusContext.Optimus_Marcaciones new_marcacion = new OptimusContext.Optimus_Marcaciones();
                    new_marcacion.Tarjeta = marcacion.Tarjeta;
                    new_marcacion.Fecha = marcacion.Fecha;

                    Context.Optimus_Marcaciones.InsertOnSubmit(new_marcacion);
                }

                Console.WriteLine("Lectura finalizada");

                Console.Write("Subiendo a la db...:");
                Context.SubmitChanges();
                Console.WriteLine("[COMPLETADO]");
            Console.ReadLine();

        }

  
    }
}
