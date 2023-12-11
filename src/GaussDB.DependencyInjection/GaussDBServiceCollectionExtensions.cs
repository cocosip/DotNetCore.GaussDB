using GaussDB;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Common;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension method for setting up GaussDB services in an <see cref="IServiceCollection" />.
/// </summary>
public static class GaussDBServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="GaussDBDataSource" /> and an <see cref="GaussDBConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An GaussDB connection string.</param>
    /// <param name="dataSourceBuilderAction">
    /// An action to configure the <see cref="GaussDBDataSourceBuilder" /> for further customizations of the <see cref="GaussDBDataSource" />.
    /// </param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="GaussDBConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="GaussDBDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddGaussDBDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<GaussDBDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddGaussDBDataSourceCore(serviceCollection, serviceKey, connectionString, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="GaussDBDataSource" /> and an <see cref="GaussDBConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An GaussDB connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="GaussDBConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="GaussDBDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddGaussDBDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddGaussDBDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="GaussDBDataSource" /> and an <see cref="GaussDBConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An GaussDB connection string.</param>
    /// <param name="dataSourceBuilderAction">
    /// An action to configure the <see cref="GaussDBSlimDataSourceBuilder" /> for further customizations of the <see cref="GaussDBDataSource" />.
    /// </param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="GaussDBConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="GaussDBDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddGaussDBSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<GaussDBSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddGaussDBSlimDataSourceCore(serviceCollection, serviceKey, connectionString, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="GaussDBDataSource" /> and an <see cref="GaussDBConnection" /> in the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An GaussDB connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="GaussDBConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="GaussDBDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddGaussDBSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddGaussDBSlimDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="GaussDBMultiHostDataSource" /> and an <see cref="GaussDBConnection" /> in the
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An GaussDB connection string.</param>
    /// <param name="dataSourceBuilderAction">
    /// An action to configure the <see cref="GaussDBDataSourceBuilder" /> for further customizations of the <see cref="GaussDBDataSource" />.
    /// </param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="GaussDBConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="GaussDBDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostGaussDBDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<GaussDBDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostGaussDBDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="GaussDBMultiHostDataSource" /> and an <see cref="GaussDBConnection" /> in the
    /// <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An GaussDB connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="GaussDBConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="GaussDBDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostGaussDBDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostGaussDBDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="GaussDBMultiHostDataSource" /> and an <see cref="GaussDBConnection" /> in the
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An GaussDB connection string.</param>
    /// <param name="dataSourceBuilderAction">
    /// An action to configure the <see cref="GaussDBDataSourceBuilder" /> for further customizations of the <see cref="GaussDBDataSource" />.
    /// </param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="GaussDBConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="GaussDBDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostGaussDBSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        Action<GaussDBSlimDataSourceBuilder> dataSourceBuilderAction,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostGaussDBSlimDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction, connectionLifetime, dataSourceLifetime);

    /// <summary>
    /// Registers an <see cref="GaussDBMultiHostDataSource" /> and an <see cref="GaussDBConnection" /> in the
    /// <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <param name="connectionString">An GaussDB connection string.</param>
    /// <param name="connectionLifetime">
    /// The lifetime with which to register the <see cref="GaussDBConnection" /> in the container.
    /// Defaults to <see cref="ServiceLifetime.Transient" />.
    /// </param>
    /// <param name="dataSourceLifetime">
    /// The lifetime with which to register the <see cref="GaussDBDataSource" /> service in the container.
    /// Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the data source.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    public static IServiceCollection AddMultiHostGaussDBSlimDataSource(
        this IServiceCollection serviceCollection,
        string connectionString,
        ServiceLifetime connectionLifetime = ServiceLifetime.Transient,
        ServiceLifetime dataSourceLifetime = ServiceLifetime.Singleton,
        object? serviceKey = null)
        => AddMultiHostGaussDBSlimDataSourceCore(
            serviceCollection, serviceKey, connectionString, dataSourceBuilderAction: null, connectionLifetime, dataSourceLifetime);

    static IServiceCollection AddGaussDBDataSourceCore(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        string connectionString,
        Action<GaussDBDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(GaussDBDataSource),
                serviceKey,
                (sp, key) =>
                {
                    var dataSourceBuilder = new GaussDBDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.Build();
                },
                dataSourceLifetime));

        AddCommonServices(serviceCollection, serviceKey, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    static IServiceCollection AddGaussDBSlimDataSourceCore(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        string connectionString,
        Action<GaussDBSlimDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(GaussDBDataSource),
                serviceKey,
                (sp, key) =>
                {
                    var dataSourceBuilder = new GaussDBSlimDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.Build();
                },
                dataSourceLifetime));

        AddCommonServices(serviceCollection, serviceKey, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    static IServiceCollection AddMultiHostGaussDBDataSourceCore(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        string connectionString,
        Action<GaussDBDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(GaussDBMultiHostDataSource),
                serviceKey,
                (sp, key) =>
                {
                    var dataSourceBuilder = new GaussDBDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.BuildMultiHost();
                },
                dataSourceLifetime));

        if (serviceKey is not null)
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(GaussDBDataSource),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<GaussDBMultiHostDataSource>(key),
                    dataSourceLifetime));
        }
        else
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(GaussDBDataSource),
                    sp => sp.GetRequiredService<GaussDBMultiHostDataSource>(),
                    dataSourceLifetime));

        }

        AddCommonServices(serviceCollection, serviceKey, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    static IServiceCollection AddMultiHostGaussDBSlimDataSourceCore(
        this IServiceCollection serviceCollection,
        object? serviceKey,
        string connectionString,
        Action<GaussDBSlimDataSourceBuilder>? dataSourceBuilderAction,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(GaussDBMultiHostDataSource),
                serviceKey,
                (sp, _) =>
                {
                    var dataSourceBuilder = new GaussDBSlimDataSourceBuilder(connectionString);
                    dataSourceBuilder.UseLoggerFactory(sp.GetService<ILoggerFactory>());
                    dataSourceBuilderAction?.Invoke(dataSourceBuilder);
                    return dataSourceBuilder.BuildMultiHost();
                },
                dataSourceLifetime));

        if (serviceKey is not null)
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(GaussDBDataSource),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<GaussDBMultiHostDataSource>(key),
                    dataSourceLifetime));
        }
        else
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(GaussDBDataSource),
                    sp => sp.GetRequiredService<GaussDBMultiHostDataSource>(),
                    dataSourceLifetime));

        }

        AddCommonServices(serviceCollection, serviceKey, connectionLifetime, dataSourceLifetime);

        return serviceCollection;
    }

    static void AddCommonServices(
        IServiceCollection serviceCollection,
        object? serviceKey,
        ServiceLifetime connectionLifetime,
        ServiceLifetime dataSourceLifetime)
    {
        // We don't try to invoke KeyedService methods if there is no service key.
        // This allows user code that use non-standard containers without support for IKeyedServiceProvider to keep on working.
        if (serviceKey is not null)
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(GaussDBConnection),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<GaussDBDataSource>(key).CreateConnection(),
                    connectionLifetime));

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbDataSource),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<GaussDBDataSource>(key),
                    dataSourceLifetime));

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbConnection),
                    serviceKey,
                    (sp, key) => sp.GetRequiredKeyedService<GaussDBConnection>(key),
                    connectionLifetime));
        }
        else
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(GaussDBConnection),
                    sp => sp.GetRequiredService<GaussDBDataSource>().CreateConnection(),
                    connectionLifetime));

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbDataSource),
                    sp => sp.GetRequiredService<GaussDBDataSource>(),
                    dataSourceLifetime));

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbConnection),
                    sp => sp.GetRequiredService<GaussDBConnection>(),
                    connectionLifetime));
        }
    }
}
