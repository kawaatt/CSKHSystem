namespace ADMIN.Extensions
{
    using ADMIN.Models.ViewModel.AplicationVm;
    using Newtonsoft.Json;

    public static class ObjectConverterExtenstion
    {
        /// <summary>
        /// Converts a dynamic object to a specific model using Newtonsoft.Json.
        /// </summary>
        /// <typeparam name="T">The type of the model to convert to.</typeparam>
        /// <param name="dynamicObject">The dynamic object to convert.</param>
        /// <returns>An instance of the specified model type.</returns>
        public static T ConvertToModel<T>(object dynamicObject)
        {
            if (dynamicObject == null) throw new ArgumentNullException(nameof(dynamicObject));

            // Serialize the dynamic object to a JSON string
            string json = JsonConvert.SerializeObject(dynamicObject);
            
            // Deserialize the JSON string to the specified model
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public static T ConvertStringToModel<T>(string dynamicObject)
        {
            if (string.IsNullOrEmpty( dynamicObject)) throw new ArgumentNullException(nameof(dynamicObject));

            // Deserialize the JSON string to the specified model
            return JsonConvert.DeserializeObject<T>(dynamicObject, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
