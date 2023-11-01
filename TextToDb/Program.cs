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
            string fileName = @"C:\Users\soute\Documents\Fake_Output_Data.txt";
            if (!File.Exists(fileName)) {
                Console.WriteLine("No File Found");
            }
            else
            {
               parseReport(fileName);

            }
                    
        }

        private static void parseReport(string file)
        {
            List<string> reportStorageList = new List<string>();
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
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
            
        }
        private static void GetHeaderData(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Console.WriteLine("does not exist");
            }

            List<string> fieldNames = new List<string>();
            string lst1 = string.Empty;
            string s = string.Empty;

            // METADATA HEADER 
            using (StreamReader sr = File.OpenText(fileName))
            {
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.Contains("="))
                    {
                        string[] vals = s.Split('=');

                        if (!fieldNames.Contains(vals[0]))
                        {
                            fieldNames.Add(vals[0]);
                            if (vals.Length > 2)
                            {
                                lst1 = lst1 + vals[1] + "'" + ",";
                            }
                        }
                    }
                    else if (s == "RESULT_DATA")
                    {
                        //InsertData(lst1.TrimEnd(','));
                        return;
                    }
                }
            }
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
        private static void InsertData(string lst1,string procedure)
        {
     
            try
            {
               string connectionString = @"Your data source path";
               SqlConnection conn = new SqlConnection(connectionString);

                SqlCommand cmd = new SqlCommand(procedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@List", lst1);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception exxp)
            {
                Console.WriteLine(exxp.Message);
            }
        }

    }//EOC
}
