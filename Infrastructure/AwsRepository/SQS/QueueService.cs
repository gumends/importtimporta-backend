using Amazon.SQS;
using Amazon.SQS.Model;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;

public class QueueService : IQueueService
{
    private readonly IAmazonSQS? _sqs;
    private readonly string _queueUrl;

    public QueueService(IConfiguration configuration)
    {
        _queueUrl = configuration["AWS:SQS:QueueUrl"] ?? "";
        if (!string.IsNullOrWhiteSpace(_queueUrl))
            _sqs = new AmazonSQSClient();
    }

    public async Task SendMessageAsync(string messageBody)
    {
        if (_sqs == null || string.IsNullOrWhiteSpace(_queueUrl))
        {
            Console.WriteLine("SQS não configurado. Mensagem de e-mail ignorada no ambiente local.");
            return;
        }

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