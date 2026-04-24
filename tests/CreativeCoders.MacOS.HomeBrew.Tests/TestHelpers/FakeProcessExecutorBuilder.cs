using CreativeCoders.ProcessUtils.Execution;
using FakeItEasy;

namespace CreativeCoders.MacOS.HomeBrew.Tests.TestHelpers;

/// <summary>
/// Test helpers that produce fluent-builder fakes for <see cref="IProcessExecutorBuilder{T}"/>.
/// Configures the builder so that every fluent setter returns the builder itself and
/// <see cref="IProcessExecutorBuilder{T}.Build"/> yields the provided executor fake.
/// </summary>
internal static class FakeProcessExecutorBuilder
{
    public static IProcessExecutorBuilder<T> Create<T>(out IProcessExecutor<T> executor)
    {
        var builder = A.Fake<IProcessExecutorBuilder<T>>();
        executor = A.Fake<IProcessExecutor<T>>();

        A.CallTo(() => builder.SetFileName(A<string>._)).Returns(builder);
        A.CallTo(() => builder.SetArguments(A<string[]>._)).Returns(builder);
        A.CallTo(() => builder.SetupStartInfo(A<Action<System.Diagnostics.ProcessStartInfo>>._)).Returns(builder);
        A.CallTo(() => builder.ShouldThrowOnError(A<bool>._)).Returns(builder);
        A.CallTo(() => builder.SetOutputParser(A<IProcessOutputParser<T>>._)).Returns(builder);
        // Fluent generic SetOutputParser<TParser>(Action<TParser>) is matched via reflection-based predicate
        A.CallTo(builder)
            .Where(call => call.Method.Name == "SetOutputParser" && call.Method.IsGenericMethod)
            .WithReturnType<IProcessExecutorBuilder<T>>()
            .Returns(builder);

        A.CallTo(() => builder.Build()).Returns(executor);

        return builder;
    }
}
