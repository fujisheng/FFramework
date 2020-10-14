using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Module.Timer
{
    internal sealed class TimerManager : Module
    {
        //当前时间的时间戳  单位是秒
        long currentTime = 0;
        float passTime = 0;

        List<Action<long>> tickerList = new List<Action<long>>();

        internal TimerManager()
        {
            currentTime = GetTimeStamp(false);
        }

        long GetTimeStamp(bool bflag)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
            long ret;
            if (bflag)
                ret = Convert.ToInt64(ts.TotalSeconds);
            else
                ret = Convert.ToInt64(ts.TotalMilliseconds);
            return ret;
        }

        public long GetTimeStamp()
        {
            return currentTime;
        }

        void Tick()
        {
            for(int i = 0; i < tickerList.Count; i++)
            {
                tickerList[i]?.Invoke(currentTime);
            }
        }

        internal override void OnLateUpdate()
        {
            passTime += Time.fixedDeltaTime;
            if(passTime >= 1)
            {
                currentTime += 1;
                Tick();
                passTime = 0;
            }
        }

        public void AddTicker(Action<long> ticker)
        {
            if(tickerList.Exists((tick) => tick == ticker))
            {
                return;
            }

            if(ticker == null)
            {
                return;
            }

            tickerList.Add(ticker);
        }

        public void RemoveTicker(Action<long> ticker)
        {
            if(!tickerList.Exists((tick) => tick == ticker))
            {
                return;
            }

            tickerList.Remove(ticker);
        }
    }
}