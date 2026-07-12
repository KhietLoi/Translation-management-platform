using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace MySolution.Migration;

public class CustomScriptProvider : IScriptProvider
{
    private readonly IScriptProvider  _innerProvider;
    private readonly Func<string, string> _rename;
    private readonly SqlScriptOptions _sqlScriptOptions;

    public CustomScriptProvider
    (
        IScriptProvider innerProvider,
        Func<string, string> rename,
        SqlScriptOptions sqlScriptOptions
    )
    {
        _innerProvider = innerProvider;
        _rename = rename;
        _sqlScriptOptions = sqlScriptOptions;
    }

    public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
    {
        var scripts = _innerProvider.GetScripts(connectionManager);

        foreach (var s in  scripts)
        {
            var newName = _rename(s.Name);
            yield return new SqlScript(newName, s.Contents, _sqlScriptOptions);
        }   
    }
}