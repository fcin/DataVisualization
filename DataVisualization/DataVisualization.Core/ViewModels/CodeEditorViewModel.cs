using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Services;
using ICSharpCode.AvalonEdit;

namespace DataVisualization.Core.ViewModels
{
    public class CodeEditorViewModel : PropertyChangedBase, IHandle<RunCodeEventArgs>
    {
        private IEventAggregator _eventAggregator;
        public TextEditor TextEditor { get; set;}
        public ActionToolbarViewModel ActionToolbarVm { get; set; }

        public CodeEditorViewModel(ActionToolbarViewModel actionToolbarVm, IEventAggregator eventAggregator)
        {
            ActionToolbarVm = actionToolbarVm;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        public void Handle(RunCodeEventArgs message)
        {
            var programRunner = new ProgramRunner();
            var stdout = programRunner.Run(TextEditor.Document.Text);
            _eventAggregator.PublishOnUIThread(new OutputLogEventArgs { Output = stdout });
        }
    }
}
