namespace Allycs.Common.Devices
{
    using System.Collections.Generic;

    /// <summary>
    /// 命令执行结果
    /// </summary>
    public class CommandResult
    {
        private readonly List<CommandError> _errors = new List<CommandError>();

        public CommandResult()
        { }

        /// <summary>
        /// 成功结果
        /// </summary>
        public static CommandResult SuccessResult => new CommandResult();

        /// <summary>
        /// 错误结果
        /// </summary>
        /// <param name="errorMessage"></param>
        public CommandResult(string errorMessage)
        {
            AddError(errorMessage);
        }

        /// <summary>
        /// 命令是否执行成功
        /// </summary>
        public bool Success => _errors.Count == 0;

        /// <summary>
        /// 命令是否执行失败
        /// </summary>
        public bool Fail => _errors.Count > 0;

        /// <summary>
        /// 添加错误消息
        /// </summary>
        /// <param name="error"></param>
        public void AddError(string error)
        {
            _errors.Add(new CommandError(error));
        }

        /// <summary>
        /// 错误消息列表
        /// </summary>
        public IReadOnlyList<CommandError> Errors => _errors.AsReadOnly();
    }

    /// <summary>
    /// 命令执行结果
    /// </summary>
    /// <typeparam name="TData">返回结果的附带数据</typeparam>
    public class CommandResult<TData> : CommandResult
    {
        public CommandResult()
        {
            Data = default(TData);
        }

        public CommandResult(string errorMessage) : base(errorMessage)
        {
            Data = default(TData);
        }

        /// <summary>
        /// 成功结果
        /// </summary>
        /// <param name="data"></param>
        new public static CommandResult<TData> SuccessResult(TData data) => new CommandResult<TData> { Data = data };

        /// <summary>
        /// 返回结果的数据
        /// </summary>
        public TData Data { get; set; }
    }

    /// <summary>
    /// 命令错误
    /// </summary>
    public class CommandError
    {
        public virtual string Message { get; }

        public CommandError(string message)
        {
            Message = message;
        }
    }
}