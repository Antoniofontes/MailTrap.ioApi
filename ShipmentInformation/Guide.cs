using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentInformation
{
    public class Guide
    {
        public float Guia { get; set; }
        public DateTime Fecha { get; set; }
        public string Remitente { get; set; }
        public string Contenido { get; set; }
        public float Peso { get; set; }
        public float Valor { get; set; }
        public string Estado { get; set; }
        public string Tracking { get; set; }
        public object TrackingLink { get; set; }
        public string Cerrada { get; set; }
        public DateTime FechaCierre { get; set; }
        public string Bolsa { get; set; }
    }

}
