using IoTFeeder.Common.Common;
using IoTFeeder.Common.DB;
using IoTFeeder.Common.Interfaces;
using IoTFeeder.Common.Models;
using IoTFeeder.Common.Repositories;
using IoTFeeder.Data.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Security.Permissions;

namespace IoTFeeder.Data
{
    class Program
    {
        int totalInserted = 0;
        static void Main(string[] args)
        {
            #region Check Existance of connection string
            if (ConfigurationManager.AppSettings["IoTFeeder_Connection"] == null)
            {
                Console.WriteLine("Connection string is missing. Please add needed connection string and restart the app");
                Console.Read();
            }
            #endregion

            #region Members Declaration
            string connectionString = ConfigurationManager.AppSettings["IoTFeeder_Connection"].ToString();
            var services = new ServiceCollection()
                .AddDbContext<IotDataFeederContext>(options => options.UseSqlServer(connectionString))
                .AddScoped<IIoTDevice, IoTDeviceRepository>()
                .AddScoped<IIoTDeviceProperty, IoTDevicePropertyRepository>()
                .AddScoped<ICommonSettings, CommonSettingsReepository>()
                .BuildServiceProvider();

            var _ioTDeviceRepository = services.GetService<IIoTDevice>();
            var _ioTDevicePropertyRepository = services.GetService<IIoTDeviceProperty>();
            var _commonSettingsRepository = services.GetService<ICommonSettings>();

            var threads = new List<Thread>();
            #endregion

            if (_ioTDeviceRepository != null && _ioTDevicePropertyRepository != null && _commonSettingsRepository != null)
            {
                var commonSettingData = _commonSettingsRepository.GetCommonSetting();
                AzureDataExporerHelper.kustoUri = commonSettingData.KustoUri;
                AzureDataExporerHelper.clientId = commonSettingData.ClientId;
                AzureDataExporerHelper.clientSecret = commonSettingData.ClientSecret;
                AzureDataExporerHelper.tenantId = commonSettingData.TenantId;
                AzureDataExporerHelper.databaseName = commonSettingData.DatabaseName;

                var deviceList = _ioTDeviceRepository.GetDevices();
                if (deviceList != null && deviceList.Count > 0)
                {
                    foreach (var item in deviceList)
                    {
                        Console.WriteLine("================");
                        Console.WriteLine("Initializing and starting a new thread for " + item.DeviceName);
                        var thread = new Thread(DoWork)
                        {
                            Name = string.Format("Started a new thread for " + item.DeviceName)
                        };
                        thread.Start(item);
                        threads.Add(thread);
                        //threads.ForEach(t => t.Join());
                    }
                }
            }
        }

        static async void DoWork(object threadState)
        {
            var item = threadState as IoTDeviceViewModel;
            if (item != null)
            {
                #region Check If Devices Exists over ADE or not, if not exists then create it.
                Console.WriteLine("Checking if ADE object exists or not?");
                await AzureDataExporerHelper.GetCount(item.DeviceName.Replace(" ", "_"));
                await AzureDataExporerHelper.SetCapacity(item.DeviceName.Replace(" ", "_"));
                Console.WriteLine("Table deleted successfully");
                bool isDeviceExists = AzureDataExporerHelper.TableExists(item.DeviceName.Replace(" ", "_"));
                if (!isDeviceExists)
                {
                    Console.WriteLine("ADE object does not exists, so creating a new ADE object.");
                    string deviceName = string.Empty;
                    var deviceDetail = item.ioTDeviceProperties;
                    if (deviceDetail == null || deviceDetail.Count == 0)
                    {
                        threadState = false;
                    }
                    IoTDevicePropertyViewModel objList = new IoTDevicePropertyViewModel();
                    objList.ioTDeviceProperties = item.ioTDeviceProperties;
                    bool isTableCreated = AzureDataExporerHelper.CreateTable(objList, item.DeviceName);
                    if (!isTableCreated)
                        threadState = false;
                }
                Console.WriteLine("ADE object exists.");
                //await AzureDataExporerHelper.GetCount(item.DeviceName.Replace(" ", "_"));
                #endregion

                Console.WriteLine("Starting with an actual process and generating random data set");
                decimal deltaFrequency = 0;
                List<TimeModel> listOfTime = new List<TimeModel>();
                if (item.FrequencyType == false)
                {
                    DateTime startTime = DateTime.Now;
                    Console.WriteLine("Start Time " + startTime);
                    Console.WriteLine(item.DeviceName + " is having fixed frequency type.");
                    Console.WriteLine("Calculating time frequency for " + item.DeviceName);
                    Console.WriteLine("Time frequency is = 60 / " + item.Frequency);
                    var totalRecords = Convert.ToDecimal(item.Frequency.HasValue ? item.Frequency.Value : 1);

                    for (; ; )
                    {
                        deltaFrequency = Convert.ToDecimal(60) / totalRecords;
                        Console.WriteLine("Calcuated frequency is " + deltaFrequency.ToString("0.0#"));
                        Console.WriteLine("Initializing and starting up a separate thread for " + item.DeviceName + " to generate a dataset every " + deltaFrequency.ToString("0.0#"));

                        if (totalRecords > 200)
                        {
                            var recordsPerSecond = Math.Floor(totalRecords / 60);
                            item.IsBulkInsert = true;

                            for (int i = 0; i < 60; i++)
                            {
                                var singlItem = item;
                                if (59 > i)
                                {
                                    totalRecords -= recordsPerSecond;
                                }
                                else
                                {
                                    recordsPerSecond = totalRecords; totalRecords = 0;
                                }

                                singlItem.RecordPerSecond = (int)recordsPerSecond;

                                DataInsert(singlItem);

                                Thread.Sleep(1000);
                            }
                        }
                        else
                        {
                            TimerCallback callback = new TimerCallback(Tick);
                            int miliseconds = Convert.ToInt32(deltaFrequency * 1000);

                            Console.WriteLine("Every " + miliseconds + " miliseconds a data set will be pushed for " + item.DeviceName);
                            Timer stateTimer = new Timer(callback, item, 0, miliseconds);

                            var totalLoop = (60 * 1000) / miliseconds;
                            for (int i = 0; i < totalLoop; i++)
                            {
                                Thread.Sleep(miliseconds);
                            }
                        }
                    }
                }
                else // Else - frequency type is random
                {
                    Console.WriteLine(item.DeviceName + " is having random frequency type.");
                    Console.WriteLine("Calculating time frequency for " + item.DeviceName);

                    for (; ; )
                    {
                        var randomData = new Random();
                        var newFrequency = randomData.Next(item.MinValue ?? 0, item.MaxValue ?? 0);
                        var totalRecords = newFrequency;
                        deltaFrequency = (60m / (newFrequency > 0 ? newFrequency : 1m));
                        Console.WriteLine("Calcuated frequency is " + deltaFrequency.ToString("0.00"));
                        Console.WriteLine("Initializing and starting up a separate thread for " + item.DeviceName + " to generate a dataset every " + deltaFrequency.ToString("0.00"));

                        #region Generate Random Time and Records
                        Random random = new Random();
                        while (listOfTime.Sum(x => x.seconds) < 60)
                        {
                            var newValue = random.Next(1, 10);
                            if (listOfTime.Sum(x => x.seconds) + newValue <= 60)
                            {
                                listOfTime.Add(new TimeModel { seconds = newValue });
                            }
                        }

                        int maxRecords = newFrequency;
                        for (int i = 0; i < listOfTime.Count - 1; i++)
                        {
                            int maxValue = (maxRecords / 5) > 0 ? maxRecords / 5 : 1;
                            var newValue = 0;
                            while (newValue == 0)
                            {
                                newValue = random.Next(1, maxValue);
                            }
                            if (listOfTime.Sum(x => x.records) + newValue <= newFrequency)
                            {
                                maxRecords -= newValue;
                                listOfTime[i].records = newValue;
                            }
                        }
                        listOfTime[listOfTime.Count - 1].records = maxRecords;

                        Console.WriteLine("Total insert - {0}", totalRecords);
                        #endregion

                        if (totalRecords > 200)
                        {
                            item.IsBulkInsert = true;
                            foreach (var timeItem in listOfTime)
                            {
                                var recordsPerSecond = Math.Floor((decimal)timeItem.records / timeItem.seconds);
                                var remainingRecords = (decimal)timeItem.records;
                                for (int i = 0; i < timeItem.seconds; i++)
                                {
                                    var singlItem = item;
                                    if (timeItem.seconds - 1 > i)
                                    {
                                        remainingRecords -= recordsPerSecond;
                                    }
                                    else
                                    {
                                        recordsPerSecond = remainingRecords; remainingRecords = 0;
                                    }

                                    singlItem.RecordPerSecond = (int)recordsPerSecond;

                                    DataInsert(singlItem);

                                    Thread.Sleep(1000);
                                }
                            }
                        }
                        else
                        {
                            foreach (var timeItem in listOfTime)
                            {
                                int miliseconds = Convert.ToInt32(timeItem.seconds * 1000);
                                for (int i = 0; i < timeItem.records; i++)
                                {
                                    int submiliseconds = Convert.ToInt32(((decimal)timeItem.seconds / (decimal)timeItem.records) * 1000);
                                    Console.WriteLine("Wait " + ((decimal)timeItem.seconds / (decimal)timeItem.records) + " miliseconds for next record");
                                    TimerCallback callback = new TimerCallback(Tick);
                                    Console.WriteLine("Every " + submiliseconds + " miliseconds a data set will be pushed for " + item.DeviceName);
                                    Timer stateTimer = new Timer(callback, item, 0, submiliseconds);
                                    Thread.Sleep(submiliseconds);
                                }
                                Console.WriteLine("Every " + miliseconds + " miliseconds a " + timeItem.records + " data set will be pushed for " + item.DeviceName);
                                Thread.Sleep(miliseconds);
                            }
                        }
                    }
                }
            }
        }

        static void DataInsert(IoTDeviceViewModel item)
        {
            List<string> bulkData = new List<string>();

            for (int i = 0; i < item.RecordPerSecond; i++)
            {
                List<string> propertyVal = new List<string>();
                foreach (var deviceProperties in item.ioTDeviceProperties)
                {
                    if (deviceProperties.DataTypeId == 1)
                    {
                        var randomInt = new Random().Next(Convert.ToInt32(deviceProperties.MininmumValue) < 0 ? 1 : Convert.ToInt32(deviceProperties.MininmumValue), Convert.ToInt32(deviceProperties.MaximumValue));
                        propertyVal.Add(randomInt.ToString());
                    }
                    else if (deviceProperties.DataTypeId == 2)
                    {
                        string strVal = RandomDecimal(deviceProperties.MininmumValue ?? 0, deviceProperties.MaximumValue ?? 0);
                        propertyVal.Add(strVal);
                    }
                    else if (deviceProperties.DataTypeId == 3)
                    {
                        var randomString = new Random().Next(1, Convert.ToInt32(deviceProperties.MaxLength));
                        string strVal = RandomString(randomString);
                        propertyVal.Add(strVal);
                    }
                    else if (deviceProperties.DataTypeId == 4)
                    {
                        var random = new Random();
                        string strVal = random.Next(2) == 1 ? "1" : "0";
                        propertyVal.Add(strVal);
                    }
                    else if (deviceProperties.DataTypeId == 5)
                    {
                        string strVal = RandomDateTime();
                        propertyVal.Add(strVal);
                    }
                }

                if (item.RecordPerSecond == 1)
                {
                    if (propertyVal.Count > 0)
                    {
                        TimeCount.Count++;
                        AzureDataExporerHelper.InsertData(item.DeviceName, new List<string>() { string.Join(",", propertyVal) });
                    }
                }
                else
                {
                    bulkData.Add(string.Join(",", propertyVal));
                }
            }

            if (bulkData.Count > 0)
            {
                TimeCount.Count++;
                AzureDataExporerHelper.BulkInsertData(item.DeviceName, bulkData).GetAwaiter().GetResult();
                foreach (var recordItem in bulkData)
                {
                    Console.WriteLine("Inserted " + recordItem + " into " + item.DeviceName + " at " + DateTime.Now.ToString("hh:mm:ss.ffff"));
                }
            }
        }

        static public void Tick(Object stateInfo)
        {
            IoTDeviceViewModel item = (IoTDeviceViewModel)stateInfo;
            List<string> propertyVal = new List<string>();
            foreach (var deviceProperties in item.ioTDeviceProperties)
            {
                if (deviceProperties.DataTypeId == 1)
                {
                    var randomInt = new Random().Next(Convert.ToInt32(deviceProperties.MininmumValue) < 0 ? 1 : Convert.ToInt32(deviceProperties.MininmumValue), Convert.ToInt32(deviceProperties.MaximumValue));
                    propertyVal.Add(randomInt.ToString());
                }
                else if (deviceProperties.DataTypeId == 2)
                {
                    string strVal = RandomDecimal(deviceProperties.MininmumValue ?? 0, deviceProperties.MaximumValue ?? 0);
                    propertyVal.Add(strVal);
                }
                else if (deviceProperties.DataTypeId == 3)
                {
                    var randomString = new Random().Next(1, Convert.ToInt32(deviceProperties.MaxLength));
                    string strVal = RandomString(randomString);
                    propertyVal.Add(strVal);
                }
                else if (deviceProperties.DataTypeId == 4)
                {
                    var random = new Random();
                    string strVal = random.Next(2) == 1 ? "1" : "0";
                    propertyVal.Add(strVal);
                }
                else if (deviceProperties.DataTypeId == 5)
                {
                    string strVal = RandomDateTime();
                    propertyVal.Add(strVal);
                }
            }
            if (propertyVal.Count > 0)
            {
                AzureDataExporerHelper.InsertData(item.DeviceName, new List<string>() { string.Join(", ", propertyVal) }).GetAwaiter().GetResult();
                Console.WriteLine("Inserted " + string.Join(", ", propertyVal) + " into " + item.DeviceName + " at " + DateTime.Now.ToString("hh:mm:ss.ffff"));
            }
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            string output = "";
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            output = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
            return output;
        }
        public static string RandomDecimal(decimal min, decimal max)
        {
            Random rand = new Random();
            var output = ((decimal)rand.Next((int)(min * 100.0M), (int)(max * 100.0M))) / 100.0M;
            return output.ToString();
        }
        public static string RandomDateTime()
        {
            Random gen = new Random();
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range)).ToString(GlobalCode.DefaultDateFormatIoT);
        }
    }
}

public class TimeModel
{
    public int seconds { get; set; }
    public int records { get; set; }
}

public static class TimeCount
{
    public static int Count { get; set; }
}