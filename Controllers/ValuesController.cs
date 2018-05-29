using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BCCRAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {

            try
            {

                //Se crea el cliente
                cr.fi.bccr.indicadoreseconomicos.wsIndicadoresEconomicos cliente = new cr.fi.bccr.indicadoreseconomicos.wsIndicadoresEconomicos();
               // Se crea la fecha de hoy
                DateTime Fecha_Actual = DateTime.UtcNow.Date;
                string Hoy = Fecha_Actual.ToString("dd/MM/yyyy");


                //Se crea la fecha de hace 6 meses 
                var Fecha_Hace_6Meses = Fecha_Actual.AddMonths(-6);
                string Antes = Fecha_Hace_6Meses.ToString("dd/MM/yyyy");

                //Se llama el api con las fechas como parametro

                DataSet Datos = cliente.ObtenerIndicadoresEconomicos("317", Antes, Hoy, "Esteban Ferarios", "N");
                

                //Se convierten los datos
                string DatoFechaHoy = Datos.Tables[0].Rows[1].ItemArray[1].ToString();
                DateTime myDate = DateTime.Parse(DatoFechaHoy);
                string Test = myDate.ToString("dd/MM/yyyy");

                string tipocambioAnterior = Datos.Tables[0].Rows[0].ItemArray[2].ToString();

              

                // Se crean los strings para enseñar las fechas
                string Fecha1 = "Fecha:" + Hoy + " Cambio: " + Test;
                string Fecha2 = "Fecha: " + Antes + " Cambio: " + tipocambioAnterior;

                int i = 0;
                while (!Datos.Tables[0].Rows[1].ItemArray[i].Equals(Hoy))
                {

                }   

                return new string[] {Fecha1, Fecha2};
            }
            catch (Exception e)
            {
               
                return new string[] { e.ToString()};

            }

            
        }

        // GET api/values/5
        public string Get(int id)
        {
            try {
                cr.fi.bccr.indicadoreseconomicos.wsIndicadoresEconomicos cliente = new cr.fi.bccr.indicadoreseconomicos.wsIndicadoresEconomicos();
                DateTime dateTime = DateTime.UtcNow.Date;
                string Hoy = dateTime.ToString("dd/MM/yyyy");
                 DataSet Datos = cliente.ObtenerIndicadoresEconomicos("317", Hoy, Hoy, "Esteban Ferarios", "N");
                string tipocambio = Datos.Tables[0].Rows[0].ItemArray[2].ToString();

                return tipocambio;
               
            }
            catch (Exception e)
            {
                return e.ToString();

            }

         }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
