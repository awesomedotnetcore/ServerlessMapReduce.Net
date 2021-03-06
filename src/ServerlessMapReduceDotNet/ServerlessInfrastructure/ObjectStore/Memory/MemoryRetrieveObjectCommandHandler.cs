﻿using System.IO;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using ServerlessMapReduceDotNet.Commands.ObjectStore;
using ServerlessMapReduceDotNet.ServerlessInfrastructure.Abstractions;

namespace ServerlessMapReduceDotNet.ServerlessInfrastructure.ObjectStore.Memory
{
    class MemoryRetrieveObjectCommandHandler : ICommandHandler<RetrieveObjectCommand, Stream>
    {
        private readonly IMemoryObjectStore _memoryObjectStore;

        public MemoryRetrieveObjectCommandHandler(IMemoryObjectStore memoryObjectStore)
        {
            _memoryObjectStore = memoryObjectStore;
        }

        public Task<Stream> ExecuteAsync(RetrieveObjectCommand command, Stream previousResult)
        {
            var storedObject = _memoryObjectStore.Retrieve(command.Key);
            var memoryStream = new MemoryStream(storedObject.Data);
            return Task.FromResult((Stream)memoryStream);
        }
    }
}