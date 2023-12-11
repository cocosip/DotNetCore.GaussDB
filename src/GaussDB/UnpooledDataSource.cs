using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using GaussDB.Internal;
using GaussDB.Util;

namespace GaussDB;

sealed class UnpooledDataSource : GaussDBDataSource
{
    public UnpooledDataSource(GaussDBConnectionStringBuilder settings, GaussDBDataSourceConfiguration dataSourceConfig)
        : base(settings, dataSourceConfig)
    {
    }

    volatile int _numConnectors;

    internal override (int Total, int Idle, int Busy) Statistics => (_numConnectors, 0, _numConnectors);

    internal override bool OwnsConnectors => true;

    internal override async ValueTask<GaussDBConnector> Get(
        GaussDBConnection conn, GaussDBTimeout timeout, bool async, CancellationToken cancellationToken)
    {
        CheckDisposed();

        var connector = new GaussDBConnector(this, conn);
        await connector.Open(timeout, async, cancellationToken).ConfigureAwait(false);
        Interlocked.Increment(ref _numConnectors);
        return connector;
    }

    internal override bool TryGetIdleConnector([NotNullWhen(true)] out GaussDBConnector? connector)
    {
        connector = null;
        return false;
    }

    internal override ValueTask<GaussDBConnector?> OpenNewConnector(
        GaussDBConnection conn, GaussDBTimeout timeout, bool async, CancellationToken cancellationToken)
        => new((GaussDBConnector?)null);

    internal override void Return(GaussDBConnector connector)
    {
        Interlocked.Decrement(ref _numConnectors);
        connector.Close();
    }

    internal override void Clear() {}
}
