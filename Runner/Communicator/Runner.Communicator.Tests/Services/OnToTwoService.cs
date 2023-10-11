using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Communicator.Tests.Interfaces;
using Runner.Communicator.Tests.Model;

namespace Runner.Communicator.Tests.Services
{
    public class OnToTwoService : IOnToTwoInterface
    {
        public Task<string> Ping()
        {
            return Task.FromResult("PONG");
        }

        public Task PrimitiveParameters(bool bol, string str, int v1, short v2, long v3)
        {
            if (!bol || str != "STRING" || v1 != 123 || v2 != short.MaxValue || v3 != long.MinValue)
            {
                throw new Exception("PrimitiveParameters invalids!");
            }
            return Task.CompletedTask;
        }

        public Task<OneModel> ComplexModel(OneModel model)
        {
            if (model.Id != 123 || model.Name != "OneModel" || model.NullStr != null || model.TwoModel == null || model.NullTwoModel != null)
            {
                throw new Exception("ComplexModel invalids!");
            }
            var two = model.TwoModel;
            if (two.Id != 234 || two.Active != true || two.ThreeModels == null || two.ThreeModels.Count != 3)
            {
                throw new Exception("ComplexModel invalids!");
            }
            var three = two.ThreeModels;
            var nowDatetime = new DateTime(10000);
            if (three[0].NowDateTime != nowDatetime || three[1].NowDateTime != nowDatetime.AddDays(4)
                || three[2].NowDateTime != nowDatetime.AddMilliseconds(333))
            {
                throw new Exception("ComplexModel invalids!");
            }

            var backDatetime = new DateTime(20000);
            return Task.FromResult(new OneModel
            {
                Id = 321,
                Name = "Back",
                NullStr = null,
                NullTwoModel = null,
                TwoModel = new TwoModel
                {
                    Id = 432,
                    Active = false,
                    ThreeModels = new List<ThreeModel>
                    {
                        new ThreeModel { NowDateTime = backDatetime },
                        new ThreeModel { NowDateTime = backDatetime.AddMilliseconds(444) }
                    }
                }
            });
        }

        public Task GetTimeout(int milliseconds)
        {
            return Task.Delay(milliseconds);
        }

        public Task Voidtask()
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C dir.exe";
            process.StartInfo.WorkingDirectory = @"C:\";
            process.Start();
            return process.WaitForExitAsync();
        }
    }
}
