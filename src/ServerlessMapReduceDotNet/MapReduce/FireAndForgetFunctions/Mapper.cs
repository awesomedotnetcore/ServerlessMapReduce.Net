using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using ServerlessMapReduceDotNet.Commands.ObjectStore;
using ServerlessMapReduceDotNet.MapReduce.Abstractions;
using ServerlessMapReduceDotNet.MapReduce.Commands.Map;
using ServerlessMapReduceDotNet.ServerlessInfrastructure.Abstractions;

namespace ServerlessMapReduceDotNet.MapReduce.FireAndForgetFunctions
{    
    public class Mapper : IMapper
    {
        private readonly IQueueClient _queueClient;
        private readonly IConfig _config;
        private readonly IWorkerRecordStoreService _workerRecordStoreService;
        private readonly ICommandDispatcher _commandDispatcher;

        public Mapper(IQueueClient queueClient, IConfig config, IWorkerRecordStoreService workerRecordStoreService, ICommandDispatcher dispatch)
        {
            _queueClient = queueClient;
            _config = config;
            _workerRecordStoreService = workerRecordStoreService;
            _commandDispatcher = dispatch;
        }

        public async Task InvokeAsync()
        {
            var instanceWorkerId = _workerRecordStoreService.GenerateUniqueId();
            Console.WriteLine($"Mapper {instanceWorkerId} starting...");
            await _workerRecordStoreService.RecordPing("mapper", instanceWorkerId);
            
            var ingestedQueueMessages = await _queueClient.Dequeue(_config.IngestedQueueName, _config.MaxQueueItemsBatchSizeToProcessPerWorker);
            if (ingestedQueueMessages.Count == 0)
            {
                await _workerRecordStoreService.RecordHasTerminated("mapper", instanceWorkerId);
                return;
            }
            
            foreach (var ingestedQueueMessage in ingestedQueueMessages)
            {
                await _workerRecordStoreService.RecordPing("mapper", instanceWorkerId);

                Stream ingestedObjectStream = await _commandDispatcher.DispatchAsync(new RetrieveObjectCommand{Key = ingestedQueueMessage.Message});

                using (var streamReader = new StreamReader(ingestedObjectStream))
                {
                    var lines = new List<string>();
                    while (!streamReader.EndOfStream)
                    {
                        var line = await streamReader.ReadLineAsync();
                        lines.Add(line);
                    }

                    await _commandDispatcher.DispatchAsync(new BatchMapDataCommand
                    {
                        Lines = lines,
                        ContextQueueMessage = ingestedQueueMessage
                    });
                }

                await _queueClient.MessageProcessed(_config.IngestedQueueName, ingestedQueueMessage.MessageId);
            }
            
            await _workerRecordStoreService.RecordHasTerminated("mapper", instanceWorkerId);
            Console.WriteLine($"Mapper {instanceWorkerId} Terminated");
        }
    }
}