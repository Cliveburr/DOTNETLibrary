﻿
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

        public Task<Inputs.InputFileResponse?> InputFiles(Inputs.InputFileRequest request)
        {
            return Show<Inputs.InputFileRequest, Inputs.InputFileResponse?>(typeof(Inputs.InputFileModal), request);
        }
    }
}