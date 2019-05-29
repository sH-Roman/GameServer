using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer.Timera
{
    //当定时器达到时间后触发
    public delegate void TimeDelegate();

    /// <summary>
    /// 定时器的数据模型
    /// </summary>
    public class TimerModel
    {
        private int Id;

        //任务执行的时间
        private long Time;

        private TimeDelegate timeDelegate;

        public TimerModel(int id, long time, TimeDelegate td)
        {
            this.Id = id;
            this.Time = time;
            this.timeDelegate = td;
        }

        public void Run()
        {
            timeDelegate();
        }
    }
}
