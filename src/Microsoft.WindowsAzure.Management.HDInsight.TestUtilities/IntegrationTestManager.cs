﻿namespace Microsoft.WindowsAzure.Management.HDInsight.TestUtilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    public class IntegrationTestManager
    {
        private Dictionary<string, AzureTestCredentials> credentialSets = new Dictionary<string, AzureTestCredentials>();
        public IntegrationTestManager()
        {
            string file = this.GetConfigPath();
            if (!string.IsNullOrEmpty(file))
            {
                if (File.Exists(file))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(List<AzureTestCredentials>));
                    using (var stream = File.OpenRead(file))
                    using (var xmlReader = XmlReader.Create(stream))
                    {
                        List<AzureTestCredentials> list = (List<AzureTestCredentials>)ser.Deserialize(xmlReader);
                        foreach (var cred in list)
                        {
                            this.credentialSets.Add(cred.CredentialsName, cred);
                        }
                    }
                }
                else
                {
                    this.MakeFile(file);
                }
            }
        }

        public AzureTestCredentials GetCredentials(string name)
        {
            AzureTestCredentials creds = null;
            this.credentialSets.TryGetValue(name, out creds);
            return creds;
        }

        public IEnumerable<AzureTestCredentials> GetAllCredentials()
        {
            return this.credentialSets.Values;
        }

        public void MakeFile(string filePath)
        {
            var def = new AzureTestCredentials();
            def.CredentialsName = "example";
            def.Certificate = @"C:\File\Path\To\Certificate\Uploaded\To\Azure\File.cer";
            def.InvalidCertificate = @"C:\File\Path\To\Certificate\NOTUploaded\To\Azure\File.cer";
            def.WellKnownCluster = new KnownCluster()
            {
                Cluster = "https://[dnsname].azurehdinsight.net:563",
                DnsName = "dnsname"
            };
            def.AzureUserName = "azureUserName";
            def.AzurePassword = "azurePassword";
            def.HadoopUserName = "hadoopUserName";

            def.Environments = new CreationDetails[1];
            var env = new CreationDetails();
            env.Location = "North Europe";
            def.EnvironmentType = EnvironmentType.Production;
            def.Endpoint = "http://foo.url -- {this is only used for non production environments}";
            def.CloudServiceName = "hdinsight -- {this is only used for non production environments}";

            env.DefaultStorageAccount = new StorageAccountCredentials();
            env.DefaultStorageAccount.Name = "blogStorageAccount";
            env.DefaultStorageAccount.Key = "blobStorageKey";
            env.DefaultStorageAccount.Container = "blogStorageContainer";
            env.AdditionalStorageAccounts = new StorageAccountCredentials[1];
            env.AdditionalStorageAccounts[0] = env.DefaultStorageAccount;
            env.HiveStores = new MetastoreCredentials[2];
            env.HiveStores[0] = new MetastoreCredentials()
            {
                SqlServer = "SqlServerLocation",
                Database = "DatabaseName",
                Description = "HiveStore1",
            };
            env.HiveStores[1] = new MetastoreCredentials()
            {
                SqlServer = "SqlServerLocation",
                Database = "DatabaseName",
                Description = "HiveStore2",
            };
            env.OozieStores = new MetastoreCredentials[2];
            env.OozieStores[0] = new MetastoreCredentials()
            {
                SqlServer = "SqlServerLocation",
                Database = "DatabaseName",
                Description = "OozieStore1",
            };
            env.OozieStores[1] = new MetastoreCredentials()
            {
                SqlServer = "SqlServerLocation",
                Database = "DatabaseName",
                Description = "OozieStore2",
            };
            List<AzureTestCredentials> data = new List<AzureTestCredentials>();
            data.Add(def);

            XmlSerializer ser = new XmlSerializer(typeof(List<AzureTestCredentials>));
            using (var stream = File.OpenWrite(filePath))
            {
                ser.Serialize(stream, data);
            }
        }

        public bool RunAzureTests()
        {
            return this.GetConfigPath() != null;
        }

        private string GetConfigPath()
        {
            return Environment.GetEnvironmentVariable("MS_HADOOP_TEST_AZURECONFIG");
        }
    }
}
