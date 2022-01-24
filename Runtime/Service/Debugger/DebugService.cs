using FInject;
using System;

namespace Framework.Service.Debug
{
    internal sealed class DebugService : Service, IDebugService
    {
        IDebugger debugger;
        int level = (int)DebugLevel.All;

        [Inject]
        public void SetDebugger(IDebugger debugger)
        {
            Utility.Assert.IfNull(debugger, new NullReferenceException($"debugger can not be null"));

            this.debugger = debugger;
        }

        /// <summary>
        /// 设置打印等级
        /// </summary>
        /// <param name="debugLevel">打印等级</param>
        public void SetLevel(DebugLevel debugLevel)
        {
            this.level = (int)debugLevel;
        }

        /// <summary>
        /// 判断是否有某个打印等级
        /// </summary>
        /// <param name="level">打印等级</param>
        /// <returns></returns>
        bool HasLevel(DebugLevel level)
        {
            return !((this.level & (int)level) == 0);
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="color">颜色</param>
        public void Log(object message, string color = "white")
        {
            if (!HasLevel(DebugLevel.Normal))
            {
                return;
            }
            debugger.Log(message, color);
        }

        /// <summary>
        /// 绿色的打印
        /// </summary>
        /// <param name="message">消息</param>
        public void LogG(object message)
        {
            if (!HasLevel(DebugLevel.Green))
            {
                return;
            }
            Log(message, "green");
        }

        /// <summary>
        /// 红色的打印
        /// </summary>
        /// <param name="message">消息</param>
        public void LogR(object message)
        {
            if (!HasLevel(DebugLevel.Red))
            {
                return;
            }
            Log(message, "red");
        }

        /// <summary>
        /// 黄色的打印
        /// </summary>
        /// <param name="message">消息</param>
        public void LogY(object message)
        {
            if (!HasLevel(DebugLevel.Yellow))
            {
                return;
            }
            Log(message, "yellow");
        }

        /// <summary>
        /// 蓝色的打印
        /// </summary>
        /// <param name="message">消息</param>
        public void LogB(object message)
        {
            if (!HasLevel(DebugLevel.Blue))
            {
                return;
            }
            Log(message, "blue");
        }

        /// <summary>
        /// 打印警告
        /// </summary>
        /// <param name="message">消息</param>
        public void LogWarning(object message)
        {
            if (!HasLevel(DebugLevel.Warning))
            {
                return;
            }
            debugger.LogWarning(message);
        }

        /// <summary>
        /// 打印错误
        /// </summary>
        /// <param name="message">消息</param>
        public void LogError(object message)
        {
            if (!HasLevel(DebugLevel.Error))
            {
                return;
            }
            debugger.LogError(message);
        }
    }
}

