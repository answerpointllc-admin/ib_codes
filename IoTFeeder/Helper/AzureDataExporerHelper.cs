using IoTFeeder.Common.Models;
using Kusto.Cloud.Platform.Data;
using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using IoTFeeder.Common.Interfaces;
using IoTFeeder.Common.Repositories;


namespace IoTFeeder.Helper
{
    public static class AzureDataExporerHelper
    {
        public static string kustoUri { get; set; }
        public static string clientId { get; set; }
        public static string clientSecret { get; set; }
        public static string tenantId { get; set; }
        public static string databaseName { get; set; }

        public static bool CreateTable(IoTDevicePropertyViewModel viewModel, string tableName)
        {
            try
            {
                var kustoConnectionString = new KustoConnectionStringBuilder(kustoUri)
                {
                    FederatedSecurity = true,
                    InitialCatalog = databaseName,
                    ApplicationClientId = clientId,
                    ApplicationKey = clientSecret,
                    Authority = tenantId,
                };

                using (var adminClient = KustoClientFactory.CreateCslAdminProvider(kustoConnectionString))
                using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kustoConnectionString))
                {
                    List<string> tableSchema = new List<string>();
                    if (viewModel.ioTDeviceProperties != null && viewModel.ioTDeviceProperties.Count > 0)
                    {
                        foreach (var item in viewModel.ioTDeviceProperties)
                        {
                            if (item.DataTypeId == 1) { tableSchema.Add($"{item.PropertyName.Replace(" ", "_")}: int32"); }
                            else if (item.DataTypeId == 2) { tableSchema.Add($"{item.PropertyName.Replace(" ", "_")}: decimal"); }
                            else if (item.DataTypeId == 3) { tableSchema.Add($"{item.PropertyName.Replace(" ","_")}: string"); }
                            else if (item.DataTypeId == 4) { tableSchema.Add($"{item.PropertyName.Replace(" ", "_")}: boolean"); }
                            else if (item.DataTypeId == 5) { tableSchema.Add($"{item.PropertyName.Replace(" ", "_")}: datetime"); }
                        }
                    }

                    var command = $".create table {tableName.Replace(" ", "_")} ({string.Join(", ", tableSchema)})";

                    var clientRequestProperties = CreateClientRequestProperties("IoTFeeder_ControlCommand");
                    var result = adminClient.ExecuteControlCommandAsync(databaseName, command, clientRequestProperties).GetAwaiter().GetResult().ToJObjects().ToArray();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool DeleteTable(string tableName)
        {
            try
            {
                var kustoConnectionString = new KustoConnectionStringBuilder(kustoUri)
                {
                    FederatedSecurity = true,
                    InitialCatalog = databaseName,
                    ApplicationClientId = clientId,
                    ApplicationKey = clientSecret,
                    Authority = tenantId,
                };

                using (var adminClient = KustoClientFactory.CreateCslAdminProvider(kustoConnectionString))
                using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kustoConnectionString))
                {
                    List<string> tableSchema = new List<string>();
                    
                    var command = $".drop table {tableName.Replace(" ", "_")} ifexists";

                    var clientRequestProperties = CreateClientRequestProperties("IoTFeeder_ControlCommand");
                    var result = adminClient.ExecuteControlCommandAsync(databaseName, command, clientRequestProperties).GetAwaiter().GetResult().ToJObjects().ToArray();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static ClientRequestProperties CreateClientRequestProperties(string scope, string timeout = null)
        {
            var clientRequestProperties = new ClientRequestProperties
            {
                Application = "IoTFeeder.csproj",
                ClientRequestId = $"{scope};{Guid.NewGuid().ToString()}"
            };
            if (timeout != null)
            {
                clientRequestProperties.SetOption(ClientRequestProperties.OptionServerTimeout, timeout);
            }

            return clientRequestProperties;
        }

        public static bool InsertData(string tableName, List<string> items) {
            try
            {
                var kustoConnectionString = new KustoConnectionStringBuilder(kustoUri)
                {
                    FederatedSecurity = true,
                    InitialCatalog = databaseName,
                    ApplicationClientId = clientId,
                    ApplicationKey = clientSecret,
                    Authority = tenantId,
                };

                using (var adminClient = KustoClientFactory.CreateCslAdminProvider(kustoConnectionString))
                using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kustoConnectionString))
                {
                    var command = $".ingest inline into table {tableName.Replace(" ", "_")} \r\n";
                    foreach (var item in items)
                    {
                        command += item + "\r\n";
                    }

                    var clientRequestProperties = CreateClientRequestProperties("IoTFeeder_ControlCommand");
                    var result = adminClient.ExecuteControlCommandAsync(databaseName, command, clientRequestProperties).GetAwaiter().GetResult().ToJObjects().ToArray();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
