using CSharp9Overview.Interfaces;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

#region .NET 5 overview & Top level programs

WriteLine("Top level programs actually work.");
WriteLine($"You can even have args: {(args.Length > 0 ? args[0] : string.Empty)}!");
WriteLine($"Currently running on: {Environment.OSVersion}");
await ShowMessageWithDelay(3000, "You can use await with top level programs!");

if (OperatingSystem.IsWindows())
{
    using RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost");
    if (key?.GetValue("Version") is string sharedVersion)
    {
        WriteLine($"SharedHost version: {sharedVersion}");
    }
    else
    {
        WriteLine("Oops. Can't find that key, sorry.");
    }

}
else if (OperatingSystem.IsLinux())
{
    WriteLine("Running on LInux registry is not available!");
}

#endregion

#region Records

var bob = new Developer("Bob", "Salivan", 80_000);
Print(bob);

var jane = new Manager("Jane", "Morgan") 
{
    BaseSalary = 100_000,
    BonusPercentage = 0.25f
};
Print(jane);

var greedyJane = jane;
greedyJane.BonusPercentage = 0.5f;
Print(jane);
Print(greedyJane);
WriteLine($"Is Jane == to GreedyJane: { jane == greedyJane}");

var bobWithRaise = bob with { Salary = 120_000 };
Print(bobWithRaise);
WriteLine($"Is Bob == to BobWithRaise: { bob == bobWithRaise}");

var bobClone = new Developer("Bob", "Salivan", 80_000);
Print(bobClone);
WriteLine($"Is Bob equal to BobClone: { bob == bobClone}");

var bobWithSkills = bob with
{
    Skills = new[] { "C# 8", "VB 6" }
};

Print(bobWithSkills);
bobWithSkills.Skills[0] = "C# 9";
Print(bobWithSkills);

var bobWithNewSkills = bobWithSkills with
{
    Skills = bobWithSkills.Skills.Union(new[] { "F#" }).ToArray()
};
WriteLine($"Is BobWithSkills equal to BobWithNewSkills: { bobWithSkills == bobWithNewSkills }");

var (first, last, salary) = bobWithSkills;
WriteLine($"You can deconstruct the record: {first} {last}, {salary}");

#endregion

#region Target-typed new expressions 

Manager sam = new("Sam", "Goodwill")
{
    BonusPercentage = 0.1f,
    BaseSalary = 110_000,
};

Developer[] developers = { new("dev", "1", 10), new("dev", "2", 10), new("dev", "3", 10) };

#endregion

#region Source generators

HelloWorldGenerated.HelloWorld.SayHello();
DI.DIService.GetService<ICar>().CheckAllSystems();

#endregion

WriteLine("Press any key to exit.");
ReadKey();

return 15478; // You can return any code from top level programs

#region Definitions

async Task ShowMessageWithDelay(int delay, string message)
{
    await Task.Delay(delay);
    WriteLine(message);
}


void Print(Employee employee)
{
    WriteLine($"Employee: {employee}");
    if (employee is Developer developer)
    {
        Write("Skills:");
        WriteLine(string.Join(", ", developer.Skills));
    }
    else if (employee is Manager manager) 
    {
        Write("Manager says: ");
        manager.Manage();
    }
}

abstract record Employee(string FirstName, string LastName);

record Developer(string FirstName, string LastName, int Salary) : Employee(FirstName, LastName)
{
    public string[] Skills { get; init; } = Array.Empty<string>();
}

record Manager(string FirstName, string LastName) : Employee(FirstName, LastName)
{
    public int BaseSalary { get; init; }

    public float BonusPercentage { get; set; }

    public void Manage() => WriteLine("We need this done by Friday!");

    public override string ToString()
    {
        return $"[Manager]: {FirstName} {LastName}. Salary ${BaseSalary + (BaseSalary * BonusPercentage)}";
    }
}

#endregion