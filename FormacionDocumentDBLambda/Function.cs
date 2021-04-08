using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using MongoDB.Driver;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FormacionDocumentDBLambda
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            string template = "mongodb://{0}:{1}@{2}/?replicaSet=rs0&readpreference={3}";
            //string template = "mongodb://{0}:{1}@{2}/?sslAllowInvalidHostnames=true&ssl=true&sslCAFile=rds-combined-ca-bundle.pem";
            string username = "adminadmin";
            string password = "adminadmin";
            string readPreference = "secondaryPreferred";
            string clusterEndpoint = "docdb-2021-03-31-11-44-27.cluster-cihrsqslvehp.eu-west-1.docdb.amazonaws.com:27017";
            string connectionString = String.Format(template, username, password, clusterEndpoint, readPreference);

            string pathToCAFile = "rds-combined-ca-bundle.pem";


            X509Store localTrustStore = new X509Store(StoreName.Root);
            X509Certificate2Collection certificateCollection = new X509Certificate2Collection();
            certificateCollection.Import(pathToCAFile);
            try
            {
                localTrustStore.Open(OpenFlags.ReadWrite);
                localTrustStore.AddRange(certificateCollection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Root certificate import failed: " + ex.Message);
                throw;
            }
            finally
            {
                localTrustStore.Close();
            }

            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            var client = new MongoClient(settings);
            var database = client.ListDatabaseNames();
            Console.WriteLine("Conexión realizada");

            while (database.MoveNext())
            {
                Console.WriteLine("Existen " + database.Current.Count() + " bases de datos");
                foreach (string s in database.Current)
                {
                    Console.WriteLine(s);
                }
            }

            return "OK";
        }
    }
}
