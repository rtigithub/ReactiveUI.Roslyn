using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ReactiveUI.Analysis.Roslyn
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WhenAnyValueClosureCodeFixProvider)), Shared]
    public class WhenAnyValueClosureCodeFixProvider : UnsupportedExpressionCodeFixProvider
    {
        protected override void RegisterCodeFix(
            CodeFixContext context,
            InvocationExpressionSyntax invocation,
            SimpleLambdaExpressionSyntax declaration,
            Diagnostic diagnostic) => context.RegisterCodeFix(
            CodeAction.Create(
                title: "When Any Fixer Up",
                createChangedDocument: c => Fixup(context.Document, invocation, declaration, c),
                equivalenceKey: UnsupportedExpressionAnalyzer.Rule.Id + UnsupportedExpressionAnalyzer.Rule.Title + "WhenAnyValue"),
            diagnostic
        );

        protected override async Task<Document> Fix(Document document, InvocationExpressionSyntax invocation, SimpleLambdaExpressionSyntax declaration, CancellationToken cancellationToken)
        {
            var rootAsync = await document.GetSyntaxRootAsync(cancellationToken);
            if (rootAsync == null)
            {
                return document;
            }

            var arguments = ArgumentList(
                SingletonSeparatedList(
                    Argument(
                        SimpleLambdaExpression(
                                Parameter(
                                    Identifier(
                                        TriviaList(),
                                        declaration.Parameter.Identifier.Text,
                                        TriviaList(
                                            Space
                                        )
                                    )
                                )
                            )
                           .WithArrowToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.EqualsGreaterThanToken,
                                    TriviaList(
                                        Space
                                    )
                                )
                            )
                           .WithExpressionBody(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName(declaration.Parameter.Identifier),
                                    IdentifierName(declaration.Body.GetLastToken())
                                )
                            ))));

            var changed = rootAsync.ReplaceNode(invocation.ArgumentList, arguments);
            return document.WithSyntaxRoot(changed);
        }
    }
}