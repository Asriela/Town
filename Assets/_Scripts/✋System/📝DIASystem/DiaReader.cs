using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum DiaActionType : short
{
    none,
    menu,
    hangout
}

public enum DiaOptionType
{
    permanent,
    subtract,
    clear

}
public class DiaOption
{
    public int LineNumber { get; }
    public string Label { get; }
    public DiaActionType Action { get; }

    public DiaOptionType OptionType { get; }

    public DiaOption(int lineNumber, string label, DiaOptionType optionType, DiaActionType action)
    {
        LineNumber = lineNumber;
        Label = label;
        Action = action;
        OptionType = optionType;
    }
}


public static class DiaReader
{
    private static string currentFileText = "";
    private static List<string> allLines = new();
    private static string currentSection = "";
    private static string currentDialogue = "";
    private static int currentIndent = 1;
    private static int currentLine = 0;
    private static List<DiaOption> currentOptions= new();



    public static void OpenNewDialogue(string filename)
    {
        SetCurrentDialogueFile(filename);
        FindNextSection();
        Next(true);
    }


    public static DiaActionType? ChooseOption(string chosenOptionLabel)
    {
        DiaActionType? actionFromChosenOption = null;

        var chosenOption = FindOptionInCurrentOptions(chosenOptionLabel);
        currentLine = chosenOption.LineNumber;

        Next(false);

        return actionFromChosenOption;
    }



    public static void Next(bool firstNext)
    {
        //indent
        if (!firstNext)
        {
            currentIndent++;
        }


        //find the next dialogue at indent
        FindNextDialogue();

        //find the next options at the indent
        FindNextOptions();
        BasicFunctions.Log($"new section is: {currentSection}", LogType.dia);

        BasicFunctions.Log($"new dialogue is: {currentDialogue}", LogType.dia);

        foreach (var option in currentOptions)
        {
            BasicFunctions.Log($"new option is: {option.Label}", LogType.dia);
        }
    }

    public static void FindNextDialogue()
    {
        // Start from the currentLine index
        for (int i = currentLine; i < allLines.Count; i++)
        {
            string line = allLines[i];

            // Check if the line contains quotes
            int firstQuote = line.IndexOf('"');
            int secondQuote = line.IndexOf('"', firstQuote + 1);

            if (firstQuote != -1 && secondQuote != -1)
            {
                // Extract the text between the quotes
                currentDialogue = line.Substring(firstQuote + 1, secondQuote - firstQuote - 1);

                // Update currentLine to the next line for future searches
                currentLine = i + 1;

                return; // Exit the method once the dialogue is found
            }
        }

        // If no dialogue is found, set currentDialogue to an empty string
        currentDialogue = string.Empty;
    }

    private static void FindNextOptions()
    {
        // Clear the current options list
        currentOptions.Clear();

        // Iterate through all lines starting from the currentLine
        for (int i = currentLine; i < allLines.Count; i++)
        {
            string line = allLines[i].Trim();

            // Count the number of leading tabs
            int indentLevel = 0;
            while (indentLevel < line.Length && line[indentLevel] == '\t')
            {
                indentLevel++;
            }

            // Stop searching when we reach a line with fewer tabs than currentIndent
            if (indentLevel < currentIndent)
            {
                break;
            }

            // Check if the line starts with '>', '-', or '+'
            if (line.StartsWith(">") || line.StartsWith("-") || line.StartsWith("+"))
            {
                // Determine the OptionType based on the symbol at the beginning of the line
                var optionType = line.StartsWith("-") ? DiaOptionType.subtract :
                                 line.StartsWith("+") ? DiaOptionType.permanent :
                                 line.StartsWith(">") ? DiaOptionType.clear :
                                 DiaOptionType.permanent;

                // Extract the label (everything after the symbol)
                string label = line.Substring(1).Trim();

                // Create a new DiaOption object
                DiaOption newOption = new(i, label, optionType, DiaActionType.none);

                // Add the new option to the current options list
                currentOptions.Add(newOption);
            }
        }
    }




    private static void FindNextSection()
    {
        // Iterate through all lines starting from the current line
        for (int i = currentLine; i < allLines.Count; i++)
        {
            string line = allLines[i];

            // Check if the line contains "==" marking a section
            int startIndex = line.IndexOf("==");
            if (startIndex != -1)
            {
                int endIndex = line.IndexOf("==", startIndex + 2);

                if (endIndex != -1)
                {
                    // Extract the section name between the "=="
                    currentSection = line.Substring(startIndex + 2, endIndex - startIndex - 2).Trim();
                }
                else
                {
                    // No closing "==", take everything after the first "=="
                    currentSection = line.Substring(startIndex + 2).Trim();
                }

                // Update currentLine to the next line for future searches
                currentLine = i + 1;
                return;
            }
        }

        // No section found
        currentSection = string.Empty;
    }

    private static DiaOption FindOptionInCurrentOptions(string optionLabel)
    {
        foreach (var option in currentOptions)
        {
            if (option.Label == optionLabel)
            {
                return option;
            }
        }

        Debug.LogWarning($"Option '{optionLabel}' not found in current options.");
        return null;
    }

    public static void SetCurrentDialogueFile(string fileName)
    {
        currentFileText = DiaImporter.GetDiaFileText(fileName);
        TurnFileIntoLines();

    }
    public static void TurnFileIntoLines()
    {
        if (string.IsNullOrWhiteSpace(currentFileText))
        {
            return; // Do nothing if the input is null, empty, or whitespace
        }

        // Split the string by line breaks and add each line to the list
        allLines.AddRange(currentFileText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));


    }


}
