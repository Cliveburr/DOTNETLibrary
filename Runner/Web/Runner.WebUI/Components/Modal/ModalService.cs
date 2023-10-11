using Runner.Business.Entities;

namespace Runner.WebUI.Components.Modal
{
    public class ModalService
    {
        public ModalComponent? Component { get; set; }

        public Task<(bool Ok, string Value)> Question(string question, string defaultValue, string placeholder)
        {
            Assert.MustNotNull(Component, "Modal is not ready!");
            return Component.Question(question, defaultValue, placeholder);
        }

        public Task<(bool Ok, NodeType Value)> NodeTypeSelection()
        {
            Assert.MustNotNull(Component, "Modal is not ready!");
            return Component.NodeTypeSelection();
        }
    }
}
