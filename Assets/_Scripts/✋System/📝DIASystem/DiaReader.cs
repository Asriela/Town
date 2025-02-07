using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Mind;

public enum DiaActionType : short
{
    none,
    menu,
    menu_askAbout,
    action_hangout,
    action_rentRoom,
    scriptedAction,
    share
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

    public int Index { get; set; }

    public int TabLevel { get; }
    public DiaActionType Action { get; }

    public object ActionData { get; }

    public DiaOptionType OptionType { get; }

    public DiaOption(int lineNumber, string label, DiaOptionType optionType, DiaActionType action, object actionData, int index, int tabLevel)
    {
        LineNumber = lineNumber;
        Label = label;
        Action = action;
        OptionType = optionType;
        Index = index;
        TabLevel = tabLevel;
        ActionData = actionData;
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
    private static bool skipNextLineDueToBadCondition = false;



    public static DiaPackage OpenNewDialogue(string filename)
    {
        SetCurrentDialogueFile(filename);
        ClearAllData();
        FindNextSection();
        var ret = Next(true, false, null, 0);
        return ret;
    }


    private static void ClearAllData()
    {
        currentSection = "";
        currentDialogue = "";
        currentTab = 1;
        currentLine = 0;
        currentOptions.Clear();
    }


    public static DiaPackage ChooseOption(int optionIndex, out DiaActionType? actionFromChosenOption, out object actionDataFromChosenOption)
    {

        var chosenOption = currentOptions[optionIndex];
        currentLine = chosenOption.LineNumber;
        actionFromChosenOption = chosenOption.Action;
        actionDataFromChosenOption = chosenOption.ActionData;

        BasicFunctions.Log($"✅ CHOSE: {chosenOption.Label} ACTIVATE: {actionFromChosenOption} data: {actionDataFromChosenOption}", LogType.dia);



        return Next(false, false, chosenOption, 0);

    }

    public static DiaPackage Next(bool firstNext, bool gotoDifferentSection, DiaOption lastOption, int differentSectionLine)
    {
        //indent
        if (!(firstNext || gotoDifferentSection))
        {
            currentTab = lastOption.TabLevel + 1;
            currentLine = lastOption.LineNumber + 1;
        }
        if (gotoDifferentSection)
        {
            currentTab = 1;
            currentLine = differentSectionLine;
        }

        //find the next dialogue at indent
        if (!FindNextDialogue(out bool noOptions))
        {
            //currentLine++;
            if (gotoDifferentSection)
            { currentLine = differentSectionLine; }
        }
        if (FindGotoNextSection(out string sectionToFind))
        {
            if (sectionToFind == "end" || sectionToFind == "exit")
            {
                BasicFunctions.Log($"🚪: exit menu", LogType.dia);
                GameManager.Instance.CloseInteractionMenu();
                return null;
            }
            BasicFunctions.Log($"🗨X: {currentDialogue}", LogType.dia);
            BasicFunctions.Log($"⭐: LOOKING FOR NEW SECTION: {sectionToFind}", LogType.dia);
            if (FindSpecificSection(sectionToFind, out int sectionLine))
            {
                Next(false, true, null, sectionLine);
                BasicFunctions.Log($"⭐: FOUND NEW SECTION: {sectionToFind}", LogType.dia);
            }

        }
        else
        {
            BasicFunctions.Log($"======NEXT=====", LogType.dia);
            BasicFunctions.Log($"⛔: {currentSection}", LogType.dia);

            BasicFunctions.Log($"🗨: {currentDialogue}", LogType.dia);
            if (noOptions == false)
            { FindNextOptions(lastOption); }




            foreach (var option in currentOptions)
            {
                BasicFunctions.Log($"▶: {option.Label}   [{option.OptionType}]", LogType.dia);
            }
        }


        return new DiaPackage(currentDialogue, currentOptions);

    }
    private static void CheckForCondition(String line)
    {
        if (line.Contains("(") && line.Contains(")"))
        {
            int start = line.IndexOf('(');
            int end = line.IndexOf(')');
            string condition = line.Substring(start + 1, end - start - 1);
            BasicFunctions.Log($"✋: {condition}", LogType.dia);
            condition = condition.ToLowerInvariant().Replace(" ", "");

            string dataString = string.Empty;

            int colonIndex = condition.IndexOf(':');
            if (colonIndex != -1 && colonIndex < condition.Length - 1)
            {
                dataString = condition.Substring(colonIndex + 1);
            }
            var personWeAreSpeakingTo = GameManager.Instance.GetPersonWeAreSpeakingTo();
            var player = WorldManager.Instance.ThePlayer;


            if (condition.StartsWith("know:"))
            {
                if (Enum.TryParse(typeof(MemoryTags), dataString, true, out object rawData))
                {
                    MemoryTags enumData = (MemoryTags)rawData;
                    if (personWeAreSpeakingTo.PersonKnowledge.HasKnowledge(personWeAreSpeakingTo, player, enumData) == false)
                    {
                        skipNextLineDueToBadCondition = true;
                    }
                }
                else
                { BasicFunctions.Log($"⚠️ Invalid MemoryTag: {dataString}", LogType.dia); }

            }
            if (condition.StartsWith("completed:"))
            {
                if (Enum.TryParse(typeof(ScriptedTaskType), dataString, true, out object rawData))
                {
                    ScriptedTaskType enumData = (ScriptedTaskType)rawData;
                    if (personWeAreSpeakingTo.Memory.GetScriptedTaskProgress(enumData) != ScriptedTaskProgressType.completed)
                    {
                        skipNextLineDueToBadCondition = true;
                    }
                }
                else
                { BasicFunctions.Log($"⚠️ Invalid MemoryTag: {dataString}", LogType.dia); }

            }
            if (condition.StartsWith("notcompleted:"))
            {
                if (Enum.TryParse(typeof(ScriptedTaskType), dataString, true, out object rawData))
                {
                    ScriptedTaskType enumData = (ScriptedTaskType)rawData;
                    if (personWeAreSpeakingTo.Memory.GetScriptedTaskProgress(enumData) == ScriptedTaskProgressType.completed)
                    {
                        skipNextLineDueToBadCondition = true;
                    }
                }
                else
                { BasicFunctions.Log($"⚠️ Invalid MemoryTag: {dataString}", LogType.dia); }

            }
            if (condition.StartsWith("saw:"))
            {
                if (Enum.TryParse(typeof(CharacterName ), dataString, true, out object rawData))
                {
                    CharacterName enumData = (CharacterName)rawData;
                    var characterObject = WorldManager.Instance.GetCharacter(enumData);
                    var visualStatus= player.VisualStatusKnowledge.GetVisualStatus(player, characterObject);

                    if (visualStatus == null || visualStatus.Count!=0)
                    {
                        skipNextLineDueToBadCondition = true;
                    }
                }
                else
                { BasicFunctions.Log($"⚠️ Invalid MemoryTag: {dataString}", LogType.dia); }

            }
        }
    }
    public static bool FindNextDialogue(out bool noOptions)
    {
        noOptions = false;
        // Start from the currentLine index
        for (int i = currentLine; i < allLines.Count; i++)
        {

            string line = allLines[i];
            BasicFunctions.Log($"🔎📖: {line} {allTabs[i]} vs {currentTab} ", LogType.dia);
            if ((allTabs[i] < currentTab ) || (line.Contains("<") && allTabs[i] == currentTab) )
            { currentDialogue="";return false; }

            if (allTabs[i] <= currentTab && (line.StartsWith('-') || line.StartsWith('+') || line.StartsWith('>')))
            { return false; }
            if (allTabs[i] == currentTab)
            {
                CheckForCondition(line);


                // Check if the line contains quotes
                int firstQuote = line.IndexOf('"');
                int secondQuote = line.IndexOf('"', firstQuote + 1);

                if (firstQuote != -1 && secondQuote != -1)
                {
                    if (skipNextLineDueToBadCondition == false)
                    {
                        // Extract the text between the quotes
                        currentDialogue = line.Substring(firstQuote + 1, secondQuote - firstQuote - 1);

                        // Update currentLine to the next line for future searches
                        currentLine = i + 1;
                        BasicFunctions.Log($"😄📖: CHOSE! {line} {allTabs[i]} vs {currentTab} ", LogType.dia);
                        currentTab = allTabs[i];

                        line = allLines[currentLine];
                        firstQuote = line.IndexOf('"');
                        secondQuote = line.IndexOf('"', firstQuote + 1);
                        if (firstQuote != -1 && secondQuote != -1)
                        { noOptions = true; }
                        return true; // Exit the method once the dialogue is found
                    }
                    else
                    {
                        BasicFunctions.Log($"🔎📖🛑: SKIPPING!! {line} ", LogType.dia);
                        skipNextLineDueToBadCondition = false;
                    }
                }
            }

        }
        return false;
        // If no dialogue is found, set currentDialogue to an empty string

    }

    public static bool FindGotoNextSection(out string sectionToFind)
    {
        sectionToFind = "";
        // Start from the currentLine index
        for (int i = currentLine; i < allLines.Count; i++)
        {
            string line = allLines[i];
             BasicFunctions.Log($"🔍: {line} with tab {allTabs[i]} vs currentTab {currentTab}", LogType.dia);
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



            if (allTabs[i] < currentTab || line.StartsWith("==") || (allTabs[i]<= currentTab && line.StartsWith('"')))
            {
                i = allLines.Count;
            }
            else
            // Check if the line starts with '>', '-', or '+'
            if (allTabs[i] == currentTab)
            {
                BasicFunctions.Log($"🔎📦: {line} {allTabs[i]} vs {currentTab} ", LogType.dia);
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

                    object actionData = null;
                    if (actionStart >= 0 && actionEnd > actionStart)
                    {
                        string actionString = label.Substring(actionStart + 1, actionEnd - actionStart - 1).Trim();
                        actionType = GetActionFromString(actionString, out actionData);
                        labelWithoutAction = label.Substring(0, actionStart).Trim();
                    }


                    // Create a new DiaOption object
                    DiaOption newOption = new(i, labelWithoutAction, optionType, actionType, actionData, currentOptions.Count, allTabs[i]);

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

        for (int i = 0; i < currentOptions.Count; i++)
        {
            currentOptions[i].Index = i;  // Set the new index position
        }
    }
    private static DiaActionType GetActionFromString(string theString, out object actionData)
    {
        var text = "";
        DiaActionType ret = DiaActionType.none;
        actionData = ScriptedTaskType.none;



        if (string.IsNullOrWhiteSpace(theString))
            ret = DiaActionType.none;
        var dataString=string.Empty;

        int colonIndex = theString.IndexOf(':');
        if (colonIndex != -1 && colonIndex < theString.Length - 1)
        {
            dataString = theString.Substring(colonIndex + 1);
        }

        // Normalize spacing and case
        theString = theString.ToLowerInvariant().Replace(" ", "");

        if (theString == "menu")
            ret = DiaActionType.menu;

        if (theString.StartsWith("menu:") && theString.Contains("askabout"))
            ret = DiaActionType.menu_askAbout;

        if (theString.StartsWith("action:") && theString.Contains("hangout"))
            ret = DiaActionType.action_hangout;

        if (theString.StartsWith("action:") && theString.Contains("rentroom"))
            ret = DiaActionType.action_rentRoom;

        if (theString.StartsWith("scriptedaction:"))
        {
            ret = DiaActionType.scriptedAction;
            actionData = ScriptedTaskType.takePlayerToInn;
        }

        if (theString.StartsWith("share:"))
        {
            ret = DiaActionType.share;
            if (Enum.TryParse(typeof(MemoryTags), dataString, true, out object rawData))
            {
                actionData = (MemoryTags)rawData;
            }

        }
        return ret;
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
    private static bool FindSpecificSection(string sectionToFind, out int sectionLine)
    {
        currentLine = 0;
        sectionLine = 0;
        // Iterate through all lines starting from the current line
        for (int i = currentLine; i < allLines.Count; i++)
        {
            string line = allLines[i];

            // Check if the line contains "==" marking a section
            if (line.Contains($"=={sectionToFind}=="))
            {
                sectionLine = i + 1;
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
        if (allLines != null)
        { allLines.Clear(); }
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
