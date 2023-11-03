using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToDb
{
    public class CRUD
    {
        public static void InsertData(DataTable lst1, string procedure)
        {

            try
            {
                string connectionString = @"Data Source=localhost;Initial Catalog=TextToDB;Integrated Security=True";
                SqlConnection conn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand(procedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Data", lst1);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (SqlException exxp)
            {
                Console.WriteLine(exxp.Message);
            }
        }
        public static void InsertMetaData(string lst1, string procedure)
        {

            try
            {
                string connectionString = @"Data Source=localhost;Initial Catalog=TextToDB;Integrated Security=True";
                SqlConnection conn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand(procedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@List", lst1);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (SqlException exxp)
            {
                Console.WriteLine(exxp.Message);
            }
        }


    }//EOC



}
