using System.Reflection;

namespace OAuthCore.Infrastructure.Configuration;

public static class EnvSettingsMapping
{
    public static T MapTo<T>() where T : new()
    {
        var settings = new T();
        var settingsType = typeof(T);

        foreach (var property in settingsType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var envVarName = ConvertToEnvVarName(property.Name);
            var envValue = Environment.GetEnvironmentVariable(envVarName);

            if (envValue == null)
            {
                if (property.GetValue(settings) == null ||
                    (property.PropertyType == typeof(string) && string.IsNullOrEmpty(property.GetValue(settings) as string)))
                {
                    throw new InvalidOperationException($"Required environment variable {envVarName} is not set and no default value is provided.");
                }
                continue; // Skip if environment variable is not set but a default value exists
            }

            try
            {
                if (property.PropertyType == typeof(int) && int.TryParse(envValue, out var intValue))
                {
                    property.SetValue(settings, intValue);
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(settings, envValue);
                }
                else if (property.PropertyType == typeof(bool))
                {
                    if (bool.TryParse(envValue, out var boolValue))
                    {
                        property.SetValue(settings, boolValue);
                    }
                    else
                    {
                        // Handle cases where the env var might be "1" or "0" instead of "true" or "false"
                        property.SetValue(settings, envValue.ToLower() == "1" || envValue.ToLower() == "true");
                    }
                }
                // Add more type conversions as needed
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error setting property {property.Name} from environment variable {envVarName}", ex);
            }
        }

        return settings;
    }

    private static string ConvertToEnvVarName(string propertyName)
    {
        // Convert camelCase or PascalCase to uppercase with underscores
        return string.Concat(propertyName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToUpper();
    }
}