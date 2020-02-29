namespace SatisfactoryTools.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Services;

    public class MainWindowViewModel
    {
        private readonly ApplicationState state;

        public MainWindowViewModel(ApplicationState state)
        {
            this.state = state;
        }

        public Task SaveState(CancellationToken ct = default)
        {
            return this.state.SaveAsync(ct);
        }
    }
}