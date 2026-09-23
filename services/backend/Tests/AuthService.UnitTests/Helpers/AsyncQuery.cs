using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace AuthService.UnitTests.Helpers;

// Provides async LINQ execution for handler unit tests; this does not test SQL translation.
internal static class AsyncQuery
{
    public static IQueryable<T> Create<T>(params T[] items) => new AsyncEnumerable<T>(items);

    private sealed class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(IEnumerable<T> items) : base(items) { }
        public AsyncEnumerable(Expression expression) : base(expression) { }

        IQueryProvider IQueryable.Provider => new AsyncProvider(this);

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator(), cancellationToken);
    }

    private sealed class AsyncProvider(IQueryProvider inner) : IAsyncQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = expression.Type.GetGenericArguments()[0];
            return (IQueryable)Activator.CreateInstance(
                typeof(AsyncEnumerable<>).MakeGenericType(elementType), expression)!;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new AsyncEnumerable<TElement>(expression);

        public object? Execute(Expression expression) => inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var result = inner.Execute(expression);
            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType).Invoke(null, [result])!;
        }
    }

    private sealed class AsyncEnumerator<T>(IEnumerator<T> inner, CancellationToken cancellationToken)
        : IAsyncEnumerator<T>
    {
        public T Current => inner.Current;
        public ValueTask<bool> MoveNextAsync()
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
