using IoTFeeder.Common.Models;
using Kusto.Cloud.Platform.Data;
using Kusto.Data;
using Kusto.Data.Common;
using Kusto.Data.Net.Client;
using IoTFeeder.Common.Interfaces;
using IoTFeeder.Common.Repositories;
using Kusto.Cloud.Platform.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IoTFeeder.Data.Helper
{
    public static class AzureDataExporerHelper
    {
        //private static readonly string kustoUri = "https://iotfeeder.eastasia.kusto.windows.net";
        //private static readonly string clientId = "a0575d8f-7f60-416b-a310-dd480897eb58";
        //private static readonly string clientSecret = "AEL8Q~plFtPyqakguVV~c0mnq2CaXC5dz8_WLbPO";
        //private static readonly string tenantId = "1abc753d-89fa-4876-9875-b54ae007dd03";
        //private static readonly string databaseName = "iotfeeder_db";
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
                            else if (item.DataTypeId == 3) { tableSchema.Add($"{item.PropertyName.Replace(" ", "_")}: string"); }
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

        public static bool TableExists(string tableName)
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

                    var command = $".show table {tableName.Replace(" ", "_")} details";

                    var clientRequestProperties = CreateClientRequestProperties("IoTFeeder_ControlCommand");
                    var result = adminClient.ExecuteControlCommandAsync(databaseName, command, clientRequestProperties).GetAwaiter().GetResult().ToJObjects().ToArray();
                    if (result != null && result.Count() > 0)
                        return true;
                    else
                        return false;
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
            clientRequestProperties.SetOption("ConcurrentRequests", 100000);

            return clientRequestProperties;
        }

        public async static Task<bool> InsertData(string tableName, List<string> items)
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
                    var command = $".ingest inline into table {tableName.Replace(" ", "_")} <| \r\n";
                    foreach (var item in items)
                    {
                        command += item + "\r\n";
                    }

                    var clientRequestProperties = CreateClientRequestProperties("IoTFeeder_ControlCommand");
                    adminClient.ExecuteControlCommandAsync(databaseName, command, clientRequestProperties); //.GetAwaiter().GetResult().ToJObjects().ToArray();
                    Console.WriteLine("Inserted at " + DateTime.Now.ToString("hh:mm:ss.ffff"));
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    Console.WriteLine("Failed", ex.Message);
                }
                else
                {
                    Console.WriteLine("Failed", ex.InnerException.Message);
                }
                return false;
            }

        }

        public async static Task<bool> BulkInsertData(string tableName, List<string> items)
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
                {
                    var command = $".ingest inline into table {tableName.Replace(" ", "_")} <| \r\n";
                    foreach (var item in items)
                    {
                        command += item + "\r\n";
                    }

                    var clientRequestProperties = CreateClientRequestProperties("IoTFeeder_ControlCommand");
                    adminClient.ExecuteControlCommandAsync(databaseName, command, clientRequestProperties); //.GetAwaiter().GetResult().ToJObjects().ToArray();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task GetCount(string tableName)
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

                using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kustoConnectionString))
                {
                    using var reader = queryProvider.ExecuteQuery($"set max_memory_consumption_per_query_per_node=68719476736; \r\n{tableName} | count");
                    if (reader.Read())
                    {
                        var data = reader.GetInt64(0);
                        Console.WriteLine($"Total records in database : {reader.GetInt64(0)}");
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async Task DropTable(string tableName)
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
                {
                    using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kustoConnectionString))
                    {
                        var command = $".drop table {tableName}";

                        var clientRequestProperties = CreateClientRequestProperties("IoTFeeder_ControlCommand");
                        var result = await adminClient.ExecuteControlCommandAsync(databaseName, command, clientRequestProperties);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async Task SetCapacity(string tableName)
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
                {
                    using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kustoConnectionString))
                    {
                        //var command = $".show workload_group default";
                        //var command = $".alter-merge workload_group default ```{{\r\n  \"RequestRateLimitPolicies\": [\r\n    {{\r\n      \"IsEnabled\": true,\r\n      \"Scope\": \"WorkloadGroup\",\r\n      \"LimitKind\": \"ConcurrentRequests\",\r\n      \"Properties\": {{\r\n        \"MaxConcurrentRequests\": 2000\r\n      }}\r\n    }},\r\n    {{\r\n      \"IsEnabled\": true,\r\n      \"Scope\": \"Principal\",\r\n      \"LimitKind\": \"ConcurrentRequests\",\r\n      \"Properties\": {{\r\n        \"MaxConcurrentRequests\": 2000\r\n      }}\r\n    }},\r\n    {{\r\n      \"IsEnabled\": true,\r\n      \"Scope\": \"Principal\",\r\n      \"LimitKind\": \"ResourceUtilization\",\r\n      \"Properties\": {{\r\n        \"ResourceKind\": \"RequestCount\",\r\n        \"MaxUtilization\": 2000,\r\n        \"TimeWindow\": \"00:01:00\"\r\n      }}\r\n    }}\r\n  ],\r\n  \"RequestRateLimitsEnforcementPolicy\": {{\r\n    \"QueriesEnforcementLevel\": \"QueryHead\",\r\n    \"CommandsEnforcementLevel\": \"Database\"\r\n  }}\r\n}}```";
                        //var command = $".alter-merge workload_group default ```\r\n{{\r\n  \"RequestQueuingPolicy\": {{\r\n      \"IsEnabled\": true\r\n  }}\r\n}} ```";
                        //var command = $".alter-merge workload_group default ```\r\n{{\r\n  \"RequestRateLimitsEnforcementpolicy\": {{\r\n      \"QueriesEnforcementLevel\": \"QueryHead\",\r\n      \"CommandsEnforcementLevel\": \"Cluster\"\r\n  }}\r\n}} ```";
                        //var command = $".alter-merge workload_group default ```\r\n{{\r\n  \"RequestRateLimitPolicies\": [\r\n    {{\r\n      \"IsEnabled\": true,\r\n      \"Scope\": \"WorkloadGroup\",\r\n      \"LimitKind\": \"ConcurrentRequests\",\r\n      \"Properties\": {{\r\n        \"MaxConcurrentRequests\": 200\r\n      }}\r\n    }}\r\n  ]\r\n}}\r\n```";
                        //var command = $".show table {tableName} policy update";
                        var command = $".drop table {tableName}";
                        //var command = $".create-or-alter workload_group default\r\n\r\n```\r\n\r\n{{\r\n\r\n  \"RequestLimitsPolicy\": {{\r\n\r\n    \"DataScope\": {{\r\n\r\n      \"IsRelaxable\": true,\r\n\r\n      \"Value\": \"All\"\r\n\r\n    }},\r\n\r\n    \"MaxMemoryPerQueryPerNode\": {{\r\n\r\n      \"IsRelaxable\": true,\r\n\r\n      \"Value\": 96\r\n\r\n    }},\r\n\r\n    \"MaxMemoryPerIterator\": {{\r\n\r\n      \"IsRelaxable\": true,\r\n\r\n      \"Value\": 5368709120\r\n\r\n    }},\r\n\r\n    \"MaxFanoutThreadsPercentage\": {{\r\n\r\n      \"IsRelaxable\": true,\r\n\r\n      \"Value\": 100\r\n\r\n    }},\r\n\r\n    \"MaxFanoutNodesPercentage\": {{\r\n\r\n      \"IsRelaxable\": true,\r\n\r\n      \"Value\": 100\r\n\r\n    }},\r\n\r\n    \"MaxResultRecords\": {{\r\n\r\n      \"IsRelaxable\": true,\r\n\r\n      \"Value\": 5000\r\n\r\n    }},\r\n\r\n    \"MaxResultBytes\": {{\r\n\r\n      \"IsRelaxable\": true,\r\n\r\n      \"Value\": 67108864\r\n\r\n    }},\r\n\r\n    \"MaxExecutionTime\": {{\r\n\r\n      \"IsRelaxable\": true,\r\n\r\n      \"Value\": \"00:04:00\"\r\n\r\n    }}\r\n\r\n  }},\r\n\r\n  \"RequestRateLimitPolicies\": [\r\n\r\n    {{\r\n\r\n      \"IsEnabled\": true,\r\n\r\n      \"Scope\": \"WorkloadGroup\",\r\n\r\n      \"LimitKind\": \"ConcurrentRequests\",\r\n\r\n      \"Properties\": {{\r\n\r\n        \"MaxConcurrentRequests\": 2000\r\n\r\n      }}\r\n\r\n    }}\r\n\r\n  ],\r\n\r\n  \"RequestRateLimitsEnforcementPolicy\": {{\r\n\r\n    \"QueriesEnforcementLevel\": \"QueryHead\",\r\n\r\n    \"CommandsEnforcementLevel\": \"Database\"\r\n\r\n  }}\r\n\r\n}}\r\n\r\n ```";

                        var clientRequestProperties = CreateClientRequestProperties("IoTFeeder_ControlCommand");
                        var result = adminClient.ExecuteControlCommandAsync(databaseName, command, clientRequestProperties).GetAwaiter().GetResult().ToJObjects().ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static async Task SetLimit(string tableName)
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
                {
                    using (var queryProvider = KustoClientFactory.CreateCslQueryProvider(kustoConnectionString))
                    {
                        //var command = $".alter cluster policy capacity ```\r\n{{\r\n  \"IngestionCapacity\": {{\r\n    \"ClusterMaximumConcurrentOperations\": 2000,\r\n    \"CoreUtilizationCoefficient\": 1\r\n  }}\r\n}}```";
                        var command = $".alter table {tableName} policy ingestionbatching\r\n```\r\n{{\r\n    \"MaximumBatchingTimeSpan\" : \"00:00:30\",\r\n    \"MaximumNumberOfItems\" : 500,\r\n    \"MaximumRawDataSizeMB\": 1024\r\n}}\r\n```";

                        var clientRequestProperties = CreateClientRequestProperties("IoTFeeder_ControlCommand");
                        var result = adminClient.ExecuteControlCommandAsync(databaseName, command, clientRequestProperties).GetAwaiter().GetResult().ToJObjects().ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
