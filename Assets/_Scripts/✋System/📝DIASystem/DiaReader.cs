using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public enum DiaActionType : short
{
    none,
    menu,
    menu_askAbout,
    action_hangout
}

public enum DiaOptionType
{
    none,
    permanent,
    subtract,
    clear

}
public class DiaOption
{
    public int LineNumber { get; }
    public string Label { get; }

    public int Index { get; }

    public int TabLevel { get; }
    public DiaActionType Action { get; }

    public DiaOptionType OptionType { get; }

    public DiaOption(int lineNumber, string label, DiaOptionType optionType, DiaActionType action, int index, int tabLevel)
    {
        LineNumber = lineNumber;
        Label = label;
        Action = action;
        OptionType = optionType;
        Index = index;
        TabLevel = tabLevel;
    }
}

public class DiaPackage
{
    public string Dialogue { get; }
    public List<DiaOption> Options { get; }

    public DiaPackage(string dialogue, List<DiaOption> options)
    {
        Dialogue = dialogue;
        Options = options;
    }

}

public static class DiaReader
{
    private static string currentFileText = "";
    private static List<string> allLines = new();
    private static List<int> allTabs = new();
    private static string currentSection = "";
    private static string currentDialogue = "";
    private static int currentTab = 1;
    private static int currentLine = 0;
    private static List<DiaOption> currentOptions = new();




    public static DiaPackage OpenNewDialogue(string filename)
    {
        SetCurrentDialogueFile(filename);
        FindNextSection();
        return Next(true, false, null);
    }




    public static DiaPackage ChooseOption(int optionIndex, out DiaActionType? actionFromChosenOption)
    {

        var chosenOption = currentOptions[optionIndex];
        currentLine = chosenOption.LineNumber;
        actionFromChosenOption = chosenOption.Action;

        BasicFunctions.Log($"✅ CHOSE: {chosenOption.Label} ACTIVATE: {actionFromChosenOption}", LogType.dia);



        return Next(false, false, chosenOption);

    }

    public static DiaPackage Next(bool firstNext, bool gotoDifferentSection, DiaOption lastOption)
    {
        //indent
        if (!firstNext && !gotoDifferentSection)
        {
            currentTab = lastOption.TabLevel + 1;
            currentLine = lastOption.LineNumber;
        }


        //find the next dialogue at indent
        FindNextDialogue();
        if (FindGotoNextSection(out string sectionToFind))
        {
            BasicFunctions.Log($"🗨: {currentDialogue}", LogType.dia);
            BasicFunctions.Log($"⭐: LOOKING FOR NEW SECTION: {sectionToFind}", LogType.dia);
            if (FindSpecificSection(sectionToFind))
            {
                Next(false, true, null);
                BasicFunctions.Log($"⭐: FOUND NEW SECTION: {sectionToFind}", LogType.dia);
            }

        }
        else
        {
            FindNextOptions(lastOption);


            BasicFunctions.Log($"======NEXT=====", LogType.dia);
            BasicFunctions.Log($"⛔: {currentSection}", LogType.dia);

            BasicFunctions.Log($"🗨: {currentDialogue}", LogType.dia);

            foreach (var option in currentOptions)
            {
                BasicFunctions.Log($"▶: {option.Label}   [{option.OptionType}]", LogType.dia);
            }
        }


        return new DiaPackage(currentDialogue, currentOptions);

    }

    public static void FindNextDialogue()
    {
        // Start from the currentLine index
        for (int i = currentLine; i < allLines.Count; i++)
        {
            string line = allLines[i];
            if (allTabs[i] == currentTab)
            {
                // Check if the line contains quotes
                int firstQuote = line.IndexOf('"');
                int secondQuote = line.IndexOf('"', firstQuote + 1);

                if (firstQuote != -1 && secondQuote != -1)
                {
                    // Extract the text between the quotes
                    currentDialogue = line.Substring(firstQuote + 1, secondQuote - firstQuote - 1);

                    // Update currentLine to the next line for future searches
                    currentLine = i + 1;
                    currentTab = allTabs[i];

                    return; // Exit the method once the dialogue is found
                }
            }

        }

        // If no dialogue is found, set currentDialogue to an empty string
        currentDialogue = string.Empty;
    }

    public static bool FindGotoNextSection(out string sectionToFind)
    {
        sectionToFind = "";
        // Start from the currentLine index
        for (int i = currentLine; i < allLines.Count; i++)
        {
            string line = allLines[i];

            if (allTabs[i] == currentTab)
            {




                // Check if the line contains quotes
                int firstQuote = line.IndexOf('<');
                int secondQuote = line.IndexOf('>', firstQuote + 1);

                if (firstQuote != -1 && secondQuote != -1)
                {
                    // Extract the text between the quotes
                    sectionToFind = line.Substring(firstQuote + 1, secondQuote - firstQuote - 1);


                    return true; // Exit the method once the dialogue is found
                }
            }
            else
                return false;

        }
        return false;
    }

    private static void FindNextOptions(DiaOption lastOption)
    {
        if (lastOption != null)
        {
            // Clear the current options list
            if (lastOption.OptionType == DiaOptionType.clear)
            {
                List<DiaOption> optionsToKeep = new List<DiaOption>();

                foreach (var option in currentOptions)
                {
                    if (option.OptionType == DiaOptionType.permanent)
                    {
                        optionsToKeep.Add(option);
                    }
                }

                currentOptions.Clear();
                currentOptions.AddRange(optionsToKeep);
            }

            if (lastOption.OptionType == DiaOptionType.subtract)
            {
                currentOptions.RemoveAt(lastOption.Index);
            }
        }

        // Iterate through all lines starting from the currentLine
        for (int i = currentLine; i < allLines.Count; i++)
        {
            string line = allLines[i].Trim();

            if (allTabs[i] < currentTab || line.StartsWith("=="))
            {
                i = allLines.Count;
            }
            else
            // Check if the line starts with '>', '-', or '+'
            if (allTabs[i] == currentTab)
            {
                if (line.StartsWith(">") || line.StartsWith("-") || line.StartsWith("+"))
                {
                    // Determine the OptionType based on the symbol at the beginning of the line
                    var optionType = line.StartsWith("-") ? DiaOptionType.subtract :
                                     line.StartsWith("+") ? DiaOptionType.permanent :
                                     line.StartsWith(">") ? DiaOptionType.clear :
                                     DiaOptionType.permanent;

                    // Extract the label (everything after the symbol)
                    string label = line.Substring(1).Trim();

                    // Check for an action in square brackets
                    DiaActionType actionType = DiaActionType.none;
                    string labelWithoutAction = label;
                    int actionStart = label.IndexOf('[');
                    int actionEnd = label.IndexOf(']');

                    if (actionStart >= 0 && actionEnd > actionStart)
                    {
                        string actionString = label.Substring(actionStart + 1, actionEnd - actionStart - 1).Trim();
                        actionType = GetActionFromString(actionString);
                        labelWithoutAction = label.Substring(0, actionStart).Trim();
                    }

                    // Create a new DiaOption object
                    DiaOption newOption = new(i, labelWithoutAction, optionType, actionType, currentOptions.Count, allTabs[i]);

                    // Add the new option to the current options list
                    currentOptions.Add(newOption);
                }
            }
        }

        // Sort the options so that all options with option type permanent are at the end
        currentOptions.Sort((a, b) =>
        {
            if (a.OptionType == DiaOptionType.permanent && b.OptionType != DiaOptionType.permanent)
            {
                return 1;
            }
            if (a.OptionType != DiaOptionType.permanent && b.OptionType == DiaOptionType.permanent)
            {
                return -1;
            }
            return 0;
        });
    }
    private static DiaActionType GetActionFromString(string theString)
    {
        if (string.IsNullOrWhiteSpace(theString))
            return DiaActionType.none;

        // Normalize spacing and case
        theString = theString.ToLowerInvariant().Replace(" ", "");

        if (theString == "menu")
            return DiaActionType.menu;

        if (theString.StartsWith("menu:") && theString.Contains("askabout"))
            return DiaActionType.menu_askAbout;

        if (theString.StartsWith("action:") && theString.Contains("hangout"))
            return DiaActionType.action_hangout;

        return DiaActionType.none;
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
                currentTab = allTabs[i] + 1;

                return;
            }
        }

        // No section found
        currentSection = string.Empty;
    }
    private static bool FindSpecificSection(string sectionToFind)
    {
        currentLine = 0;

        // Iterate through all lines starting from the current line
        for (int i = currentLine; i < allLines.Count; i++)
        {
            string line = allLines[i];

            // Check if the line contains "==" marking a section
            if (line.Contains($"=={sectionToFind}=="))
            {
                currentLine = i + 1;
                currentTab = allTabs[i];
                currentSection = sectionToFind;
                currentOptions.Clear();
                return true;
            }




        }

        // No section found
        currentSection = string.Empty;
        return false;
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
        for (int i = 0; i < allLines.Count; i++)
        {
            BasicFunctions.Log($"{i} - {allTabs[i]} tabs - {allLines[i]}", LogType.dia);
        }
    }
    public static void TurnFileIntoLines()
    {
        if (string.IsNullOrWhiteSpace(currentFileText))
        {
            return; // Do nothing if the input is null, empty, or whitespace
        }

        // Split the string by line breaks and add each line to the list
        allLines.AddRange(currentFileText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));

        // Ensure allTabs is initialized
        allTabs = new List<int>();

        // Count tabs in each line and add to allTabs
        foreach (string line in allLines)
        {
            int tabCount = line.Count(c => c == '\t');
            allTabs.Add(tabCount);
        }
    }


}
