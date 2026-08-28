using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplePad.Services
{
    /// <summary>
    /// Provides methods for displaying message boxes (dialogues) with different icons and button configurations, ensuring that only one dialogue is open at a time.
    /// </summary>
    public static class MessageBoxService
    {
        private static bool isDialogueOpened;

        /// <summary>
        /// Displays an error message box with the specified title and message.
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The message to display in the message box.</param>
        /// <returns></returns>
        public static async Task ShowErrorDialogue(string title, string message)
        {
            if (isDialogueOpened) return;
            isDialogueOpened = true;

            var messageBox = BuildDialogue(title, message, ["Ок"], Icon.Error);
            await messageBox.ShowAsync();

            isDialogueOpened = false;
        }

        /// <summary>
        /// Displays a warning message box with the specified title, message, and buttons, returning the button clicked by the user.
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The message to display in the message box.</param>
        /// <param name="buttons">The names of the buttons to display in the message box.</param>
        /// <returns>The button name clicked by the user.</returns>

        public static async Task<string> ShowWarningDialogue(string title, string message, string[] buttons)
        {
            if (isDialogueOpened) return "";
            isDialogueOpened = true;

            var messageBox = BuildDialogue(title, message, buttons, Icon.Warning);
            string result = await messageBox.ShowAsync();

            isDialogueOpened = false;

            return result;
        }

        /// <summary>
        /// Displays an information message box with the specified title, message, and buttons, returning the button clicked by the user.
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The message to display in the message box.</param>
        /// <param name="buttons">The names of the buttons to display in the message box.</param>
        /// <returns>The button name clicked by the user.</returns>

        public static async Task<string> ShowInfoDialogue(string title, string message, string[] buttons)
        {
            if (isDialogueOpened) return "";
            isDialogueOpened = true;

            var messageBox = BuildDialogue(title, message, buttons, Icon.Info);
            string result = await messageBox.ShowAsync();

            isDialogueOpened = false;

            return result;
        }

        /// <summary>
        /// Displays an information message box with the specified title, message, and buttons, returning the button clicked by the user.
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The message to display in the message box.</param>
        /// <param name="buttons">The names of the buttons to display in the message box.</param>
        /// <returns>The button name clicked by the user.</returns>

        public static async Task<string> ShowQuestionDialogue(string title, string message, string[] buttons)
        {
            if (isDialogueOpened) return "";
            isDialogueOpened = true;

            var messageBox = BuildDialogue(title, message, buttons, Icon.Question);
            string result = await messageBox.ShowAsync();

            isDialogueOpened = false;

            return result;
        }

        /// <summary>
        /// Builds a custom message box with the specified title, message, buttons, and icon.
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="message">The message to display in the message box.</param>
        /// <param name="buttons">The names of the buttons to display in the message box.</param>
        /// <param name="icon">The icon to display in the message box.</param>
        /// <returns>The custom message box.</returns>
        private static IMsBox<string> BuildDialogue(string title, string message, string[] buttons, Icon icon)
        {
            List<ButtonDefinition> buttonDefinitions = new();

            foreach (string button in buttons)
            {
                buttonDefinitions.Add(new ButtonDefinition { Name = button });
            }
            buttonDefinitions.Last().IsDefault = true;

            MessageBoxCustomParams msBoxParams = new()
            {
                Icon = icon,
                ContentTitle = title,
                ContentMessage = message,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.WidthAndHeight,
                CanResize = false,
                Topmost = true,
                ButtonDefinitions = buttonDefinitions,
            };

            return MessageBoxManager.GetMessageBoxCustom(msBoxParams);
        }
    }
}