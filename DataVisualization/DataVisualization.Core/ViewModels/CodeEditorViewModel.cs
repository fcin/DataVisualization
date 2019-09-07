using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
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
        private ScriptDataConfiguration _config;
        private readonly ScriptDataService _scriptDataService;

        public CodeEditorViewModel(ActionToolbarViewModel actionToolbarVm, IEventAggregator eventAggregator, ScriptDataService scriptDataService)
        {
            ActionToolbarVm = actionToolbarVm;
            _eventAggregator = eventAggregator;
            _scriptDataService = scriptDataService;
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

        public async void Handle(DataConfigurationOpenedEventArgs message)
        {
            if (!(message?.Opened is ScriptDataConfiguration config) || message?.Publisher == this)
                return;

            _config = config;

            var data = await _scriptDataService.GetDataAsync(_config.DataName);
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                TextEditor.Document.Text = data.Data;
            });

            _eventAggregator.PublishOnUIThread(new DataConfigurationOpenedEventArgs(this, _config));
        }

        public async void OnSave()
        {
            var content = TextEditor.Document.Text;
            await _scriptDataService.SaveAsync(_config.DataName, content);
        }
    }
}
