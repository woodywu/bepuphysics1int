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

		private string GetStaticDefintionFromValue<T>(Optional<T> value)
		{
			string v = value.ToString().ToLower();

			string result = v.Replace('f', 'm');
			if (v.StartsWith("-"))
				return "(" + result + ")";

			return result;
		}

		private async Task<Solution> FixLiteralAsync(Document document, LiteralExpressionSyntax literal, CancellationToken cancellationToken)
        {
			if (document.Name == "F64.cs")
				return null;
			//string 

			// Get the symbol representing the type to be renamed.
			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			ISymbol symbol = semanticModel.GetSymbolInfo(literal).Symbol;

			var value = semanticModel.GetConstantValue(literal);
			string staticName = GetStaticNameFromValue(value);

			if (staticName == null)
				return null;

			string staticDefinition = GetStaticDefintionFromValue(value);

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
			bool hasUsing = syntaxRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().Any(d => d.Name.ToFullString() == "BEPUutilities");

			if (!hasUsing)
			{
				var usingF64 = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("BEPUutilities"));
				documentEditor.InsertAfter(syntaxRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().Last(), usingF64);
			}

			var newSolution = documentEditor.GetChangedDocument().Project.Solution;

			var BEPUutilities = newSolution.Projects.Single(proj => proj.AssemblyName == "BEPUutilities");
			var F64 = BEPUutilities.Documents.Single(doc => doc.Name == "F64.cs");

			syntaxRoot = await F64.GetSyntaxRootAsync();
			bool constantDefined = syntaxRoot.DescendantNodes().OfType<FieldDeclarationSyntax>().Any(d => d.ToString().Contains(staticName));
			if (!constantDefined)
			{
				//documentEditor = await DocumentEditor.CreateAsync(F64);

				var valExp = SyntaxFactory.ParseExpression("(Fix64)" + staticDefinition);
				var definition = SyntaxFactory.FieldDeclaration(
					SyntaxFactory.VariableDeclaration(
						SyntaxFactory.ParseTypeName("Fix64"),
						SyntaxFactory.SeparatedList(new[] {
							SyntaxFactory.VariableDeclarator(
								SyntaxFactory.Identifier( staticName ),
								null,
								SyntaxFactory.EqualsValueClause( valExp )
							)
						})
					)).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)); ;

				//var definition = SyntaxFactory.ParseExpression("public static readonly Fix64 " + staticName + " = (Fix64)" + staticDefinition);
				ClassDeclarationSyntax cls = syntaxRoot.DescendantNodes().OfType<ClassDeclarationSyntax>().Single();

				var newCls = cls.WithMembers(cls.Members.Add(definition));
				//documentEditor.InsertMembers(cls, 0, new [] { definition });
				//documentEditor.InsertAfter(cls, definition);
				//var newDoc = documentEditor.GetChangedDocument();

				newSolution = F64.WithSyntaxRoot(syntaxRoot.ReplaceNode(cls, newCls)).Project.Solution;
			}

			//var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

			// Produce a new solution that has all references to that type renamed, including the declaration.
			//var originalSolution = document.Project.Solution;
			//var optionSet = originalSolution.Workspace.Options;
			//var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

			// Return the new solution with the now-uppercase type name.

			return newSolution;
        }
    }
}
