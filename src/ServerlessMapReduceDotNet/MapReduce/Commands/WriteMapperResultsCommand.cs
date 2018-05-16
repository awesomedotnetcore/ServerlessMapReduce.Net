﻿using AzureFromTheTrenches.Commanding.Abstractions;
using ServerlessMapReduceDotNet.Model;

namespace ServerlessMapReduceDotNet.MapReduce.Commands
{
    public class WriteMapperResultsCommand : ICommand
    {
        public KeyValuePairCollection ResultOfMap { get; set; }
        public QueueMessage ContextQueueMessage { get; set; }
    }
}