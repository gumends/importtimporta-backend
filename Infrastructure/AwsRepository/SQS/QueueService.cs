using Amazon.SQS;
using Amazon.SQS.Model;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

public class QueueService : IQueueService
{
    private readonly IAmazonSQS _sqs;
    private readonly string _queueUrl;

    public QueueService(IConfiguration configuration)
    {
        _sqs = new AmazonSQSClient();
        _queueUrl = configuration["AWS:SQS:QueueUrl"] ?? "";
    }

    public async Task SendMessageAsync(string messageBody)
    {
        var request = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = messageBody,
            MessageGroupId = "email-group",
            MessageDeduplicationId = Guid.NewGuid().ToString()
        };

        await _sqs.SendMessageAsync(request);
    }
}