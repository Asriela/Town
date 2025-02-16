using UnityEngine;

public static class MyColor
{
    public static readonly Color Red = new Color(0.768f, 0.251f, 0.075f);//C03F13
    public static readonly Color Green = new Color(0.647f, 0.776f, 0.310f);//new Color(0.53f, 0.65f, 0.37f);//new Color(0.3137f, 0.6667f, 0.4863f);
    public static readonly Color Grey = new Color(0.478f, 0.494f, 0.518f);
    public static readonly Color GreenBack = new Color(0.412f, 0.478f, 0.224f);
    public static readonly Color GreyBack = new Color(0.1137f, 0.0941f, 0.1098f);
    //new Color(0.33f, 0.41f, 0.23f);
    public static readonly Color RedBack = new Color(0.57f, 0.15f, 0.07f);
    public static readonly Color PurpleBack = new Color(70f / 255f, 69f / 255f, 104f / 255f);
    public static readonly Color Purple = new Color(0.419f, 0.318f, 0.424f);
    public static readonly string PurpleHex = "#6B516C";
    public static readonly string RedHex = "#D7431F";
    public static readonly string GreenHex = "#A5C64F";
    public static readonly string WhiteHex = "#FFFFFF";
    public static readonly string YellowHex = "#E4B83A";
    public static readonly string CyanHex = "#6CADB5";
    public static readonly string GreyHex = "#A0A0A0";
    public static readonly string DarkGreyHex = "#6B6B6B";
    
    public static readonly Color Cyan = new  Color(0.424f, 0.678f, 0.710f);
    public static readonly Color CyanBack = new Color(0.286f, 0.459f, 0.478f);


    public static string WrapTextInPurpleTag(string pastDialogue)
    {
        // Define the purple color tag
        string purpleTagStart = $"<color={YellowHex}>"; // Use your specific MyColor.PurpleHex value here
        string purpleTagEnd = "</color>";
        var beforetext = pastDialogue;
        // Regex to find text wrapped in '*' and replace it with purple color tag
        var ret= System.Text.RegularExpressions.Regex.Replace(pastDialogue, @"\*(.*?)\*", match =>
        {
            // Wrap the matched text with the purple color tag
            return purpleTagStart + match.Groups[1].Value + purpleTagEnd;
        });
        return ret;
    }


    public static string StripColorTags(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "<color[^>]*?>|</color>", string.Empty);


    }
}
