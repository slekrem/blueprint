namespace com.smerkel.blueprint
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;
    using RazorEngine;
    using RazorEngine.Templating;

    public class Program
    {
        public static void Main(string[] args)
        {
            // arrange
            var blueprint = new Blueprint<IDataModel>();
            
            // assert
            if(blueprint.Object == null)
                throw new Exception("blueprint.Object is null");
            
            // assert
            if(blueprint.Object.GetType() != typeof(IDataModel))
                throw new Exception("");
                
            Console.WriteLine("Ende ...");
        }

        public class Blueprint<T> : IBlueprint<T>
        {
            private T _object;
            
            private string _sourceCode;
            private const string CLASS_TEMPLATE = 
            @"
            namespace @Model.NamespaceName
            {
                public class @Model.ClassName 
                {
                    
                }
            }
            ";
            
            public Blueprint()
            {
                _sourceCode = Engine.Razor.RunCompile(CLASS_TEMPLATE, "templateKey", null, new 
                {
                    NamespaceName = "com.smerkel.asd",
                    ClassName = "ASD"
                 });
                 
                 Console.WriteLine("sourceCode:");
                 Console.WriteLine(_sourceCode);
                 
                 Program2.Main(_sourceCode);
            }
            
            public T Object { get; private set; }
        }

        public interface IBlueprint<T>
        {
            T Object { get; }
        }
        
        public interface IDataModel
        {
            string Data { get; }
        }
    }
    
    class Program2
    {
        private static readonly System.Collections.Generic.IEnumerable<string> DefaultNamespaces =
            new[]
            {
                "System", 
                "System.IO", 
                "System.Net", 
                "System.Linq", 
                "System.Text", 
                "System.Text.RegularExpressions", 
                "System.Collections.Generic"
            };

        private static string runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1\{0}.dll";

        private static readonly System.Collections.Generic.IEnumerable<MetadataReference> DefaultReferences =
            new[]
            {
                MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core"))
            };

        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release)
                    .WithUsings(DefaultNamespaces);

        private static SyntaxTree Parse(string text, string filename = "", CSharpParseOptions options = null)
        {
            var stringText = SourceText.From(text);
            return SyntaxFactory.ParseSyntaxTree(stringText, options, filename);
        }

        public static void Main(string sourceCode)
        {
            var parsedSyntaxTree = Parse(sourceCode, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp5));

            var compilation
                = CSharpCompilation.Create("Test.dll", new SyntaxTree[] { parsedSyntaxTree }, DefaultReferences, DefaultCompilationOptions);
            try
            {
                var result = compilation.Emit(@".\Test.dll");

                Console.WriteLine(result.Success ? "Sucess!!" : "Failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Read();
        }
    }
}
