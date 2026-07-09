using UmaFestHub.I18nGenerator;

var command = args.Length > 0 ? args[0].ToLowerInvariant() : "build";

try
{
	var exitCode = command switch
	{
		"build" => BuildResx(),
		"fix-comments" => FixMissingKeyComments(),
		_ => PrintUsage()
	};

	Environment.Exit(exitCode);
}
catch (Exception ex)
{
	Console.Error.WriteLine(ex.Message);
	Environment.Exit(1);
}

static int BuildResx()
{
	var repoRoot = RepoPaths.FindRoot();
	var dataDir = Path.Combine(repoRoot, "tools", "i18n", "data");
	var resourcesDir = Path.Combine(repoRoot, "src", "UmaFestHub.Web", "Resources");

	var locales = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
	foreach (var locale in new[] { "en", "pt", "fr" })
	{
		var path = Path.Combine(dataDir, $"{locale}.json");
		if (!File.Exists(path))
		{
			throw new FileNotFoundException($"Locale file not found: {path}");
		}

		locales[locale] = ResxWriter.LoadLocale(path);
	}

	ResxWriter.ValidateLocales(locales);

	var keyCount = locales["en"].Count;
	ResxWriter.WriteResx(Path.Combine(resourcesDir, "SharedResources.resx"), locales["en"]);
	ResxWriter.WriteResx(Path.Combine(resourcesDir, "SharedResources.pt.resx"), locales["pt"]);
	ResxWriter.WriteResx(Path.Combine(resourcesDir, "SharedResources.fr.resx"), locales["fr"]);

	Console.WriteLine($"Done. Generated 3 files with {keyCount} keys each.");
	return 0;
}

static int FixMissingKeyComments()
{
	var repoRoot = RepoPaths.FindRoot();
	var viewsDir = Path.Combine(repoRoot, "src", "UmaFestHub.Web", "Views");
	MissingKeyCommentFixer.FixViews(viewsDir);
	return 0;
}

static int PrintUsage()
{
	Console.WriteLine("UmaFestHub i18n generator");
	Console.WriteLine();
	Console.WriteLine("Usage:");
	Console.WriteLine("  dotnet run --project tools/UmaFestHub.I18nGenerator -- build");
	Console.WriteLine("  dotnet run --project tools/UmaFestHub.I18nGenerator -- fix-comments");
	return 1;
}

internal static class RepoPaths
{
	public static string FindRoot()
	{
		var cwd = Directory.GetCurrentDirectory();
		if (File.Exists(Path.Combine(cwd, "UmaFestHub.sln")))
		{
			return cwd;
		}

		var dir = new DirectoryInfo(AppContext.BaseDirectory);
		while (dir is not null)
		{
			if (File.Exists(Path.Combine(dir.FullName, "UmaFestHub.sln")))
			{
				return dir.FullName;
			}

			dir = dir.Parent;
		}

		throw new InvalidOperationException("Could not locate repository root (UmaFestHub.sln).");
	}
}
