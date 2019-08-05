using Adapter;
using Json2DB.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;

namespace Json2DB
{
    class ConversionManager
    {
        private string connectionString;
        public ResponseModel Json2Db()
        {
            ResponseModel responseModel;
            responseModel = new ResponseModel()
            {
                NumRowsUpdated = 0,
                Status = ResponseStatus.Success
            };

            //getting the connectionString from Windows Registry (be aware the real cn has CDATA and it's not easily asignable to variable
            string connectionStringID;
            using (StreamReader r = new StreamReader("connectionString.txt"))
            {
                connectionStringID = r.ReadToEnd();
                connectionString = ConnectionStringHelper.GetApplicationConnectionString(connectionStringID);
            }


            //asking name of content
            string fileIntroducedByUser;
            Console.Write("Enter filename: ");
            fileIntroducedByUser = Console.ReadLine();
            if (string.IsNullOrEmpty(fileIntroducedByUser))
            {
                fileIntroducedByUser = "incomingFile.json";
            }


            //convertirn JSON-content to list<DbModel>
            var listComingFromFile = new List<DbModel>();

            using (StreamReader r = new StreamReader(fileIntroducedByUser))
            {
                string json = r.ReadToEnd();
                listComingFromFile = JsonConvert.DeserializeObject<List<DbModel>>(json);
            }

            foreach (var element in listComingFromFile)
            {
                element.FileName = fileIntroducedByUser;
            }

            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    Connection = connection
                };

                //option1
                //foreach (var element in listComingFromFile)
                //{
                //    cmd.CommandText = "INSERT INTO [tescosubscription].[UpdateRecurringPaymentIntermediate] " +
                //                      "(CustomerPaymentId, CustomerId, Status, InsertedDate, Filename) " +
                //                      "VALUES (" + element.CustomerPaymentId + ", " + element.CustomerId + ", '" + element.Status +
                //                      "', '" + DateTime.Now + "', '" + fileIntroducedByUser + "')";

                //    try
                //    {
                //        cmd.ExecuteNonQuery();
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine("Id: " + element.CustomerId + ". Description: " + ex.Message);
                //    }

                //    responseModel.NumRowsUpdated++;
                //}


                //option2
                var objectsDr = new DataReaderAdapter<DbModel>(listComingFromFile);
                var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = "[tescosubscription].[UpdateRecurringPaymentIntermediate]",
                    BatchSize = 1000
                };
                bulkCopy.WriteToServer(objectsDr);
            }
            catch (SqlException ex)
            {
                responseModel.Status = ResponseStatus.Failure;
                responseModel.TraceError = ex.Message;

            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return responseModel;

        } //end method Json2Db

    } //end class
} //end namespace
