using Microsoft.Extensions.DependencyInjection;
using SmartGreenhouse.Application.Abstractions;
using SmartGreenhouse.Domain.Entities;
using SmartGreenhouse.Domain.Enums;

namespace SmartGreenhouse.Application.DeviceIntegration;

/// <summary>
/// Interface for resolving device integration factories based on device type.
/// </summary>
public interface IDeviceFactoryResolver
{
    /// <summary>
    /// Resolves the appropriate device integration factory for the given device.
    /// </summary>
    /// <param name="device">The device to resolve a factory for</param>
    /// <returns>The appropriate device integration factory</returns>
    IDeviceIntegrationFactory Resolve(Device device);
}

/// <summary>
/// Resolver that picks the correct device integration factory based on DeviceType.
/// Uses dependency injection to resolve concrete factory instances.
/// </summary>
public class DeviceFactoryResolver : IDeviceFactoryResolver
{
    private readonly IServiceProvider _serviceProvider;

    public DeviceFactoryResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IDeviceIntegrationFactory Resolve(Device device)
    {
        return device.DeviceType switch
        {
            DeviceTypeEnum.Simulated => _serviceProvider.GetRequiredService<SimulatedDeviceFactory>(),
            
            // Future device types can be added here:
            // DeviceTypeEnum.MqttEdge => _serviceProvider.GetRequiredService<MqttDeviceFactory>(),
            // DeviceTypeEnum.UsbProbe => _serviceProvider.GetRequiredService<UsbDeviceFactory>(),
            
            _ => throw new NotSupportedException($"Device type '{device.DeviceType}' is not supported yet. " +
                                               $"Please implement a corresponding device integration factory.")
        };
    }
}