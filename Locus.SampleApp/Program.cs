using Locus;

var provider = new LocalizationProvider(new LocaleConfiguration());

Console.WriteLine(provider["Greetings"]["Hello"].Format(new { name = "John" }));  
Console.WriteLine(provider["Messages"]["Error"]);   

provider.SetLocale("es");

Console.WriteLine(provider["Greetings"]["Hello"].Format(new { name = "Jane" }));  
Console.WriteLine(provider["Messages"]["Error"]);   

var non = provider["Messages"]["NonExistant"];

Console.WriteLine(provider["Messages"]["NonExistant"]); 