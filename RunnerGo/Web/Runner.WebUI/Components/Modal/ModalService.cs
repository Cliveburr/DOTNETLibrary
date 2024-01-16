using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Runner.WebUI.Components.Modal
{
    public class ModalService
    {
        public delegate void OnShowModalDelegate(Type modal, object data);
        public event OnShowModalDelegate? ShowModal;
        public delegate void OnCloseModalDelegate();
        public event OnCloseModalDelegate? CloseModal;

        public Task<(bool Ok, string Value)> Question(string title, string defaultValue, string? placeholder = null)
        {
            return Task.Run<(bool Ok, string Value)>(() =>
            {
                var data = new QuestionModal.QuestionData
                {
                    Title = title,
                    Value = defaultValue,
                    PlaceHolder = placeholder,
                    OkValue = false,
                    Resume = new ManualResetEvent(false)
                };
                ShowModal?.Invoke(typeof(QuestionModal), data);

                data.Resume.WaitOne();

                var ok = data.OkValue;
                var value = data.Value;

                CloseModal?.Invoke();

                return (ok, value);
            });
        }

        public Task<(bool Ok, List<InputFileModal.InputFile>? Files)> InputFile(string title, string fileLabel)
        {
            return Task.Run<(bool Ok, List<InputFileModal.InputFile>? Files)>(() =>
            {
                var data = new InputFileModal.InputFileData
                {
                    Title = title,
                    FileLabel = fileLabel,
                    OkValue = false,
                    Resume = new ManualResetEvent(false)
                };
                ShowModal?.Invoke(typeof(InputFileModal), data);

                data.Resume.WaitOne();

                CloseModal?.Invoke();

                return (data.OkValue, data.Files);
            });
        }
    }
}
