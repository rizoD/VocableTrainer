
using AnkiNet;
using DBExtractor;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
	config.ValidateExamples();

	config.AddCommand<ExtractCommand>("extract")
		.WithDescription("Extracts data and files from the provided dbFile.")
		.WithExample(new[] { "extract", "database.db3", "." });
});


return app.Run(args);