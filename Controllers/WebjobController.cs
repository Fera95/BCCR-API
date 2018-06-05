using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
namespace BCCRAPI.Controllers
{
    public class WebjobController : ApiController
    {
        
        public void Get()
        {

            try
            {

                //Se crea el cliente
                cr.fi.bccr.indicadoreseconomicos.wsIndicadoresEconomicos cliente = new cr.fi.bccr.indicadoreseconomicos.wsIndicadoresEconomicos();

                /*Se crea el string de conexion*/
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "avanzadas.database.windows.net";
                cb.UserID = "bases";
                cb.Password = "BATEC123*";
                cb.InitialCatalog = "Bases_DW";


                // Se crea la fecha  Actual
                DateTime Fecha_Actual = DateTime.UtcNow.Date;
                //Se le da formato y se convierte en string para usarla como parametro
                string Hoy = Fecha_Actual.ToString("dd/MM/yyyy");

                //Se crea un query para obtener la ultima fecha guardada en la DB
                string queryUltimaFecha = "SELECT TOP 1 Fecha FROM dimFecha ORDER BY dimFecha.Id_Fecha DESC";

                //Se crea una variable DateTime para guardar la ultima fecha que se obtendra de la DB
                //por defecto se utilizara el dia anterior para que al agregar un dia sea el dia actual
                DateTime UltimaFecha = DateTime.UtcNow.Date.AddDays(-1);

                //se obtiene la ultima fecha guardada conectandose a la db

                using (SqlConnection connection = new SqlConnection(cb.ConnectionString))
                {
                    //se abre la conexion
                    connection.Open();
                    //se crea el comando con el query
                    using (SqlCommand command = new SqlCommand(queryUltimaFecha, connection))
                    {
                        //se inicia el try
                        try
                        {
                            //se hace un result del comando
                            SqlDataReader result = command.ExecuteReader();
                            //se lee el result
                            while (result.Read())
                            {
                                /*como solo es una fecha se lee la posicion 0 y
                                se le asigna a la variable DateTime*/
                                UltimaFecha = result.GetDateTime(0);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                    }
                    //se cierra la conexion
                    connection.Close();
                }






                /*Como se sabe que no necitamos la ultima fecha si el dia siguiente de esta
                 le agregamos un dia*/


                UltimaFecha = UltimaFecha.AddDays(1);

                //Se le da al resultado y se convierte en string para usarla como parametro

                 string Antes = UltimaFecha.ToString("dd/MM/yyyy");


                //Se llama el api con las fechas como parametro

                DataSet Datos = cliente.ObtenerIndicadoresEconomicos("317", Antes, Hoy, "Esteban Ferarios", "N");



                //Se obtienen las fechas y tipo de cambio desde la ultima fecha
                int i = 0;
                /*Se crea la conexcion*/
                using (SqlConnection connection = new SqlConnection(cb.ConnectionString))
                {

                    //Se abre la conexion
                    connection.Open();



                    //Se leen los datos
                    while (i != Datos.Tables[0].Rows.Count)
                    {
                        try
                        {
                            //Se Obtiene la fecha segun la posicion i en formato String
                            string Fechas = Datos.Tables[0].Rows[i].ItemArray[1].ToString();
                            //Se convierten a tipo Datetime para darles formato y manejarlas
                            DateTime myDate = DateTime.Parse(Fechas);

                            //Se dividen los formatos necesarios para el Store Procedure
                            string Fecha = myDate.ToString("MM/dd/yyyy");
                            string Dia = myDate.ToString("dd");
                            string Mes = myDate.ToString("MM");
                            string Año = myDate.ToString("yyyy");
                            string YYYY_MM = myDate.ToString("yyyy/MM");
                            string Mes_String = "Unknown";

                            //Se convierten el Dia,Mes y Año para crear un datetime y obtener el nombre del dia

                            int Dia_int = 0;
                            Int32.TryParse(Dia, out Dia_int);
                            int Mes_int = 0;
                            Int32.TryParse(Mes, out Mes_int);
                            int Año_int = 0;
                            Int32.TryParse(Año, out Año_int);

                            //Se crea el objeto Datetime para saber el nombre del dia 
                            DateTime Date = new DateTime(Año_int, Mes_int, Dia_int, 0, 00, 00);

                            //Se obtiene el nombre del dia de la fecha 
                            string Nombre_dia = Date.DayOfWeek.ToString();

                            //Se inician los switch cases para formar las fechas con el nombre del mes
                            switch (Mes)
                            {
                                case "01":
                                    Mes_String = "January";
                                    break;
                                case "02":
                                    Mes_String = "February";
                                    break;
                                case "03":
                                    Mes_String = "March";
                                    break;
                                case "04":
                                    Mes_String = "April";
                                    break;
                                case "05":
                                    Mes_String = "May";
                                    break;
                                case "06":
                                    Mes_String = "June";
                                    break;
                                case "07":
                                    Mes_String = "July";
                                    break;
                                case "08":
                                    Mes_String = "August";
                                    break;
                                case "09":
                                    Mes_String = "Setember";
                                    break;
                                case "10":
                                    Mes_String = "October";
                                    break;
                                case "11":
                                    Mes_String = "November";
                                    break;
                                case "12":
                                    Mes_String = "December";
                                    break;
                            }

                            // Se crea la fecha completa en formato string
                            string Fecha_String = Nombre_dia + ", " + Mes_String + " " + Dia + "," + Año;



                            //Se Obtiene el tipo de cambio segun el i
                            String Cambio = Datos.Tables[0].Rows[i].ItemArray[2].ToString();
                            // Se reemplaza , por . para complir el formato del Float
                            Cambio = Cambio.Replace(",", ".");





                            //Se crea el query del store y se inserta en la base
                            String query = "EXEC dbo.insertar @Fecha = '" + Fecha + "', @Dia = '" + Dia + "', @Mes = '" + Mes + "', @Año = '" + Año + "', @Mes_String = '"
                                + Mes_String + "', @YYYY_MM = '" + YYYY_MM + "', @Fecha_String = '" + Fecha_String + "', @Tipo_Cambio = '" + Cambio + "'";


                            using (SqlCommand command = new SqlCommand(query, connection))
                            {

                                int resultado = command.ExecuteNonQuery();

                                // Check Error
                                if (resultado < 0)
                                    Console.WriteLine("Error inserting data into Database!");
                            }




                            i++;
                            //System.Threading.Thread.Sleep(5000);
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            // System.Threading.Thread.Sleep(50000);

                        }

                    }
                    //se cierra la conexion
                    connection.Close();
                }
                
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            
        }
    }
}