using Framework.IoC;
using System;

namespace Framework.Module.Log
{
    [DefaultModuleDependency]
    internal sealed class LogModule : Module, ILogModule
    {
        ILogger logger;
        int level = (int)LogLevel.All;

        [Inject]
        public void SetLogger(ILogger debugger)
        {
            Utility.Assert.IfNull(debugger, new NullReferenceException($"debugger can not be null"));

            this.logger = debugger;
        }

        /// <summary>
        /// 设置打印等级
        /// </summary>
        /// <param name="debugLevel">打印等级</param>
        public void SetLevel(LogLevel debugLevel)
        {
            this.level = (int)debugLevel;
        }

        /// <summary>
        /// 判断是否有某个打印等级
        /// </summary>
        /// <param name="level">打印等级</param>
        /// <returns></returns>
        bool HasLevel(LogLevel level)
        {
            return !((this.level & (int)level) == 0);
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="color">颜色</param>
        public void Info(object message, string color = "white")
        {
            if (!HasLevel(LogLevel.Normal))
            {
                return;
            }
            logger.Info(message, color);
        }

        /// <summary>
        /// 绿色的打印
        /// </summary>
        /// <param name="message">消息</param>
        public void InfoG(object message)
        {
            if (!HasLevel(LogLevel.Green))
            {
                return;
            }
            Info(message, "green");
        }

        /// <summary>
        /// 红色的打印
        /// </summary>
        /// <param name="message">消息</param>
        public void InfoR(object message)
        {
            if (!HasLevel(LogLevel.Red))
            {
                return;
            }
            Info(message, "red");
        }

        /// <summary>
        /// 黄色的打印
        /// </summary>
        /// <param name="message">消息</param>
        public void InfoY(object message)
        {
            if (!HasLevel(LogLevel.Yellow))
            {
                return;
            }
            Info(message, "yellow");
        }

        /// <summary>
        /// 蓝色的打印
        /// </summary>
        /// <param name="message">消息</param>
        public void InfoB(object message)
        {
            if (!HasLevel(LogLevel.Blue))
            {
                return;
            }
            Info(message, "blue");
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        /// <param name="message">消息</param>
        public void Warning(object message)
        {
            if (!HasLevel(LogLevel.Warning))
            {
                return;
            }
            logger.Warning(message);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        /// <param name="message">消息</param>
        public void Error(object message)
        {
            if (!HasLevel(LogLevel.Error))
            {
                return;
            }
            logger.Error(message);
        }
    }
}

