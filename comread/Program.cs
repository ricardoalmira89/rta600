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

            OptimusDataContext Context = new OptimusDataContext();
            List<Marcacion> Marcaciones = new List<Marcacion>();

                //RTA600 rta = new RTA600();
                //Marcaciones.AddRange(rta.LeerMarcaciones(2));
                //rta.CloseConnection();

            /*
                Marcacion m;
                m.Tarjeta = "2015000000";
                m.Fecha = new DateTime(2015, 05, 15, 8, 10, 15);
                Marcaciones.Add(m);
                m.Tarjeta = "2015000000";
                m.Fecha = new DateTime(2015, 05, 15, 4, 35, 45);
                Marcaciones.Add(m);
             * */

            IncidencesManager im = new IncidencesManager(Context);
            DateTime day = new DateTime(2015, 04, 02);

            foreach (LibraryLibrarian item in Context.LibraryLibrarians) { 
             //if (item.Username == "ricardoalmira89") 
                    im.generateIncidences(item, day);

                 
            }

            Context.SubmitChanges();
            



            /*foreach (Marcacion marcacion in Marcaciones)
            {
                RhMark new_marcacion = new OptimusContext.RhMark();
                new_marcacion.Card = marcacion.Tarjeta;
                new_marcacion.Date = marcacion.Fecha;

                Context.RhMarks.InsertOnSubmit(new_marcacion);
            }

            Console.WriteLine("Lectura finalizada");

            Console.Write("Subiendo a la db...:");
            Context.SubmitChanges();
            Console.WriteLine("[COMPLETADO]");
            Console.ReadLine();*/

        }


    }
}
