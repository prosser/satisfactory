namespace SatisfactoryTools.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            this.DataContext = viewModel;
            this.InitializeComponent();
        }
    }
}