using Runner.Business.Entities.Job;

namespace Runner.Business.Model
{
    public class JobTickerAndSchedule
    {
        public required JobTicker Ticker { get; set; }
        public JobSchedule? Schedule { get; set; }
    }
}
