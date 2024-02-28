using Runner.Business.Entities.Job;

namespace Runner.Business.Model.Schedule
{
    public class JobTickerAndSchedule
    {
        public required JobTicker Ticker { get; set; }
        public required JobSchedule Schedule { get; set; }
    }
}
