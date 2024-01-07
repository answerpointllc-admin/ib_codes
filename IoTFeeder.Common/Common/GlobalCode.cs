//using GemBox.Document;
using IoTFeeder.Common.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Color = System.Drawing.Color;

namespace IoTFeeder.Common.Common
{
    public static class GlobalCode
    {
        public static string strEncryptionKey = "IoTFeeder";

        public static string ActiveText = "Active";
        public static string InActiveText = "Inactive";

        public static string FrequencyRandom = "Random";
        public static string FrequencyFix = "Fixed";

        public static string PSActiveText = "Readily Available";
        public static string PSInActiveText = "Prepare on Order";

        public static string YesText = "Yes";
        public static string NoText = "No";

        public static string InfoCulture = "en-US";
        public static byte ButtonCount = 5;
        public static byte PageSize = 10;
        public static byte MobilePageSize = 15;
        public static int[] RecordPerPageList = new[] { 5, 10, 20, 50, 100, 200, 500 };
        public static string DefaultDateFormat = "MM/dd/yyyy";
        public static string DDMMYYFormat = "dd/MM/yyyy";
        public static string NewDDMMYYFormat = "dd MMM, yyyy";
        public static string LongDateFormat = "MMM dd, yyyy";
        public static string GridDateFormat = "dddd dd MMM,yyyy";
        public static string DefaultTimeFormat = "HH:mm";
        public static string DefaultGridDateTimeFormat = "{0: dd/MM/yyyy HH:mm}";
        public static string DefaultGridDateFormat = "{0: dd/MM/yyyy}";
        public static string USALongDateFormat = "MMM/dd/yyyy";
        public static string USALongDateTimeFormat = "MMM/dd/yyyy hh:mm tt";
        public static string DateTimeUsaFormat = "MMM dd, yyyy hh:mm tt";
        public static string ShortDateTimeUsaFormat = "MM-dd-yyyy HH:mm";
        public static string LongDateTimeFormat = "MMM dd, yyyy hh:mm tt";
        public static string TimeFormat = "HH:mm tt";
        public static string BookDateTime = "MMM dd, yyyy (hh:mm tt)";
        public static string ApiRequestDateFormat = "MM-dd-yyyy HH:mm";
        public static string ViewDateTime = "MMM dd, yyyy 'at' (hh:mm tt)";
        public static string DefaultDateTimeFormat = "dd/MM/yyyy HH:mm";
        public static string DefaultDateTimeFormatWith12 = "dd/MM/yyyy hh:mm tt";
        public static string DefaultYearWiseDateTimeFormat = "yyyy/M/d HH:mm";
        public static string DateTimePickerFormat = "yyyy/M/d";
        public static string DefaultUTCDateFormat = "yyyy-MM-dd HH:mm:ss.000Z";
        public static string DefaultDateFormatIoT = "yyyy-MM-dd HH:mm:ss";

        public static string UserFiles = "appdata//UserFiles//";
        public static string DefaultCarImage = "featured-1.jpg";
        public static string DefaultUserProfileImage = "user.png";

        public static string foreignKeyReference = "The DELETE statement conflicted with the REFERENCE constraint";

        public static int AdminRoleID = 1;
        public static int ProfileImageSize = 1; //1mb

        public static string[] AllowImgFileExt = new[] { "jpg", "jpeg", "png", "bmp", "JPG", "JPEG", "PNG", "BMP" };
        public static string[] AllowAttechmentFileExt = new[] { "jpg", "jpeg", "png", "bmp", "doc", "docx", "xls", "xlsx", "odt", "ppt", "pptx", "pdf", "ods", "odp", "txt" };
        public static string[] AllowDocumentFileExt = new[] { "jpg", "jpeg", "png", "doc", "docx", "pdf" };

        public static int DefaultMessageCount = 30;
        public static int CacheTime = 1200; // minutes
        public static int StudentUploadDocumentLimit = 5;
        public static int OTP = 1234;
        public static int VisibilityTypeId = 23;

        public static string OneSignal_APPID = "f7f836c9-bc69-4023-99a6-2b9281e28135";
        public static string OneSignal_RESTAPI = "Yzc2ZmVmYWItNTYxNy00OWY4LWI4NmQtNmRmYzEyOGViZTJm";

        #region Action Enum
        public enum Actions
        {
            Index,
            Create,
            Edit,
            Delete,
            Detail,
            Search
        }
        #endregion

        public static string RemoveSpecialCharacters(string input)
        {
            Regex reg = new Regex("[.$-*'\",_&#^@]");
            return reg.Replace(input, string.Empty);
        }

        //#region Send Email
        //public static bool SendEmail(string recipients, string subject, string emailBody, CommonSettingViewModel commonSettingViewModel)
        //{
        //    try
        //    {
        //        using (MailMessage mailMessage = new MailMessage())
        //        {
        //            mailMessage.From = new MailAddress(commonSettingViewModel.Email ?? string.Empty);
        //            mailMessage.To.Add(recipients);
        //            mailMessage.Subject = subject;
        //            mailMessage.Body = emailBody;
        //            mailMessage.IsBodyHtml = true;
        //            mailMessage.Priority = MailPriority.High;

        //            var port = commonSettingViewModel.Port;
        //            using (SmtpClient smtpClient = new SmtpClient(commonSettingViewModel.SMTPServer, Convert.ToInt32(port)))
        //            {
        //                smtpClient.Credentials = new NetworkCredential(commonSettingViewModel.Email, commonSettingViewModel.Password);
        //                smtpClient.EnableSsl = true;
        //                smtpClient.Send(mailMessage);
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        //#endregion

        #region Convert List to Json string
        public static string ObjectToString(object data)
        {
            if (data != null)
            {
                return JsonConvert.SerializeObject(data);
            }
            return string.Empty;
        }
        #endregion

        #region Generate Random Code
        public static string RandomString(int length, bool? isNumberOnly = false, bool? hasCapitalOnly = false)
        {
            Random random = new Random();
            string output = "";
            string chars = (isNumberOnly.HasValue && isNumberOnly.Value) ? "0123456789" : ((hasCapitalOnly ?? false) ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

            output = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());

            return output;
        }
        #endregion

        public static string GetEnumDescription(this Enum value)
        {
            // get attributes  
            var field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                // return description
                return attributes.Any() ? ((DescriptionAttribute)attributes.ElementAt(0)).Description : string.Empty;
            }
            else { return string.Empty; }
        }

        /*public static (string,string) GetEnumMultipleDescription(this Enum value)
        {
            // get attributes  
            var field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var attributes = field.GetCustomAttributes(typeof(ExtraDescriptionAttribute), false);

                // return description
                string first_desc = attributes.Any() ? ((ExtraDescriptionAttribute)attributes.ElementAt(0)).Description : string.Empty;
                string second_desc = attributes.Any() ? ((ExtraDescriptionAttribute)attributes.ElementAt(0)).ExtraInfo : string.Empty;
                return (first_desc, second_desc);
            }
            else { return (string.Empty, string.Empty); }
        }*/

        #region get cropped image
        public static byte[] Crop(Image OriginalImage, int Width, int Height, int X, int Y, int rHeight = 0, int rWidth = 0)
        {
            try
            {
                if (rWidth == 0) { rWidth = Width; }
                if (rHeight == 0) { rHeight = Height; }
                using (Bitmap bmp = new Bitmap(rWidth, rHeight))
                {
                    using (Graphics Graphic = Graphics.FromImage(bmp))
                    {
                        Graphic.SmoothingMode = SmoothingMode.AntiAlias;

                        Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        Graphic.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                        Graphic.DrawImage(OriginalImage, new Rectangle(0, 0, rWidth, rHeight), X, Y, Width, Height, GraphicsUnit.Pixel);

                        MemoryStream ms = new MemoryStream();
                        bmp.Save(ms, OriginalImage.RawFormat);
                        return ms.GetBuffer();
                    }
                }
            }

            catch
            {
                throw;
            }
        }
        #endregion

        #region Send Push Notification

        public static string SendPushNotification(string PlayerID, string Title, string Message, int notificationType, int? bookingId = 0)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", "Basic " + OneSignal_RESTAPI);

            byte[] byteArray = Encoding.UTF8.GetBytes("{"
                                                    + "\"app_id\": \"" + OneSignal_APPID + "\","
                                                    + "\"contents\": {\"en\": \"" + Message + "\"},"
                                                    + "\"headings\": {\"en\": \"" + Title + "\"},"
                                                    + "\"data\": {\"type\": \"" + notificationType + "\", \"bookingid\": \"" + bookingId + "\"},"
                                                    + "\"include_player_ids\": [\"" + PlayerID + "\"]}");
            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }
            return responseContent;
        }

        #endregion

        #region Convert string to base64
        public static string Base64Encode(string text)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        #endregion

        #region Convert base64 to string
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        public static List<TEnum> GetEnumList<TEnum>() where TEnum : Enum => ((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();

        public static void WriteException(string cmdText = "", Exception ex = null)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//appdata//ErrorLog//ErrorLog.txt");
            if (!File.Exists(filePath))
            {
                using (File.Create(filePath)) { };
            }
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("---------------------------------------New Log--------------------------------------");
                writer.WriteLine("Date : " + DateTime.UtcNow.ToString());

                if (!(string.IsNullOrEmpty(cmdText)))
                    writer.WriteLine("Log: " + cmdText.ToString());

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    if (!(string.IsNullOrEmpty(ex.StackTrace)))
                        writer.WriteLine("StackTrace : " + ex.StackTrace);
                    if (ex.InnerException != null)
                        writer.WriteLine("InnerException : " + ex.InnerException.ToString());
                    if (!(string.IsNullOrEmpty(cmdText)))
                        writer.WriteLine("CommandText ErrorCommand: " + cmdText.ToString());
                    ex = ex.InnerException;
                }
                writer.WriteLine();
            }
        }

        public static List<T> GetListFromXML<T>(string tagName, string content)
        {
            var str = "<" + tagName + ">" + content + "</" + tagName + ">";
            var dataList = new List<T>();
            var serializer = new XmlSerializer(typeof(List<T>), new XmlRootAttribute(tagName));
            using (TextReader reader = new StringReader(str))
            {
                dataList = (List<T>)serializer.Deserialize(reader);
            }

            return dataList;
        }

        public static List<DateTime> GetTimeSlots(int duration, List<Tuple<DateTime, DateTime>> dateTimes, List<Tuple<DateTime, DateTime>> currentBookings, string offset = "")
        {
            List<DateTime> startTimeSlots = new List<DateTime>();
            foreach (var item in dateTimes)
            {
                DateTime startTime = item.Item1;
                bool status = true;
                while (status)
                {
                    DateTime endTime = startTime.AddMinutes(duration);
                    if (endTime > item.Item2)
                    {
                        status = false;
                        break;
                    }
                    else
                    {
                        if (currentBookings.FindAll(e => (startTime > e.Item1 && startTime < e.Item2) || (endTime > e.Item1 && (endTime <= e.Item2 || startTime < e.Item1)) || startTime == e.Item1 || endTime == e.Item2).Any())
                        {
                            startTime = startTime.AddMinutes(15);
                            continue;
                        }
                        startTimeSlots.Add(startTime);
                        startTime = startTime.AddMinutes(15);
                    }
                }
            }

            List<DateTime> finalTime = new List<DateTime>();
            startTimeSlots = startTimeSlots.OrderBy(a => a).ToList();
            foreach(DateTime item in startTimeSlots)
            {
                finalTime.Add(ConvertUTCToLocalTimezone(item, offset));
            }
            return finalTime;
        }

        public static DateTime ConvertLocalToUserTimeZone(this DateTime dateTime, string timeOffSet)
        {
            if (!string.IsNullOrWhiteSpace(timeOffSet))
            {
                TimeSpan offset;
                if (timeOffSet.StartsWith("-"))
                { offset = -TimeSpan.ParseExact(timeOffSet, @"\-hhmm", null, System.Globalization.TimeSpanStyles.None); }
                else
                { offset = TimeSpan.ParseExact(timeOffSet.Replace("+", ""), @"hhmm", null, System.Globalization.TimeSpanStyles.None); }
                DateTimeOffset dateTimeOffset = new DateTimeOffset(dateTime, offset);
                return dateTimeOffset.UtcDateTime;
            }
            else return dateTime;
        }

        public static DateTime ConvertUTCToLocalTimezone(this DateTime dateTime, string timeOffSet)
        {
            if (!string.IsNullOrWhiteSpace(timeOffSet))
            {
                TimeSpan offset;
                if (timeOffSet.StartsWith("-"))
                { offset = -TimeSpan.ParseExact(timeOffSet, @"\-hhmm", null, System.Globalization.TimeSpanStyles.None); }
                else
                { offset = TimeSpan.ParseExact(timeOffSet.Replace("+", ""), @"hhmm", null, System.Globalization.TimeSpanStyles.None); }

                DateTimeOffset dateTimeOffset = TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.CreateCustomTimeZone("Custom", offset, "Local", "Local"));

                return dateTimeOffset.LocalDateTime;
            }
            else return dateTime;
        }
    }
}