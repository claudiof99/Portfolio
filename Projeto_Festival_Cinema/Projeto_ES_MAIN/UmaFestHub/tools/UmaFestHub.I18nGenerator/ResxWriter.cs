using System.Text;
using System.Text.Json;

namespace UmaFestHub.I18nGenerator;

internal static class ResxWriter
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = false
	};

	public static Dictionary<string, string> LoadLocale(string jsonPath)
	{
		var json = File.ReadAllText(jsonPath, Encoding.UTF8);
		var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json, JsonOptions)
		           ?? throw new InvalidOperationException($"Could not parse locale file: {jsonPath}");

		if (data.Count == 0)
		{
			throw new InvalidOperationException($"Locale file is empty: {jsonPath}");
		}

		return data;
	}

	public static void ValidateLocales(IReadOnlyDictionary<string, Dictionary<string, string>> locales)
	{
		if (!locales.TryGetValue("en", out var en))
		{
			throw new InvalidOperationException("English (en) locale is required.");
		}

		var enKeys = en.Keys.ToHashSet(StringComparer.Ordinal);
		foreach (var (locale, strings) in locales)
		{
			if (locale == "en")
			{
				continue;
			}

			var localeKeys = strings.Keys.ToHashSet(StringComparer.Ordinal);
			var missing = enKeys.Except(localeKeys).Order(StringComparer.Ordinal).ToList();
			var extra = localeKeys.Except(enKeys).Order(StringComparer.Ordinal).ToList();

			if (missing.Count > 0)
			{
				throw new InvalidOperationException($"{locale} missing keys: {string.Join(", ", missing)}");
			}

			if (extra.Count > 0)
			{
				throw new InvalidOperationException($"{locale} has extra keys: {string.Join(", ", extra)}");
			}
		}
	}

	public static void WriteResx(string outputPath, IReadOnlyDictionary<string, string> strings)
	{
		var directory = Path.GetDirectoryName(outputPath);
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}

		var lines = new List<string>
		{
			"<?xml version=\"1.0\" encoding=\"utf-8\"?>",
			"<root>"
		};

		foreach (var key in strings.Keys.Order(StringComparer.Ordinal))
		{
			var value = EscapeXml(strings[key]);
			lines.Add($"  <data name=\"{key}\" xml:space=\"preserve\"><value>{value}</value></data>");
		}

		lines.Add("</root>");
		var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
		File.WriteAllText(outputPath, string.Join(Environment.NewLine, lines) + Environment.NewLine, utf8NoBom);
		Console.WriteLine($"Wrote {outputPath} ({strings.Count} keys)");
	}

	private static string EscapeXml(string value) =>
		value
			.Replace("&", "&amp;", StringComparison.Ordinal)
			.Replace("<", "&lt;", StringComparison.Ordinal)
			.Replace(">", "&gt;", StringComparison.Ordinal)
			.Replace("\"", "&quot;", StringComparison.Ordinal);
}
