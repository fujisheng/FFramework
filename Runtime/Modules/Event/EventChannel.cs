using System;
using System.Collections.Generic;

namespace Framework.Module.Event
{
	public interface IEventChannel<T> : IDisposable
	{
		void Register(Action<T> listener);
		void Register(Action listener);
		void Unregister(Action<T> listener);
		void Unregister(Action listener);
		void Broadcast(T args);
	}

	public class EventChannel<T> : IEventChannel<T>
	{
		/// <summary>
		/// 存储带参数的回调监听列表
		/// </summary>
		readonly List<Action<T>> handlers;

		/// <summary>
		/// 存储无参数的回调监听列表
		/// </summary>
		readonly List<Action> plainHandlers;

		/// <summary>
		/// 构造函数，初始化事件通道
		/// </summary>
		public EventChannel()
		{
			handlers = new List<Action<T>>();
			plainHandlers = new List<Action>();
		}

		/// <summary>
		/// 注册带参数的回调监听
		/// </summary>
		public void Register(Action<T> listener)
		{
			if (handlers.Contains(listener))
			{
				return;
			}

			handlers.Add(listener);
		}

		/// <summary>
		/// 注册无参数的回调监听
		/// </summary>
		public void Register(Action listener)
		{
			if (plainHandlers.Contains(listener))
			{
				return;
			}

			plainHandlers.Add(listener);
		}

		/// <summary>
		/// 取消注册带参数的回调监听
		/// </summary>
		public void Unregister(Action<T> listener)
		{
			var index = handlers.IndexOf(listener);
			if (index < 0)
			{
				return;
			}
			handlers[index] = null;
		}

		/// <summary>
		/// 取消注册无参数的回调监听
		/// </summary>
		public void Unregister(Action listener)
		{
			var index = plainHandlers.IndexOf(listener);
			if (index < 0)
			{
				return;
			}
			plainHandlers[index] = null;
		}

		/// <summary>
		/// 广播事件，依次触发所有监听
		/// </summary>
		public void Broadcast(T args)
		{
			var index = 0;
			while (true)
			{
				if (index >= handlers.Count)
				{
					break;
				}

				var current = handlers[index];
				index++;
				current?.Invoke(args);
			}

			handlers.RemoveAll(item => item == null);

			index = 0;
			while (true)
			{
				if (index >= plainHandlers.Count)
				{
					break;
				}

				var currentPlain = plainHandlers[index];
				index++;
				currentPlain?.Invoke();
			}

			plainHandlers.RemoveAll(item => item == null);
		}

		/// <summary>
		/// 释放资源并清理所有监听
		/// </summary>
		public void Dispose()
		{
			handlers.Clear();
			plainHandlers.Clear();
		}
	}
}
