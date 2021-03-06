﻿using AzureFromTheTrenches.Commanding.Abstractions;
using ServerlessMapReduceDotNet.ServerlessInfrastructure.Abstractions;

namespace ServerlessMapReduceDotNet.HostingEnvironments
{
    public interface IRegisterFireAndForgetHandler
    {
        IRegisterFireAndForgetHandler RegisterFireAndForgetFunctionImpl<TFunction, TCommand>()
            where TFunction : class, IFireAndForgetFunction
            where TCommand : ICommand;
    }
}