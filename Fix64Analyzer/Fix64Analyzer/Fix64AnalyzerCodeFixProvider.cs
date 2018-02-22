using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Editing;

namespace Fix64Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Fix64AnalyzerCodeFixProvider)), Shared]
    public class Fix64AnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string title = "Replace with static constant";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(Fix64AnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the literal identified by the diagnostic.
            var literal = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<LiteralExpressionSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: c => FixLiteralAsync(context.Document, literal, c), 
                    equivalenceKey: title),
                diagnostic);
        }

		private string GetStaticNameFromValue<T>(Optional<T> value)
		{
			if (!value.HasValue)
				return null;

			return "C"+value.ToString().ToLower().Replace('-', 'm').Replace('.', 'p').Replace("f", "");
		}

        private async Task<Solution> FixLiteralAsync(Document document, LiteralExpressionSyntax literal, CancellationToken cancellationToken)
        {
			//string 

			// Get the symbol representing the type to be renamed.
			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			ISymbol symbol = semanticModel.GetSymbolInfo(literal).Symbol;

			string staticName = GetStaticNameFromValue(semanticModel.GetConstantValue(literal));

			if (staticName == null)
				return null;

			var newLiteral = SyntaxFactory.ParseExpression("F64." + staticName)
				  .WithLeadingTrivia(literal.GetLeadingTrivia())
				  .WithTrailingTrivia(literal.GetTrailingTrivia())
				  .WithAdditionalAnnotations(Formatter.Annotation);

			Solution solution = document.Project.Solution;

			//			Project bepuUtils = solution.GetProject("BEPUutilities");
			//			Document F64 = bepuUtils.GetDocument("F64.cs");

			var documentEditor = await DocumentEditor.CreateAsync(document);
			documentEditor.ReplaceNode(literal, newLiteral);

			var syntaxRoot = await document.GetSyntaxRootAsync();
			bool hasUsing = syntaxRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().Any(d => d.Name.ToFullString() == "BEPUutilities.F64");

			if (!hasUsing)
			{
				var usingF64 = SyntaxFactory.ParseExpression("using BEPUutilities.F64;");
				documentEditor.InsertAfter(syntaxRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().Last(), usingF64);
			}
			
			
			//var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

			// Produce a new solution that has all references to that type renamed, including the declaration.
			//var originalSolution = document.Project.Solution;
			//var optionSet = originalSolution.Workspace.Options;
			//var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

			// Return the new solution with the now-uppercase type name.

			return documentEditor.GetChangedDocument().Project.Solution;
        }
    }
}
