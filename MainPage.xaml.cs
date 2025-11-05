using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HerbavitaCMR.Models;
using HerbavitaCMR.Services;
using HerbavitaCMR.Views;
using Microsoft.Maui.Controls; // SelectionChangedEventArgs, CollectionView

namespace HerbavitaCMR
{
    public partial class MainPage : ContentPage
    {
        readonly CMRService _cmrService = new();
        bool _isLoaded;

        public ObservableCollection<Document> Documents { get; } = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_isLoaded)
                return;

            await LoadDataAsync();
            _isLoaded = true;
        }

        async Task LoadDataAsync()
        {
            try
            {
                var cmrDocuments = await _cmrService.GetCmrDocumentAsync(forceRefresh: true);
                Documents.Clear();
                if (cmrDocuments != null)
                {
                    foreach (var d in cmrDocuments)
                        Documents.Add(d);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        [Obsolete]
        async void Documents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDocument = e.CurrentSelection?.FirstOrDefault() as Document;
            if (selectedDocument == null)
            {
                if (sender is CollectionView cv) cv.SelectedItem = null;
                return;
            }

            try
            {
                var previewPage = new CmrPreviewPage(selectedDocument);

                if (Navigation != null)
                {
                    await Navigation.PushAsync(previewPage);
                }
                else if (Application.Current != null)
                {
                    Application.Current.MainPage = new NavigationPage(previewPage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.Message}");
            }
            finally
            {
                if (sender is CollectionView collectionView)
                    collectionView.SelectedItem = null;
            }
        }
    }
}