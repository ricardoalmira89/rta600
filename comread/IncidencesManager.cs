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

        public void generateIncidences(LibraryLibrarian librarian, DateTime day) {

            if (librarian.Office.BossId != librarian.Id) { 

                RhMark inicio, fin;
                int count = 0;

                //--- Marcaciones del dia de hoy para la tarjeta especificada ---
                var marks = from mark in context.RhMarks
                            orderby mark.Date ascending
                            where mark.Card == librarian.Card 
                            &&  mark.Date.Date == day.Date
                            select mark;

                count = marks.Count();

                //--- Ausencia ---
                if (count == 0)
                       context.RhIncidences.InsertOnSubmit(new RhIncidence() { Hours = librarian.RhSchedule.End.Value.Subtract(librarian.RhSchedule.Begin).Hours, LibraryLibrarian = librarian, Date = DateTime.Now.Date, IncidenceStateId = 1 });
  
                //--- Falta marcacion de entrada o de salida ---
                if (count == 1)
                {
                    inicio = marks.First();
                    context.RhIncidences.InsertOnSubmit(new RhIncidence() { Hours = 4.0, LibraryLibrarian = librarian, RhMark = inicio, Date = inicio.Date, IncidenceStateId = 1 });

                }

                //--- Normal o de mas
                if (count >= 2) {

                    inicio = marks.First();
                    fin = marks.ElementAt(count - 1);

                    double diffInicio = librarian.RhSchedule.Begin.Subtract(inicio.Date.TimeOfDay).TotalHours;
                    double diffEnd = fin.Date.TimeOfDay.Subtract(librarian.RhSchedule.End.Value).TotalHours;

                    if (diffInicio < 0)
                        context.RhIncidences.InsertOnSubmit(new RhIncidence() { Hours = diffInicio * -1, LibraryLibrarian = librarian, RhMark = inicio, Date = inicio.Date, IncidenceStateId = 1 });
         
                    if (diffEnd < 0)
                        context.RhIncidences.InsertOnSubmit(new RhIncidence() { Hours = diffEnd * -1, LibraryLibrarian = librarian, RhMark = fin, Date = fin.Date, IncidenceStateId = 1 });

                }
            }
            
        }

    }
}
