using System;
using System.Configuration;
using System.IO;
using System.Windows.Media.Imaging;

namespace mod_add.Utils
{
    public class ConfiguracionLocalServicio
    {
        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "";
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                return "";
            }
        }

        public static BitmapImage GetImage(string key)
        {
            string rutaImagen = ReadSetting(key);

            if (!string.IsNullOrEmpty(rutaImagen))
            {
                if (File.Exists(rutaImagen))
                {
                    try
                    {
                        return new BitmapImage(new Uri(rutaImagen, UriKind.Absolute));
                    }
                    catch
                    {

                    }
                }
            }

            return null;
        }

        public static TimeSpan GetTimeSpan(string key, bool required = false)
        {
            TimeSpan valueDefaultRequired = new TimeSpan(0, 0, 1);

            string valueSetting = ReadSetting(key);

            if (!string.IsNullOrEmpty(valueSetting))
            {
                try
                {
                    TimeSpan value = TimeSpan.Parse(valueSetting);

                    if (required && value == TimeSpan.Zero)
                        return valueDefaultRequired;

                    return value;
                }
                catch
                {

                }
            }

            return required ? valueDefaultRequired : TimeSpan.Zero;
        }

        public static int GetInt(string key, bool AceptNegative = false)
        {
            string valueSetting = ReadSetting(key);

            if (!string.IsNullOrEmpty(valueSetting))
            {
                try
                {
                    int value = int.Parse(valueSetting);

                    if (!AceptNegative && value < 0)
                        return 0;

                    return value;
                }
                catch
                {

                }
            }

            return 0;
        }

        public static bool GetBoolean(string key)
        {
            string valueSetting = ReadSetting(key);

            if (!string.IsNullOrEmpty(valueSetting))
            {
                if (valueSetting.Equals("SI"))
                    return true;
            }

            return false;
        }
    }
}
