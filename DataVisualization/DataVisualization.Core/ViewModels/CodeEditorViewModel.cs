using System.Threading;
using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Services;
using ICSharpCode.AvalonEdit;

namespace DataVisualization.Core.ViewModels
{
    public class CodeEditorViewModel : PropertyChangedBase, IHandle<RunCodeEventArgs>, IHandle<StopCodeExecutionEventArgs>
    {
        private readonly IEventAggregator _eventAggregator;
        public TextEditor TextEditor { get; set;}
        public ActionToolbarViewModel ActionToolbarVm { get; set; }

        private CancellationTokenSource _cts;

        public CodeEditorViewModel(ActionToolbarViewModel actionToolbarVm, IEventAggregator eventAggregator)
        {
            ActionToolbarVm = actionToolbarVm;
            _eventAggregator = eventAggregator;
            _cts = new CancellationTokenSource();
            _eventAggregator.Subscribe(this);
        }

        public async void Handle(RunCodeEventArgs message)
        {
            var programRunner = new ProgramRunner();
            var stdout = await programRunner.RunAsync(TextEditor.Document.Text, _cts.Token);
            _eventAggregator.PublishOnUIThread(new OutputLogEventArgs { Output = stdout });
        }

        public void Handle(StopCodeExecutionEventArgs message)
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
        }
    }
}
