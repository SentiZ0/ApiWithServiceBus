using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using ServiceBusFunctions.Models;

namespace ServiceBusFunctions
{
    public class ServiceBusFunction
    {
        [FunctionName("UserRegisteredEventHandler")]
        public async Task Run([ServiceBusTrigger("user-registration-queue", 
            Connection = "ServiceBusConnection")] ServiceBusReceivedMessage  message,
            ServiceBusMessageActions messageActions, ILogger log)
        {
            try
            {
                log.LogInformation($"Received message: {message}");

                var userMessage = JsonSerializer.Deserialize<UserDto>(message.Body.ToString());

                if (userMessage == null)
                {
                    log.LogError("Message deserialization failed.");
                    throw new Exception("Invalid message format.");
                }

                if (userMessage.FirstName == "A" && userMessage.LastName == "A")
                {
                    log.LogWarning($"Dead-lettering message: {message}");
                    await messageActions.DeadLetterMessageAsync(message, "Business logic failure", "Invalid messages are not processed.");
                    return;
                }

                if (userMessage.FirstName == "B" && userMessage.LastName == "B")
                {
                    log.LogError("Simulated exception for testing retries.");
                    throw new Exception("Simulated processing error.");
                }

                log.LogInformation($"Processing valid user: {userMessage.FirstName} {userMessage.LastName}, Email: {userMessage.Email}");
            }
            catch (Exception ex)
            {
                log.LogError($"Exception while processing message: {ex.Message}");
                throw;
            }
        }
    }
}
