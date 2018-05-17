﻿using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;

namespace ServerlessMapReduceDotNet.ServerlessInfrastructure.Execution
{
    abstract class CommandDispatcherBase : ICommandDispatcher
    {
        public virtual Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(new CommandResult<TResult>(default(TResult), false));
        }

        public Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(new CommandResult(false));
        }

        public abstract ICommandExecuter AssociatedExecuter { get; }
    }
}