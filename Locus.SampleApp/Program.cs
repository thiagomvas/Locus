using Locus;

var provider = new LocalizationProvider(new LocaleConfiguration());

Console.WriteLine(provider["Greetings"]["Hello"]);  
Console.WriteLine(provider["Messages"]["Error"]);   

Console.WriteLine(provider["Greetings"]["Hello"]);  
Console.WriteLine(provider["Messages"]["Success"]); 

provider.SetLocale("pt-PT");

Console.WriteLine(provider["Greetings"]["Hello"]);  
Console.WriteLine(provider["Messages"]["Error"]);   

provider.SetLocale("en");

provider.SetLocale("es");

Console.WriteLine(provider["Greetings"]["Hello"]);  
Console.WriteLine(provider["Messages"]["Error"]);   

var non = provider["Messages"]["NonExistant"];

Console.WriteLine(provider["Messages"]["NonExistant"]); 