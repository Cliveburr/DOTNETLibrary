
namespace Runner.WebUI.Components.Modal
{
    public class ModalService
    {
        public delegate void OnShowModalDelegate(Type modal, ModalControl control);
        public event OnShowModalDelegate? ShowModal;
        public delegate void OnCloseModalDelegate();
        public event OnCloseModalDelegate? CloseModal;

        public Task<TResponse?> Show<TResponse>(Type modal)
        {
            var control = new ModalControl
            {
                Resume = new ManualResetEvent(false)
            };

            return Task.Run(() =>
            {
                ShowModal?.Invoke(modal, control);

                control.Resume.WaitOne();

                CloseModal?.Invoke();

                return control.Response == null ?
                    default :
                    (TResponse)control.Response;
            });
        }

        public Task<TResponse?> Show<TRequest, TResponse>(Type modal, TRequest request)
        {
            var control = new ModalControl
            {
                Request = request,
                Resume = new ManualResetEvent(false)
            };

            return Task.Run(() =>
            {
                ShowModal?.Invoke(modal, control);

                control.Resume.WaitOne();

                CloseModal?.Invoke();

                return control.Response == null ?
                    default :
                    (TResponse)control.Response;
            });
        }

        public Task<CreateNode.CreateNodeResponse?> CreateNode()
        {
            return Show<CreateNode.CreateNodeResponse>(typeof(CreateNode.CreateNodeModal));
        }

        public Task<string?> InputText(Inputs.InputTextRequest request)
        {
            return Show<Inputs.InputTextRequest, string>(typeof(Inputs.InputTextModal), request);
        }

        public Task<bool?> Question(Question.QuestionRequest request)
        {
            return Show<Question.QuestionRequest, bool?>(typeof(Question.QuestionModal), request);
        }

        //public Task<(bool Ok, List<InputFileModal.InputFile>? Files)> InputFile(string title, string fileLabel)
        //{
        //    return Task.Run<(bool Ok, List<InputFileModal.InputFile>? Files)>(() =>
        //    {
        //        var data = new InputFileModal.InputFileData
        //        {
        //            Title = title,
        //            FileLabel = fileLabel,
        //            OkValue = false,
        //            Resume = new ManualResetEvent(false)
        //        };
        //        ShowModal?.Invoke(typeof(InputFileModal), data);

        //        data.Resume.WaitOne();

        //        CloseModal?.Invoke();

        //        return (data.OkValue, data.Files);
        //    });
        //}
    }
}
