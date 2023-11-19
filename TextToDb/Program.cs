﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ex = TextToDb.CRUD;

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
                    List<string> resultsReport = reportStorageList.GetRange(resultsHeaderIndx + 2, reportStorageList.Count - resultsHeaderIndx - 2);
                    headerReport = headerReport.Distinct().ToList();
                    string[] keypair = headerReport[6].Split('=');
                    CreateHeaderReport(headerReport);
                    CreateResultsReport(resultsReport, keypair[1]);
                }
                    
                   
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                reportStorageList.Clear();
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
            
            ex.InsertMetaData(s.TrimEnd(','), "sp_insertMetaData");
            
        }

        
        private static void CreateResultsReport(List<string> resultsReport,string headerindx)
        {
            DataTable results = new DataTable();
            results.Columns.Add("PortlandTestNumber", typeof(string));
            results.Columns.Add("PortlandTestName", typeof(string));
            results.Columns.Add("TestParagraphNumber", typeof(string));
            results.Columns.Add("TestCaseNumber", typeof(string));
            results.Columns.Add("TestCaseName", typeof(string));
            results.Columns.Add("LowerLimit", typeof(string));
            results.Columns.Add("UpperLimit", typeof(string));
            results.Columns.Add("Measurement", typeof(string));
            results.Columns.Add("Units", typeof(string));
            results.Columns.Add("StepPassed", typeof(string));
            results.Columns.Add("UnitSN");

            foreach (string item in resultsReport)
            {
               
                string[] values = item.Split(',');
                DataRow row = results.NewRow();
                if (values.Length == 10)
                {
                    row["PortlandTestNumber"] = values[0];
                    row["PortlandTestName"] = values[1];
                    row["TestParagraphNumber"] = values[2];
                    row["TestCaseNumber"] = values[3];
                    row["TestCaseName"] = values[4];
                    row["LowerLimit"] = values[5];
                    row["UpperLimit"] = values[6];
                    row["Measurement"] = values[7];
                    row["Units"] = values[8];
                    row["StepPassed"] = values[9];
                    row["UnitSN"] = headerindx;
                    results.Rows.Add(row);    
                }

            }


            ex.InsertData(results, "sp_insertData" );





        }
  
        


    }//EOC
}
