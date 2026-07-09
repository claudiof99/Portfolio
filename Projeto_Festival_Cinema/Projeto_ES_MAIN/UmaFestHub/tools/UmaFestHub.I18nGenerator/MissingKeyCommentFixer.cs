using System.Text.RegularExpressions;

namespace UmaFestHub.I18nGenerator;

internal static partial class MissingKeyCommentFixer
{
	[GeneratedRegex(@"@\* missing key: (\w+) \*@[^""<]*(?=""|<|</|\s*$|\))", RegexOptions.Multiline)]
	private static partial Regex MissingKeyBlockPattern();

	[GeneratedRegex(@"Localizer\[""(\w+)""\]\s*/\* missing key: (\w+) \*/")]
	private static partial Regex MissingKeyFallbackPattern();

	public static int FixViews(string viewsDirectory)
	{
		if (!Directory.Exists(viewsDirectory))
		{
			throw new DirectoryNotFoundException($"Views directory not found: {viewsDirectory}");
		}

		var updatedFiles = 0;
		foreach (var path in Directory.EnumerateFiles(viewsDirectory, "*.cshtml", SearchOption.AllDirectories))
		{
			var original = File.ReadAllText(path);
			var updated = MissingKeyBlockPattern().Replace(original, m => $"@Localizer[\"{m.Groups[1].Value}\"]");
			updated = MissingKeyFallbackPattern().Replace(updated, m => $"Localizer[\"{m.Groups[2].Value}\"]");

			if (!string.Equals(original, updated, StringComparison.Ordinal))
			{
				File.WriteAllText(path, updated);
				updatedFiles++;
			}
		}

		Console.WriteLine($"Updated {updatedFiles} files");
		return updatedFiles;
	}
}
