using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OptimusContext;

namespace comread
{
    class IncidencesManager
    {

        private OptimusDataContext context;

        public IncidencesManager(OptimusDataContext _context) {

            context = _context;
        }

        public void generateIncidences(LibraryLibrarian librarian) {

            RhMark inicio, fin;
            int count = 0;
            DateTime dia = new DateTime(2015, 05, 17);

            //--- Marcaciones del dia de hoy para la tarjeta especificada ---
            var marks = from mark in context.RhMarks
                        orderby mark.Date ascending
                        where mark.Card == librarian.Card 
                        &&  mark.Date.Date == dia
                        select mark;

            count = marks.Count();

            //--- Ausencia ---
            if (count == 0)
                   context.RhIncidences.InsertOnSubmit(new RhIncidence() { Hours = librarian.RhSchedule.End.Value.Subtract(librarian.RhSchedule.Begin).Hours, LibraryLibrarian = librarian });
  
            //--- Falta marcacion de entrada o de salida ---
            if (count == 1)
            {

            }

            //--- Normal o de mas
            if (count >= 2) {

                inicio = marks.First();
                fin = marks.ElementAt(count - 1);

                double diffInicio = librarian.RhSchedule.Begin.Subtract(inicio.Date.TimeOfDay).TotalHours;
                double diffEnd = fin.Date.TimeOfDay.Subtract(librarian.RhSchedule.End.Value).TotalHours;

                if (diffInicio < 0) 
                    context.RhIncidences.InsertOnSubmit(new RhIncidence() { Hours = diffInicio * -1, LibraryLibrarian = librarian, RhMark = inicio });
         
                if (diffEnd < 0)
                    context.RhIncidences.InsertOnSubmit(new RhIncidence() { Hours = diffEnd * -1, LibraryLibrarian = librarian, RhMark = inicio });

            }

            context.SubmitChanges();
        }

    }
}
