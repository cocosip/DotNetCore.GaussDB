using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using GaussDB.Internal;

namespace GaussDB;

/// <inheritdoc />
public class GaussDBBatch : DbBatch
{
    internal const int DefaultBatchCommandsSize = 5;

    private protected GaussDBCommand Command { get; }

    /// <inheritdoc />
    protected override DbBatchCommandCollection DbBatchCommands => BatchCommands;

    /// <inheritdoc cref="DbBatch.BatchCommands"/>
    public new GaussDBBatchCommandCollection BatchCommands { get; }

    /// <inheritdoc />
    public override int Timeout
    {
        get => Command.CommandTimeout;
        set => Command.CommandTimeout = value;
    }

    /// <inheritdoc cref="DbBatch.Connection"/>
    public new GaussDBConnection? Connection
    {
        get => Command.Connection;
        set => Command.Connection = value;
    }

    /// <inheritdoc />
    protected override DbConnection? DbConnection
    {
        get => Connection;
        set => Connection = (GaussDBConnection?)value;
    }

    /// <inheritdoc cref="DbBatch.Transaction"/>
    public new GaussDBTransaction? Transaction
    {
        get => Command.Transaction;
        set => Command.Transaction = value;
    }

    /// <inheritdoc />
    protected override DbTransaction? DbTransaction
    {
        get => Transaction;
        set => Transaction = (GaussDBTransaction?)value;
    }

    /// <summary>
    /// Controls whether to place error barriers between all batch commands within this batch. Default to <see langword="false" />.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     By default, any exception in a command causes later commands in the batch to be skipped, and earlier commands to be rolled back.
    ///     Enabling error barriers ensures that errors do not affect other commands in the batch.
    /// </para>
    /// <para>
    ///     Note that if the batch is executed within an explicit transaction, the first error places the transaction in a failed state,
    ///     causing all later commands to fail in any case. As a result, this option is useful mainly when there is no explicit transaction.
    /// </para>
    /// <para>
    ///     At the PostgreSQL wire protocol level, this corresponds to inserting a Sync message between each command, rather than grouping
    ///     all the batch's commands behind a single terminating Sync.
    /// </para>
    /// <para>
    ///     To control error barriers on a command-by-command basis, see <see cref="GaussDBBatchCommand.AppendErrorBarrier" />.
    /// </para>
    /// </remarks>
    public bool EnableErrorBarriers
    {
        get => Command.EnableErrorBarriers;
        set => Command.EnableErrorBarriers = value;
    }

    /// <summary>
    /// Marks all of the batch's result columns as either known or unknown.
    /// Unknown results column are requested them from PostgreSQL in text format, and GaussDB makes no
    /// attempt to parse them. They will be accessible as strings only.
    /// </summary>
    internal bool AllResultTypesAreUnknown
    {
        get => Command.AllResultTypesAreUnknown;
        set => Command.AllResultTypesAreUnknown = value;
    }

    /// <summary>
    /// Initializes a new <see cref="GaussDBBatch"/>.
    /// </summary>
    /// <param name="connection">A <see cref="GaussDBConnection"/> that represents the connection to a PostgreSQL server.</param>
    /// <param name="transaction">The <see cref="GaussDBTransaction"/> in which the <see cref="GaussDBCommand"/> executes.</param>
    public GaussDBBatch(GaussDBConnection? connection = null, GaussDBTransaction? transaction = null)
    {
        GC.SuppressFinalize(this);
        Command = new(DefaultBatchCommandsSize);
        BatchCommands = new GaussDBBatchCommandCollection(Command.InternalBatchCommands);

        Connection = connection;
        Transaction = transaction;
    }

    internal GaussDBBatch(GaussDBConnector connector)
    {
        GC.SuppressFinalize(this);
        Command = new(connector, DefaultBatchCommandsSize);
        BatchCommands = new GaussDBBatchCommandCollection(Command.InternalBatchCommands);
    }

    private protected GaussDBBatch(GaussDBDataSourceCommand command)
    {
        GC.SuppressFinalize(this);
        Command = command;
        BatchCommands = new GaussDBBatchCommandCollection(Command.InternalBatchCommands);
    }

    /// <inheritdoc />
    protected override DbBatchCommand CreateDbBatchCommand() => CreateBatchCommand();

    /// <inheritdoc cref="DbBatch.CreateBatchCommand"/>
    public new GaussDBBatchCommand CreateBatchCommand()
        => new GaussDBBatchCommand();

    /// <inheritdoc />
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        => ExecuteReader(behavior);

    /// <inheritdoc cref="DbBatch.ExecuteReader"/>
    public new GaussDBDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
        => Command.ExecuteReader(behavior);

    /// <inheritdoc />
    protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(
        CommandBehavior behavior,
        CancellationToken cancellationToken)
        => await ExecuteReaderAsync(behavior, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc cref="DbBatch.ExecuteReaderAsync(CancellationToken)"/>
    public new Task<GaussDBDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        => Command.ExecuteReaderAsync(cancellationToken);

    /// <inheritdoc cref="DbBatch.ExecuteReaderAsync(CommandBehavior,CancellationToken)"/>
    public new Task<GaussDBDataReader> ExecuteReaderAsync(
        CommandBehavior behavior,
        CancellationToken cancellationToken = default)
        => Command.ExecuteReaderAsync(behavior, cancellationToken);

    /// <inheritdoc />
    public override int ExecuteNonQuery()
        => Command.ExecuteNonQuery();

    /// <inheritdoc />
    public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default)
        => Command.ExecuteNonQueryAsync(cancellationToken);

    /// <inheritdoc />
    public override object? ExecuteScalar()
        => Command.ExecuteScalar();

    /// <inheritdoc />
    public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken = default)
        => Command.ExecuteScalarAsync(cancellationToken);

    /// <inheritdoc />
    public override void Prepare()
        => Command.Prepare();

    /// <inheritdoc />
    public override Task PrepareAsync(CancellationToken cancellationToken = default)
        => Command.PrepareAsync(cancellationToken);

    /// <inheritdoc />
    public override void Cancel() => Command.Cancel();

    /// <inheritdoc />
    public override void Dispose()
    {
        Command.ResetTransaction();
        if (Command.IsCacheable && Connection is not null && Connection.CachedBatch is null)
        {
            BatchCommands.Clear();
            Command.Reset();
            Connection.CachedBatch = this;
            return;
        }

        Command.IsCacheable = false;
    }

    internal static GaussDBBatch CreateCachedBatch(GaussDBConnection connection)
    {
        var batch = new GaussDBBatch(connection);
        batch.Command.IsCacheable = true;
        return batch;
    }
}
