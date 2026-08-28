using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using System;
using System.Text.RegularExpressions;

namespace SimplePad.Transformers
{
    /// <summary>
    /// Transformer class for colorizing search results in a text editor. 
    /// <br/><br/>
    /// It highlights specific parts of the text like line numbers, search matches, and replacement text with different colors for better visibility.
    /// </summary>
    public class SearchResultsTransformer : DocumentColorizingTransformer
    {
        private string _searchText = "???";
        private string _replaceText = "???";

        private static readonly SolidColorBrush PaleGoldenrodBrush = new(Colors.PaleGoldenrod);
        private static readonly SolidColorBrush LightSkyBlueBrush = new(Colors.LightSkyBlue);
        private static readonly SolidColorBrush GoldBrush = new(Colors.Gold);
        private static readonly SolidColorBrush LightGreenBrush = new(Colors.LightGreen);
        private static readonly SolidColorBrush IndianRedBrush = new(Colors.IndianRed);

        /// <summary>
        /// Method to update search text dynamically
        /// </summary>
        /// <param name="searchText"></param>
        public void UpdateSearchText(string searchText)
        {
            _searchText = searchText;
        }

        /// <summary>
        /// Method to update replace text dynamically
        /// </summary>
        /// <param name="replaceText"></param>
        public void UpdateReplaceText(string replaceText)
        {
            _replaceText = replaceText;
        }

        /// <summary>
        /// Overrides the ColorizeLine method to apply custom colorization logic to each line of the document.
        /// </summary>
        /// <param name="line"></param>
        protected override void ColorizeLine(DocumentLine line)
        {
            string lineText = CurrentContext.Document.GetText(line);

            // Skip empty lines
            if (string.IsNullOrWhiteSpace(lineText)) return;

            if (lineText.StartsWith("Error"))
            {
                ColorizeErrorLine(line);
            }
            else if (!lineText.StartsWith("line "))
            {
                ColorizeNonSearchLine(line);
            }
            else
            {
                ColorizeSearchResultLine(line, lineText);
            }
        }

        /// <summary>
        /// Colors the entire line with IndianRed for error lines.
        /// </summary>
        private void ColorizeErrorLine(DocumentLine line)
        {
            ChangeLinePart(
                line.Offset,
                line.Offset + line.Length,
                element =>
                {
                    element.TextRunProperties.SetForegroundBrush(IndianRedBrush);
                }
            );
        }

        /// <summary>
        /// Colors the entire line with PaleGoldenrod for lines that don't start with "line ".
        /// </summary>
        private void ColorizeNonSearchLine(DocumentLine line)
        {
            ChangeLinePart(
                line.Offset,
                line.Offset + line.Length,
                element =>
                {
                    element.TextRunProperties.SetForegroundBrush(PaleGoldenrodBrush);
                }
            );
        }

        /// <summary>
        /// Processes search result lines (line X: ...) and applies specific colorization.
        /// </summary>
        private void ColorizeSearchResultLine(DocumentLine line, string lineText)
        {
            var match = Regex.Match(lineText, @"^(line )(\d+)(: )");

            if (!match.Success) return;

            MakeLineNumberBold(line, match);
            ColorizeLineNumber(line, match);
            HighlightSearchMatches(line, lineText, match.Length);
            HighlightReplaceText(line, lineText);
        }

        /// <summary>
        /// Makes "line X:" part of the line bold.
        /// </summary>
        private void MakeLineNumberBold(DocumentLine line, Match match)
        {
            ChangeLinePart(
                line.Offset + match.Index,
                line.Offset + match.Index + match.Length,
                element =>
                {
                    var tf = element.TextRunProperties.Typeface;
                    element.TextRunProperties.SetTypeface(
                        new Typeface(tf.FontFamily, tf.Style, FontWeight.Bold, tf.Stretch)
                    );
                }
            );
        }

        /// <summary>
        /// Colors the line number in LightSkyBlue.
        /// </summary>
        private void ColorizeLineNumber(DocumentLine line, Match match)
        {
            ChangeLinePart(
                line.Offset + match.Groups[2].Index,
                line.Offset + match.Groups[2].Index + match.Groups[2].Length,
                element =>
                {
                    element.TextRunProperties.SetForegroundBrush(LightSkyBlueBrush);
                }
            );
        }

        /// <summary>
        /// Highlights all occurrences of the search text in Gold.
        /// </summary>
        private void HighlightSearchMatches(DocumentLine line, string lineText, int startIndex)
        {
            if (string.IsNullOrEmpty(_searchText)) return;

            string searchText = lineText.Substring(startIndex);
            HighlightTextOccurrences(line, searchText, _searchText, startIndex, GoldBrush);
        }

        /// <summary>
        /// Highlights text after " => " in LightGreen.
        /// </summary>
        private void HighlightReplaceText(DocumentLine line, string lineText)
        {
            if (string.IsNullOrEmpty(_replaceText)) return;

            string replaceMarker = " => ";
            int markerIndex = lineText.IndexOf(replaceMarker);

            if (markerIndex == -1) return;

            int replaceStartIndex = markerIndex + replaceMarker.Length;

            // Check if there's text after the marker
            if (replaceStartIndex >= lineText.Length) return;

            string textAfterMarker = lineText.Substring(replaceStartIndex);
            HighlightTextOccurrences(line, textAfterMarker, _replaceText, replaceStartIndex, LightGreenBrush);
        }

        /// <summary>
        /// Generic method to find and highlight all occurrences of a specific text pattern.
        /// </summary>
        private void HighlightTextOccurrences(DocumentLine line, string text, string searchPattern, int offset, Brush brush)
        {
            int searchStart = 0;

            while (true)
            {
                int index = text.IndexOf(searchPattern, searchStart, StringComparison.OrdinalIgnoreCase);
                if (index == -1) break;

                int absoluteIndex = line.Offset + offset + index;

                ChangeLinePart(
                    absoluteIndex,
                    absoluteIndex + searchPattern.Length,
                    element =>
                    {
                        element.TextRunProperties.SetForegroundBrush(brush);
                    }
                );

                searchStart = index + searchPattern.Length;
            }
        }
    }
}