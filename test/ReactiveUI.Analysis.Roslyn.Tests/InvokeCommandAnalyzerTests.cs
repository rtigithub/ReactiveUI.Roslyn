using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;
using VerifyCS =
    ReactiveUI.Analysis.Roslyn.Tests.Verifiers.AnalyzerVerifier<ReactiveUI.Analysis.Roslyn.InvokeCommandAnalyzer>;

namespace ReactiveUI.Analysis.Roslyn.Tests
{
    public class InvokeCommandAnalyzerTests : CSharpAnalyzerTest<InvokeCommandAnalyzer, XUnitVerifier>
    {
        [Theory]
        [InlineData(InvokeCommandTestData.Incorrect)]
        public async Task Given_When_Then(string code)
        {
            // Given
            var diagnosticResult =
                VerifyCS.Diagnostic(InvokeCommandAnalyzer.Rule.Id)
                    .WithSeverity(DiagnosticSeverity.Warning)
                    .WithSpan(13, 32, 13, 39)
                    .WithMessage("Use expression lambda overload for property Command");

            // When, Then
            await VerifyCS.VerifyAnalyzerAsync(code, diagnosticResult);
        }
    }
}