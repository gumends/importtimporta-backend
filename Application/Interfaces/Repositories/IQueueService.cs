namespace Application.Interfaces.Repositories;

public interface IQueueService
{
    Task SendMessageAsync(string messageBody);
}