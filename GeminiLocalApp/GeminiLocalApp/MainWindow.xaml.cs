using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Google.GenAI;
using Google.GenAI.Types;
using System.Threading.Tasks;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GeminiLocalApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Client client;
        private Chat chat;

        public MainWindow()
        {
            InitializeComponent();

            client = new Client();
            chat = new Chat(client);

            SendBtn.Click += SendBtn_Click;
        }

        /// <summary>
        /// Handles the Click event of the Send button.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Send button.</param>
        /// <param name="e">The event data associated with the Click event.</param>
        private void SendBtn_Click (object sender, RoutedEventArgs e)
        {
            RespondAsync();
        }

        /// <summary>
        /// This is the sending function.
        /// </summary>
        private async Task RespondAsync()
        {
            Content content = new Content();
            content.Role = "user";
            content.Parts = new List<Part>() {
                new Part
                {
                    Text = SuperPrompt.Text
                }
            };
            Superresponse.Text = string.Empty;
            await foreach (var chunk in chat.SendMessageStream(content))
            {
                if (chunk.Parts != null)
                {
                    Part part = chunk.Parts[0];
                    Superresponse.Text += part.Text;
                }
            }
        }
    }
}
