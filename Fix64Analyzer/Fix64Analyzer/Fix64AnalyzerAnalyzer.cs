using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Fix64Analyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class Fix64AnalyzerAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "F64_NUM01";

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
		private static readonly string Title = "Fix64 Analyzer";
		private static readonly string MessageFormat = "Numeric literal error: {0}";
		private static readonly string Description = "Replace numeric literals with Fix64 static constants.";
		private const string Category = "Types";

		private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context)
		{
			//context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.)
			context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.NumericLiteralExpression);
		}

		private void AnalyzeNode(SyntaxNodeAnalysisContext context)
		{
			LiteralExpressionSyntax literal = (LiteralExpressionSyntax)context.Node;

			ISymbol symbol = context.SemanticModel.GetSymbolInfo(literal).Symbol;

			if (symbol != null && symbol.ContainingType.ToString() == "FixMath.NET.Fix64")
			{
				var diagnostic = Diagnostic.Create(Rule, literal.GetLocation(), "Numeric Literal should be replaced with static constant");
				context.ReportDiagnostic(diagnostic);
			}

			CastExpressionSyntax castParent = literal.Parent as CastExpressionSyntax;
			if (castParent != null)
			{
				symbol = context.SemanticModel.GetSymbolInfo(castParent).Symbol;
				if (symbol != null && symbol.ContainingType.ToString() == "FixMath.NET.Fix64")
				{
					var diagnostic = Diagnostic.Create(Rule, literal.GetLocation(), "Numeric Literal should be replaced with static constant");
					context.ReportDiagnostic(diagnostic);
				}

			}

			BinaryExpressionSyntax expParent = literal.Parent as BinaryExpressionSyntax;
			if (expParent != null)
			{
				var otherExp = expParent.Left != literal ? expParent.Left : expParent.Right;
				symbol = context.SemanticModel.GetSymbolInfo(otherExp).Symbol;

				if (symbol != null && symbol.ContainingType.ToString() == "FixMath.NET.Fix64")
				{
					var diagnostic = Diagnostic.Create(Rule, literal.GetLocation(), "Numeric Literal should be replaced with static constant");
					context.ReportDiagnostic(diagnostic);
				}
			}
			//throw new NotImplementedException();			
		}
	}
}
