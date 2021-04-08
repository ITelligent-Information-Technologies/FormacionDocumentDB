using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace FormacionDocumentDB
{
    class Program
    {
        static void Main(string[] args)
        {
            string clusterEndpoint = "docdb-2021-03-31-11-44-27.cluster-cihrsqslvehp.eu-west-1.docdb.amazonaws.com:27017";
            string template = "mongodb://{0}:{1}@{2}/sampledatabase?ssl=true&replicaSet=rs0&readpreference={3}";
            string username = "adminadmin";
            string password = "adminadmin";
            string readPreference = "secondaryPreferred";
            string connectionString = String.Format(template, username, password, clusterEndpoint, readPreference);

            string pathToCAFile = "rdscombinedcabundle_cert_out.p7b";

            // ADD CA certificate to local trust store
            // DO this once - Maybe when your service starts
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

            var database = client.GetDatabase("sample_database");
            var collection = database.GetCollection<BsonDocument>("sample_collecion");
            collection.InsertOne(new BsonDocument{ { "name", "Juan" } , { "age", 34 } });
        }
    }
}
