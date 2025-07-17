namespace CQRS.Base;

public interface ICommand : IBaseCommand { }

public interface ICommand<TResponse> : IBaseCommand { }

public interface IBaseCommand { }