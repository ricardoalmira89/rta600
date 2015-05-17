using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using System.Text.RegularExpressions;
using Extensions;

namespace RTA600_Device
{

    /// <summary>
    /// Encapsula una Marcacion
    /// </summary>
    public struct Marcacion
    {
        public string Tarjeta;
        public DateTime Fecha;
    }

    /// <summary>
    /// Encapsula el dispositivo RTA600
    /// </summary>
    public class RTA600
    {
        private SerialPort _serialPort = new SerialPort();
        private int _buff_height = 4096;
        /// <summary>
        /// Crea una interface para el dispositivo RTA600
        /// </summary>
        public RTA600()
        {
            _serialPort.PortName = "COM1";
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = System.IO.Ports.Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = System.IO.Ports.StopBits.One;
            _serialPort.Handshake = System.IO.Ports.Handshake.None;
            _serialPort.RtsEnable = true;
            _serialPort.DtrEnable = true;
            _serialPort.ReadTimeout = 1000;

            _serialPort.Open();
        }
        
        /// <summary>
        /// Cierra el puerto serial.
        /// </summary>
        public void CloseConnection()
        {
            _serialPort.Close();
        }

        /// <summary>
        /// Convierte un string en formato hexadecimal a un arreglo de bytes
        /// </summary>
        /// <param name="text">string en formato hexadecimal, ej: E7E7101090E7</param>
        /// <returns></returns>
        private byte[] StringToByteArr(string text)
        {
            byte[] buff = new byte[text.Length / 2];
            byte buff_index = 0;

            for (int i = 0; i < text.Length; i += 2)
            {
                buff[buff_index] = Convert.ToByte(text[i].ToString() + text[i+1].ToString(), 16);
                buff_index++;
            }

            return buff;
        }

        /// <summary>
        /// Lee el puerto serie y devuelve un arreglo de bytes con la lectura recibida
        /// </summary>
        /// <returns></returns>
        public byte[] ReadSerial()
        {
            List<byte> l = new List<byte>();

            try
            {
                for (int i = 0; i < _buff_height; i++)
                    l.Add((byte)_serialPort.ReadByte());
            }
            catch (Exception)
            {
            }

            return l.ToArray();
        }

        /// <summary>
        /// Escribe en el puerto serie
        /// </summary>
        /// <param name="data">los datos a escribir ej: E7E7102090E7 enviará el equivalente de cada {E7,E7,10,20,90,E7} convertido a bytes </param>
        public void WriteSerial(string data)
        {
            byte[] buffer_send = StringToByteArr(data);
            _serialPort.Write(buffer_send, 0, buffer_send.Count());
        }

        /// <summary>
        /// Limpia los datos del nodo especificado
        /// </summary>
        /// <param name="nodo"></param>
        private void LimpiaNodo(byte nodo)
        {
            string node_hex_value = (nodo * 10).ToString();
            WriteSerial(string.Format("E7E710{0}60E7",node_hex_value));
        }

        /// <summary>
        /// Convierte un buffer de bytes leido con byte[] ReadSerial() a una lista con los strings de cada marcacion con el patron "tarjeta:fecha:hora:status#"
        /// </summary>
        /// <param name="buffer">buffer recibido desde el puerto serial</param>
        /// <returns></returns>
        private List<string> BytesToMarcation(List<byte> buffer)
        {
            List<string> marcations = new List<string>();

            string line = string.Empty;

            foreach (int item in buffer)
                line += Char.ConvertFromUtf32(item);

            foreach (Match match in Regex.Matches(line, @"\d+:\d+:\d+:\d+#"))
                marcations.Add(match.Value);

            return marcations;

        }

        /// <summary>
        /// Devuelve un objeto Marcacion a partir de un string con patron "tarjeta:fecha:hora:status#" 
        /// </summary>
        /// <param name="marcation_text"></param>
        /// <returns></returns>
        private Marcacion CreateMarcation(string marcation_text)
        {
            Marcacion Temp_Marcacion;
            string[] data = marcation_text.Split(':');
            Temp_Marcacion.Tarjeta = data[0];

            int anno = int.Parse("20" + data[1].Substring(0,2));
            int mes =  int.Parse(data[1].Substring(2, 2));
            int dia =  int.Parse(data[1].Substring(4, 2));


            Temp_Marcacion.Fecha = new DateTime(int.Parse("20" + data[1].Substring(0, 2)),
                                                int.Parse(data[1].Substring(2, 2)),
                                                int.Parse(data[1].Substring(4, 2)),
                                                int.Parse(data[2].Substring(0, 2)),
                                                int.Parse(data[2].Substring(2, 2)),
                                                int.Parse(data[2].Substring(4, 2)),
                                                0);
            return Temp_Marcacion;
        }

        /// <summary>
        /// Lee las marcaciones de un nodo determinado.
        /// </summary>
        /// <param name="nodo">El valor del nodo va desde 1 hasta 9</param>
        /// <param name="limpiar">true para borrar los datos del dispositivo despues de leerlos</param>
        /// <returns>Un lista con las marcaciones leidas.</returns>
        public List<Marcacion> LeerMarcaciones(byte nodo)
        {
            List<Marcacion> Temp_Marcaciones = new List<Marcacion>();

            if (nodo <= 9)
            {
                byte[] buffer;
                List<byte> buffer_total = new List<byte>();
                List<string> marcation_strings = new List<string>();

                Console.Write("Leyendo nodo #{0}:\t", nodo);

                WriteSerial(string.Format("7E7E010{0}097E", nodo));
                buffer = ReadSerial();
                buffer_total.AddRange(buffer);

                bool stop = false;

                while (!stop)
                {
                    WriteSerial(string.Format("7E7E010{0}067E7E7E010{0}097E", nodo));

                    buffer = ReadSerial();
                    buffer_total.AddRange(buffer);
                    if (buffer.HexStr() == string.Format("7E7E010{0}06097E", nodo)) stop = true;
                    
                        
                }

                marcation_strings.AddRange(BytesToMarcation(buffer_total));
                Console.WriteLine("{0} marcaciones.", marcation_strings.Count);

                foreach (string marcacion in marcation_strings)
                {
                    Console.WriteLine(marcacion);
                    Temp_Marcaciones.Add(CreateMarcation(marcacion));
                }
            }
            else
                Console.WriteLine("Nodo fuera de rango. Debe ser entre 1 y 9");



            return Temp_Marcaciones;
        }

        
        

    }
}

namespace Extensions
{
    public static class ByteArrayExtension
    {
        static readonly char[] hexchar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        
        /// <summary>
        /// Devuelve un string que concatena todos los valores del arreglo de bytes convertidos a hexadecimal.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static string HexStr(this byte[] data, bool space = false)
        {
            int len = data.Length;
            int offset = 0;
            int i = 0, k = 2;
            if (space) k++;
            var c = new char[len * k];
            while (i < len)
            {
                byte d = data[offset + i];
                c[i * k] = hexchar[d / 0x10];
                c[i * k + 1] = hexchar[d % 0x10];
                if (space && i < len - 1) c[i * k + 2] = ' ';
                i++;
            }
            return new string(c, 0, c.Length);
        }
    }
}
