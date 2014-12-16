using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace 周公解梦
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<string> _results = new ObservableCollection<string>();

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            Msg.Text = "问问周公吧";
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Results
        {
            get
            {
                return _results;
            }
            set
            {
                _results = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Results"));
                }
            }
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: 准备此处显示的页面。

            // TODO: 如果您的应用程序包含多个页面，请确保
            // 通过注册以下事件来处理硬件“后退”按钮:
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed 事件。
            // 如果使用由某些模板提供的 NavigationHelper，
            // 则系统会为您处理该事件。

            if (VoiceCommandHelper.StartVoiceCommandArgs != null)
            {
                VoiceCommandActivatedEventArgs args = VoiceCommandHelper.StartVoiceCommandArgs;
                SpeechRecognitionResult result = args.Result;
                var key = result.Text;
                TxtSearch.Text = key;
                await GoSearch(key);
            }
        }

        private async Task GoSearch(string key)
        {
            StatusBar.GetForCurrentView().ProgressIndicator.Text = "周公正在解梦中……";
            await StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();

            TxtSearch.Text = key;
            var results = await DreamHelper.GetResult(key);
            Results = new ObservableCollection<string>(results);

            if (results.Count == 0)
            {
                Msg.Text = "对不起，周公表示无能为力";
            }
            else
            {
                Msg.Text = "";
            }

            await StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
        }

        private async void UIElement_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            string text = null;
            try
            {
                using (var recognizer = new SpeechRecognizer(new Language("zh-Hans-CN")))
                {
                    await recognizer.CompileConstraintsAsync();
                    var result = await recognizer.RecognizeWithUIAsync();
                    text = result.Text.Trim(',', '.');
                }
            }
            catch
            {
                text = null;
            }

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            await GoSearch(text);
        }

        private async void UIElement_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var textBox = sender as TextBox;
                if (textBox == null)
                {
                    return;
                }

                var text = textBox.Text;
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                FakeButton.Focus(FocusState.Programmatic);

                await GoSearch(text);
            }
        }
    }
}