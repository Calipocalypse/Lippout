// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using WebviewAppTest;

namespace BlazorWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppState _appState = new();

        public MainWindow()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWpfBlazorWebView();
            serviceCollection.AddSingleton<AppState>(_appState);
            Resources.Add("services", serviceCollection.BuildServiceProvider());

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                owner: this,
                messageBoxText: $"Current counter value is: ",
                caption: "Counter");
        }
        private void ShowLipFile(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_appState.LipFile.NumberOfPhonems.ToString());
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // SprawdŸ, czy klawisz F5 zosta³ naciœniêty
            if (e.Key == Key.F6)
            {
                var bytes = _appState.GeneratedBytesToSave;
                if (bytes.Length == 0)
                {
                    MessageBox.Show("Nothing to save!");
                }
                else
                {
                    File.WriteAllBytes(@$"A:\FA2163CLONE\ALTERNATE\data\sound\Speech\LIEUT\{_appState.OutputFileName}.LIP", bytes);
                    MessageBox.Show($"SAVED AS {_appState.OutputFileName}.LIP!");
                }
            }
        }
    }

    // Workaround for compiler error "error MC3050: Cannot find the type 'local:Main'"
    // It seems that, although WPF's design-time build can see Razor components, its runtime build cannot.
    public partial class Main { }

    // Helpful guide on WCF: https://docs.microsoft.com/en-us/aspnet/core/blazor/hybrid/tutorials/wpf?view=aspnetcore-6.0
}
