using System.Reflection;

namespace OAuthCore.Infrastructure.Configuration;

public static class EnvSettingsMapping
{
    public static T MapTo<T>() where T : new()
    {
        var settings = new T(); // Create an instance of the settings class
        var settingsType = typeof(T); // Get the type of the class

        foreach (var property in settingsType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var envVarName = property.Name.ToUpper(); // Convert the property name to uppercase
            var envValue = Environment.GetEnvironmentVariable(envVarName); // Try to get the value from environment variables

            if (envValue != null) // If the env variable is found, map it
            {
                // Handle conversion for int and string types
                if (property.PropertyType == typeof(int) && int.TryParse(envValue, out var intValue))
                {
                    property.SetValue(settings, intValue);
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(settings, envValue);
                }
                // You can add more type conversions here as needed
            }
            // If envValue is null, the default value in the class will be used automatically
        }

        return settings; // Return the populated settings instance
    }
}