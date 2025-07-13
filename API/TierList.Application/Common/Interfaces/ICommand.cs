namespace TierList.Application.Common.Interfaces;

public interface ICommand : IBaseCommand
{
}

public interface ICommand<TResponse> : IBaseCommand
{
}

public interface IBaseCommand
{
}
