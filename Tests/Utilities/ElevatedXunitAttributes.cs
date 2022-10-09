// ReSharper disable VirtualMemberCallInConstructor

namespace Tests.Utilities;

public class ElevatedFactAttribute: FactAttribute {

    internal static readonly string ElevationRequiredMessage = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true" ?
        "Run \"dotnet test\" with the argument \"--settings Tests\\Tests.runsettings\", and ensure the absolute paths to Telerik.CodeWeaver.Profiler.dll are correct inside the file" :
        "Enable JustMock Profiler, then run test with either ReSharper's Cover mode or Visual Studio's Test Explorer (although that crashes on [ElevatedTheory])";

    public ElevatedFactAttribute() {
        if (Skip == null && !Mock.IsProfilerEnabled) {
            Skip = ElevationRequiredMessage;
        }
    }

}

/// <summary>
/// <para>Warning: xUnit will try to dispose of any data values that you pass to your theory, but that disposal won't be intercepted by JustMock, so xUnit may call real methods on what
/// you thought were mocks, leading to unexpected behavior or crashes.</para>
/// <para>To avoid this issue, manually dispose of any disposable parameter values passed into your theory before the test method returns, for example, with a <c>using</c> block around the body of your test
/// method.</para>
/// <para> </para>
/// <para>Warning: This attribute will cause Visual Studio's Test Explorer to fail with
/// <c>System.InvalidOperationException : The test method expected 1 parameter value, but 0 parameter values were provided.</c></para>
/// <para>If this prevents you from running or debugging tests, you can replace it with <see cref="TheoryAttribute"/>.</para>
/// </summary>
public class ElevatedTheoryAttribute: TheoryAttribute {

    public ElevatedTheoryAttribute() {
        if (Skip == null && !Mock.IsProfilerEnabled) {
            Skip = ElevatedFactAttribute.ElevationRequiredMessage;
        }
    }

}