using System.Threading;
using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Models;
using DataVisualization.Services;
using ICSharpCode.AvalonEdit;

namespace DataVisualization.Core.ViewModels
{
    public class CodeEditorViewModel : PropertyChangedBase, IHandle<RunCodeEventArgs>, IHandle<StopCodeExecutionEventArgs>, IHandle<DataConfigurationOpenedEventArgs>
    {
        private readonly IEventAggregator _eventAggregator;
        public TextEditor TextEditor { get; set;}
        public ActionToolbarViewModel ActionToolbarVm { get; set; }

        private CancellationTokenSource _cts;
        private DataService _dataService;

        public CodeEditorViewModel(ActionToolbarViewModel actionToolbarVm, IEventAggregator eventAggregator, DataService dataService)
        {
            ActionToolbarVm = actionToolbarVm;
            _eventAggregator = eventAggregator;
            _dataService = dataService;
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

        public void Handle(DataConfigurationOpenedEventArgs message)
        {
            if (!(message.Opened is ScriptDataConfiguration config))
                return;

            var data = _dataService.GetData<ScriptData>(config.DataName);
        }
    }
}
