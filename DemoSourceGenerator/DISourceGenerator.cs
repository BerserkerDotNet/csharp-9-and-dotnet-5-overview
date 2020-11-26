using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoSourceGenerator
{
    [Generator]
    public class DISourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            var template = @"
namespace DI
{ 
    public static class DIService
    {
        public static T GetService<T>()
        {
            return default;
        }
    }
}
";
            var options = (compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(template, Encoding.UTF8), options));
            var diags = compilation.GetDiagnostics();

            var services = new List<Service>();
            var serviceLocatorClass = compilation.GetTypeByMetadataName("DI.DIService")!;
            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                var toInject = from i in tree.GetRoot().DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>()
                                                               let symbol = semanticModel.GetSymbolInfo(i).Symbol as IMethodSymbol
                                                               where symbol != null
                                                               where SymbolEqualityComparer.Default.Equals(symbol.ContainingType, serviceLocatorClass)
                                                               select symbol.ReturnType as INamedTypeSymbol;
                foreach (var typeToInject in toInject)
                {
                    var service = ConstructServiceTree(context, compilation, typeToInject);
                    if (service is object)
                    {
                        services.Add(service);
                    }
                }
            }

            var sourceCode = new StringBuilder();
            sourceCode.AppendLine(@"
namespace DI
{ 
    public static class DIService
    {");
            var fields = new List<Service>();
            CreateSingletons(sourceCode, services, fields);

            sourceCode.AppendLine(@"
        public static T GetService<T>()
        {");

            foreach (var service in services)
            {
                sourceCode.AppendLine("if (typeof(T) == typeof(" + service.Type + "))");
                sourceCode.AppendLine("{");
                sourceCode.AppendLine($"    return (T)(object){ConstructType(service, fields)};");
                sourceCode.AppendLine("}");
            }

            sourceCode.AppendLine("throw new System.InvalidOperationException(\"Don't know how to initialize type: \" + typeof(T).Name);");
            sourceCode.AppendLine(@"
        }
    }
}");

            context.AddSource("DIService", SourceText.From(sourceCode.ToString(), Encoding.UTF8));
        }

        private Service ConstructServiceTree(GeneratorExecutionContext context, Compilation compilation, INamedTypeSymbol typeToInject)
        {
            var implementation = typeToInject.IsAbstract ? FindImplementation(compilation, typeToInject) : typeToInject;

            if (implementation == null)
            {
                context.ReportDiagnostic(Diagnostic.Create("DI001", "DI", $"No implemnentation found for {typeToInject}", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, false));
                return null;
            }
            else
            {
                var dependencies = new List<Service>();
                var ctor = implementation.Constructors.FirstOrDefault();
                if (ctor is object)
                {
                    foreach (var parameter in ctor.Parameters)
                    {
                        if (parameter.Type is INamedTypeSymbol parameterType)
                        {
                            var dep = ConstructServiceTree(context, compilation, parameterType);
                            if (dep is object)
                            {
                                dependencies.Add(dep);
                            }
                        }
                    }
                }

                var service = new Service
                {
                    Type = typeToInject,
                    Implementation = implementation,
                    Dependencies = dependencies
                };

                return service;
            }
        }

        private INamedTypeSymbol FindImplementation(Compilation compilation, INamedTypeSymbol type)
        {
            foreach (var ns in compilation.GlobalNamespace.GetNamespaceMembers())
            {
                if (ns.Name == "System" || ns.Name == "Microsoft") continue;

                var count = 0;
                foreach (var x in GetAllTypes(new[] { ns }))
                {
                    count++;
                    if (!x.IsAbstract && x.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, type)))
                    {
                        return x;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(IEnumerable<INamespaceSymbol> namespaces)
        {
            foreach (var ns in namespaces)
            {
                foreach (var t in ns.GetTypeMembers())
                {
                    yield return t;
                }

                foreach (var subType in GetAllTypes(ns.GetNamespaceMembers()))
                {
                    yield return subType;
                }
            }
        }

        private static string ConstructType(Service service, List<Service> fields)
        {
            var sb = new StringBuilder();

            var field = fields.FirstOrDefault(f => SymbolEqualityComparer.Default.Equals(f.Implementation, service.Implementation));
            if (field != null)
            {
                sb.Append(field.SingletonName);
            }
            else
            {
                sb.Append("new ");
                sb.Append(service.Implementation);
                sb.Append('(');
   
                var first = true;
                foreach (var dependency in service.Dependencies)
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }
                    sb.Append(ConstructType(dependency, fields));
                    first = false;
                }
                sb.Append(')');
            }
            return sb.ToString();
        }

        private static void CreateSingletons(StringBuilder sourceBuilder, List<Service> services, List<Service> fields)
        {
            foreach (var service in services)
            {
                CreateSingletons(sourceBuilder, service.Dependencies, fields);

                service.SingletonName = $"_{service.Implementation.Name.ToLower()}";
                sourceBuilder.AppendLine($"private static {service.Type} {service.SingletonName} = {ConstructType(service, fields)};");
                fields.Add(service);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }

    public class Service
    {
        public INamedTypeSymbol Type { get; set; }

        public List<Service> Dependencies { get; set; } = new List<Service>();

        public INamedTypeSymbol Implementation { get; set; }

        public string SingletonName { get; set; }
    }
}
