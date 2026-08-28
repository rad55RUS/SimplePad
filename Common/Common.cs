using System.Collections.Generic;

namespace SimplePad
{
    /// <summary>
    /// Provides utility methods for common operations across the application
    /// </summary>
    public class Common
    {
        /// <summary>
        /// Adds the properties of the specified settings object to the provided dictionary, using the specified section name as a prefix for the keys.
        /// </summary>
        /// <param name="dictionary">The dictionary to which the properties will be added.</param>
        /// <param name="sectionName">The section name to use as a prefix for the keys.</param>
        /// <param name="settings">The object whose properties will be added to the dictionary.</param>
        public static void AddPropertiesToDictionary(Dictionary<string, string?> dictionary, string sectionName, object obj)
        {
            foreach (var property in obj.GetType().GetProperties())
            {
                var value = property.GetValue(obj);
                if (value != null)
                {
                    var key = $"{sectionName}:{property.Name}";
                    dictionary[key] = value.ToString();
                }
            }
        }
    }
}
