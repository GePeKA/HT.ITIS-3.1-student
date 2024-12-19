using System.Linq.Expressions;
using System.Reflection;

namespace Dotnet.Homeworks.Shared.Dto;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    public Result(bool isSuccessful, string? error = default)
    {
        IsSuccess = isSuccessful;
        if (error is not null) 
            Error = error;
    }

    public static dynamic Create(bool isSuccessful, Type resultType, string? error = default, dynamic? value = default)
    {
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = resultType.GetGenericArguments()[0];

            ConstructorInfo constructor = resultType.GetConstructor(
                new Type[] { valueType, typeof(bool), typeof(string) }
            )!;

            var successParam = Expression.Constant(isSuccessful, typeof(bool));
            var valueParam = Expression.Constant(value, valueType);
            var errorParam = Expression.Constant(error, typeof(string));

            var exprNew = Expression.New(constructor, valueParam, successParam, errorParam);

            var result = Expression.Lambda<Func<dynamic>>(exprNew).Compile()();

            return result;
        }
        else
        {
            return resultType.IsAssignableTo(typeof(Result))
                ? new Result(isSuccessful, error)
                : throw new Exception("Not assignable to Result");
        }
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? val, bool isSuccessful, string? error = default)
        : base(isSuccessful, error)
    {
        _value = val;
    }

    public TValue? Value => IsSuccess
        ? _value
        : throw new Exception(Error);
}