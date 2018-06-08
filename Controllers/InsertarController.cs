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
    public class InsertarController : ApiController
    {
        // GET api/insertar
        public void Get()
        {

            try
            {

                //Se crea el cliente
                cr.fi.bccr.indicadoreseconomicos.wsIndicadoresEconomicos cliente = new cr.fi.bccr.indicadoreseconomicos.wsIndicadoresEconomicos();
                // Se crea la fecha  Actual
                DateTime Fecha_Actual = DateTime.UtcNow.Date;
                //Se le da formato y se convierte en string para usarla como parametro
                string Hoy = Fecha_Actual.ToString("dd/MM/yyyy");


                //Se crea la fecha de hace 6 años 
                var Fecha_Hace_6años = Fecha_Actual.AddYears(-6);
                //Se le da formato y se convierte en string para usarla como parametro
                string Antes = Fecha_Hace_6años.ToString("dd/MM/yyyy");

                //Se llama el api con las fechas como parametro

                DataSet Datos = cliente.ObtenerIndicadoresEconomicos("317", Antes, Hoy, "Esteban Ferarios", "N");


                /*Se crea el string de conexion*/
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "avanzadas.database.windows.net";
                cb.UserID = "bases";
                cb.Password = "BATEC123*";
                cb.InitialCatalog = "Bases_DW";

                //Se inicia el while de los datos obtenidos
                int i = 0;
                /*Se crea la conexcion*/
                using (SqlConnection connection = new SqlConnection(cb.ConnectionString))
                {

                    //Se abre la conexion
                    connection.Open();

                    //Se borran los datos anteriores para la prueba

                    String queryBorrar = "DELETE FROM dimTipoCambio DELETE FROM dimFecha DBCC CHECKIDENT('dimFecha', RESEED, 0); DBCC CHECKIDENT ('dimTipoCambio', RESEED, 0);";

                    using (SqlCommand command = new SqlCommand(queryBorrar, connection))
                    {

                        int result = command.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                            Console.WriteLine("Error inserting data into Database!");
                    }

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

                            //Se inician los switch cases para formar las fechas con el nombre
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

                            Cambio = Cambio.Replace(",", ".");

                            //Se crea el query para el store procedure



                            //Se crea el query del store y se inserta en la base
                            String query = "EXEC dbo.insertar @Fecha = '" + Fecha + "', @Dia = '" + Dia + "', @Mes = '" + Mes + "', @Año = '" + Año + "', @Mes_String = '"
                                + Mes_String + "', @YYYY_MM = '" + YYYY_MM + "', @Fecha_String = '" + Fecha_String + "', @Tipo_Cambio = '" + Cambio + "'";

                            Console.WriteLine(query);
                            using (SqlCommand command = new SqlCommand(query, connection))
                            {

                                int result = command.ExecuteNonQuery();

                                // Check Error
                                if (result < 0)
                                    Console.WriteLine("Error inserting data into Database!");
                            }




                            i++;
                            //System.Threading.Thread.Sleep(50000);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            System.Threading.Thread.Sleep(50000);

                        }

                    }
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
