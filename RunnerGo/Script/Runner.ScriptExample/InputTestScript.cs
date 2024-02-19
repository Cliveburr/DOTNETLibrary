using Runner.Script.Interface.Scripts;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Runner.ScriptExample
{
    public class InputData
    {
        [Required]
        [DefaultValue("test nome")]
        public required string MyName { get; set; }

        public required InputDataCompany Company { get; set; }
    }

    public class InputDataCompany
    {
        [Required]
        public required string Name { get; set; }
    }

    [Script(0, "InputTest", typeof(InputData))]
    public class InputTestScript : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            await context.Log("input test running...");

            var input = context.Data.ReadInput<InputData>();
            if (input is null)
            {
                context.Data.SetString("InputData", "null");
            }
            else
            {
                context.Data.SetString("InputData", $"MyName: {input.MyName} - Company.Name: {input.Company.Name}");
            }
        }
    }
}
