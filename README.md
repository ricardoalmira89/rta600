# rta600
Una interface para leer los datos de un dispositivo RTA600.

Con este codigo, se puede generar una dll para acceder a las funcionalidades del reloj RTA600.
el codigo fuente esta bien documentado.

Todo el proyecto no es necesario, fue un trabajo que tuve que hacer hace algun tiempo, pero basado en el RTA600.
Solo debes compilar la dll, utilizando el fichero RTA600.cs.

Si no sabes como compilar una dll, es asi:

Abres una consola de windows, y tecleas esto:
Ojo: Las rutas son relativas a mi computador, debes cambiarlas por las tuyas propias.

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc /target:library /out:C:\Users\cyber\Desktop\comread\comread\RTA600.DLL C:\Users\cyber\Desktop\comread\comread\RTA600.cs


Si no necesitas compilar la dll, solo tienes que descargarte el fichero RTA600.dll el cual ya esta listo para utilizar.

Si tienes dudas, escribeme a ricardoalmira89@gmail.com
