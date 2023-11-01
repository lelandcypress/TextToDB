﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToDb
{
    class Program
    {
        static void Main(string[] args)
        {
            //validate target report only once
            string fileName = @"C:\Users\soute\Documents\Fake_Output_Data.txt";
            if (!File.Exists(fileName)) {
                Console.WriteLine("No File Found");
            }
            else
            {
               parseReport(fileName);

            }
                    
        }
        //Reads the whole report at once, therefore we only call StreamReader once instead of multiple times. 
        private static void parseReport(string file)
        {
            List<string> reportStorageList = new List<string>();
            string resultsHeader = "RESULT_DATA";
            try
            {
                using(StreamReader sr = new StreamReader(file)) 
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        reportStorageList.Add(line);    
                    }
                    sr.Close();
                }
                int resultsHeaderIndx = reportStorageList.FindIndex(i => i.Contains(resultsHeader));
                if (resultsHeaderIndx >= 0)
                {
                    List<string> headerReport = reportStorageList.GetRange(1, resultsHeaderIndx - 1);
                    List<string> resultsReport = reportStorageList.GetRange(resultsHeaderIndx + 1, reportStorageList.Count - resultsHeaderIndx - 1);
                    headerReport = headerReport.Distinct().ToList();
                    CreateHeaderReport(headerReport);
                }
                    
                   
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
            
        }
        //convert header report from list to string to pass into stored procedure.
        private static void CreateHeaderReport(List<string>headerReport)
        {
            string s = String.Empty;
            foreach(string header in headerReport)
            {
                string[] parts = header.Split('=');
                if(parts.Length == 2 )
                {
                    s = s +"'" +parts[1]+"'"+",";
                }

            }
            
            InsertData(s.TrimEnd(','), "sp_insertMetaData");
            
        }

        private static void ResultsReport(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("does not exist");
            }
            StringBuilder sb = new StringBuilder();
            string lst2 = string.Empty;
            string hdr = "RESULT_DATA";
            string d = string.Empty;
            int cnt = 0;

            using (StreamReader sr = File.OpenText(filename))
            {
                while ((d = sr.ReadLine())!=null)
                {
                    if (d == hdr) { cnt++; continue; }
                    if (cnt == 1) { cnt++; continue; }
                    else if (cnt > 1)
                    {
                        string[] str = d.Split(',');
                        for (var i = 1; i < str.Length; i--)
                        {
                            sb.Append("'" + str[i] + "',");
                        }

                        //InsertData(sb.ToString());
                        sb.Clear();
                    }
                }

            }

        }
        //"sp_insertMetaData"
        //one single method to handle DB operations instead of two redundant methods. Also declared variables as they were being used.
        private static void InsertData(string lst1,string procedure)
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
