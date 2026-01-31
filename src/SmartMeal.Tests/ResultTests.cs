using FluentAssertions;
using SmartMeal.Application.Common;

namespace SmartMeal.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_Create_ShouldStoreValue()
    {
        var value = 42;

        var result = new Result<int>.Success(value);

        result.Should().BeOfType<Result<int>.Success>();
        result.Match(
            onSuccess: v => v.Should().Be(value),
            onFailure: _ => throw new InvalidOperationException("Expected success"));
    }

    [Fact]
    public void Success_Match_ShouldCallOnSuccess()
    {
        var value = "test";
        var result = new Result<string>.Success(value);

        var matchResult = result.Match(
            onSuccess: v => $"Success: {v}",
            onFailure: _ => "Failure");

        matchResult.Should().Be("Success: test");
    }

    [Fact]
    public void Failure_Create_ShouldStoreError()
    {
        var error = Error.Create("TestCode", "Test message");

        var result = new Result<int>.Failure(error);

        result.Should().BeOfType<Result<int>.Failure>();
        result.Match(
            onSuccess: _ => throw new InvalidOperationException("Expected failure"),
            onFailure: e => e.Should().Be(error));
    }

    [Fact]
    public void Failure_Match_ShouldCallOnFailure()
    {
        var error = Error.Create("ErrorCode", "Error message");
        var result = new Result<string>.Failure(error);

        var matchResult = result.Match(
            onSuccess: _ => "Success",
            onFailure: e => $"Failure: {e.Message}");

        matchResult.Should().Be("Failure: Error message");
    }

    [Fact]
    public void Match_WithReferenceType_ShouldWork()
    {
        var value = new List<int> { 1, 2, 3 };
        var result = new Result<List<int>>.Success(value);

        var matchResult = result.Match(
            onSuccess: v => v.Count,
            onFailure: _ => 0);

        matchResult.Should().Be(3);
    }

    [Fact]
    public void Match_WithNullableType_Success_ShouldWork()
    {
        var result = new Result<string?>.Success(null);

        var matchResult = result.Match(
            onSuccess: v => v ?? "null",
            onFailure: _ => "error");

        matchResult.Should().Be("null");
    }

    [Fact]
    public void Error_Create_ShouldStoreCodeAndMessage()
    {
        var code = "ERR001";
        var message = "Something went wrong";

        var error = Error.Create(code, message);

        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
    }

    [Fact]
    public void Result_Success_TypeCheck()
    {
        var result = new Result<int>.Success(100);

        var isSuccess = result is Result<int>.Success;
        var isFailure = result is Result<int>.Failure;

        isSuccess.Should().BeTrue();
        isFailure.Should().BeFalse();
    }

    [Fact]
    public void Result_Failure_TypeCheck()
    {
        var result = new Result<int>.Failure(Error.Create("Code", "Message"));

        var isSuccess = result is Result<int>.Success;
        var isFailure = result is Result<int>.Failure;

        isSuccess.Should().BeFalse();
        isFailure.Should().BeTrue();
    }
}
