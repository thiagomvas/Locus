using Cocona;
using Locus.Cli;

var app = CoconaApp.Create();

app.AddCommands<InitCommand>();
app.AddCommands<ValidateCommand>();


app.Run();