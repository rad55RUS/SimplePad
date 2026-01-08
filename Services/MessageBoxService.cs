using System.Collections.Generic;
using System.Threading.Tasks;

using Avalonia.Controls;

using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;

namespace SimplePad.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageBoxService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static IMsBox<string> Dialogue(string title, string message, string[] buttons)
        {
            List<ButtonDefinition> buttonDefinitions = new();

            foreach (string button in buttons)
            {
                buttonDefinitions.Add(new ButtonDefinition { Name = button });
            }

            MessageBoxCustomParams msBoxParams = new()
            {
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
