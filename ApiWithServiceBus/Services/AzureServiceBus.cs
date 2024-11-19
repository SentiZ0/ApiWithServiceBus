using ApiWithServiceBus.Models;
using ApiWithServiceBus.Options;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ApiWithServiceBus.Services
{
    public interface IAzureServiceBus
    {
        Task SendMessage(UserDto user);
    }

    public class AzureServiceBus : IAzureServiceBus
    {
        private ServiceBusClient _client;
        private readonly IOptions<ServiceBusOptions> _options;

        public AzureServiceBus(ServiceBusClient client, IOptions<ServiceBusOptions> options)
        {
            _client = client;
            _options = options;
        }

        public async Task SendMessage(UserDto user)
        {
            var sender = _client.CreateSender(_options.Value.QueueName);

            var serializedMessage = JsonSerializer.Serialize(user);
            var busMessage = new ServiceBusMessage(serializedMessage);

            await sender.SendMessageAsync(busMessage);
        }
    }
}
