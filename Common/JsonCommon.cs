using Avalonia.Controls.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace SimplePad
{
    /// <summary>
    /// Provides static methods for working with <i>.json</i> markup via <b>Newtonsoft.Json</b> library
    /// </summary>
    public static class JsonCommon
    {
        /// <summary>
        /// Converter for <see cref="DateTime"/> values to be used in <i>.json</i> serialization and deserialization, with a specific date format of <b>"dd.MM.yyyy HH:mm:ss"</b>
        /// </summary>
        private static readonly IsoDateTimeConverter _dateConverter = new()
        {
            DateTimeFormat = "dd.MM.yyyy HH:mm:ss"
        };

        #region String methods
        /// <summary>
        /// Serializes <see cref="object"/> of any type to <i>.json</i> markup <see cref="string"/>
        /// </summary>
        /// <param name="obj"><see cref="object"/> that must be serialized</param>
        /// <returns>Serialized <i>.json</i> markup <see cref="string"/></returns>
        public static string SerializeJson(object obj)
        { 
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
        }

        /// <summary>
        /// Deserializes a <i>.json</i> markup <see cref="string"/> to an object of the specified type
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to</typeparam>
        /// <param name="jsonStr">The <i>.json</i> markup <see cref="string"/> to deserialize</param>
        /// <returns>The deserialized object</returns>
        /// <exception cref="Exception">Thrown when deserialization fails</exception>
        public static T GetDeserializedJson<T>(string jsonStr)
        {
            T? obj = default;
            try
            {
                obj = JsonConvert.DeserializeObject<T>(jsonStr, new IsoDateTimeConverter { DateTimeFormat = "dd.MM.yyyy HH:mm:ss" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            if (obj == null)
            {
                throw new Exception("Deserialization error");
            }
            return obj;
        }
        #endregion

        #region Stream methods
        /// <summary>
        /// Deserializes a <i>.json</i> markup from a <see cref="Stream"/> to an object of the specified type
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to</typeparam>
        /// <param name="jsonStream">The <i>.json</i> markup <see cref="Stream"/> to deserialize</param>
        /// <returns>The deserialized object</returns>
        /// <exception cref="Exception">Thrown when deserialization fails</exception>
        public static T? GetDeserializedJson<T>(Stream jsonStream)
        {
            T? obj = default;
            try
            {
                using var streamReader = new StreamReader(jsonStream);
                using var jsonReader = new JsonTextReader(streamReader);

                JsonSerializer serializer = new();
                serializer.Converters.Add(_dateConverter);

                obj = serializer.Deserialize<T>(jsonReader);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            if (obj == null)
            {
                throw new Exception("Deserialization error");
            }
            return obj;
        }
        #endregion
    }
}
