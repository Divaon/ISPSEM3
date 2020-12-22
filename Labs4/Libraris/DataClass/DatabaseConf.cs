using System;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace Libraris
{
    public class DatabaseConf

    {
        readonly string connectionString;
        public DatabaseConf(string ConnectionString)
        {
            connectionString = ConnectionString;
        }
        public void ClearInsights()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand("sp_ClearInsights", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;
                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                    {
                        sw.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                    }

                    transaction.Rollback();
                }
            }
        }
        public void InsertInsight(string catalog0, string catalog1, string catalog2, string catalog3, int sdvig )
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand("sp_InsertInsight", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;
                try
                {
                    SqlParameter catalog0Param = new SqlParameter
                    {
                        ParameterName = "@catalog0",
                        Value = catalog0
                    };
                    SqlParameter catalog1Param = new SqlParameter
                    {
                        ParameterName = "@catalog1",
                        Value = catalog1
                    };
                    SqlParameter catalog2Param = new SqlParameter
                    {
                        ParameterName = "@catalog2",
                        Value = catalog2
                    };
                    SqlParameter catalog3Param = new SqlParameter
                    {
                        ParameterName = "@catalog3",
                        Value = catalog3
                    };
                    SqlParameter sdvigParam = new SqlParameter
                    {
                        ParameterName = "@sdvig",
                        Value = sdvig
                    };
                 
                    command.Parameters.AddRange(new[] { catalog0Param, catalog1Param, catalog2Param, catalog3Param,sdvig });
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                    {
                        sw.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                    }
                    transaction.Rollback();
                }
            }
        }
        public void WriteInsightsToXml(string outputFolder)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand("sp_GetInsights", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet dataSet = new DataSet("Insights");
                    DataTable dataTable = new DataTable("Insight");
                    dataSet.Tables.Add(dataTable);
                    adapter.Fill(dataSet.Tables["Insight"]);
                    XmlGenerator xmlGenerator = new XmlGenerator(outputFolder);
                    xmlGenerator.WriteToXml(dataSet, "appInsights");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exceptions.txt"), true))
                    {
                        sw.WriteLine($"{DateTime.Now:dd/MM/yyyy HH:mm:ss} Exception: {ex.Message}");
                    }
                    transaction.Rollback();
                }
            }
        }
        public void GetParams(string outputFolder, DataBaseWorker appInsights, string customersFileName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand("sp_GetConfP", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet dataSet = new DataSet("ConfParams");
                    DataTable dataTable = new DataTable("ConfParam");
                    dataSet.Tables.Add(dataTable);
                    adapter.Fill(dataSet.Tables["ConfParam"]);
                    XmlGenerator xmlGenerator = new XmlGenerator(outputFolder);
                    xmlGenerator.WriteToXml(dataSet, customersFileName);
                    appInsights.InsertInsight("Configuration parametres were received successfully");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    appInsights.InsertInsight("EXCEPTION: " + ex.Message);
                    transaction.Rollback();
                }
            }
        }
    }
}