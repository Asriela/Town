using System.Text.RegularExpressions;
using Mind;
using UnityEngine;
using UnityEngine.UIElements;

public static class MenuHelper
{
    public static void Portrait(Character personWeAreSpeakingTo, VisualElement menuContainer2, VisualElement sideBarBack, VisualElement portraitImage)
    {
        if (personWeAreSpeakingTo != null)
        {
            // Add the portrait images
            menuContainer2.Add(sideBarBack);
            menuContainer2.Add(portraitImage);

            // Update the portrait image with the character's name
            portraitImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>($"Sprites/portraits/portrait{personWeAreSpeakingTo.CharacterName}"));

            // Create a separate container for the name button (this will not affect the menu options layout)
            VisualElement nameButtonContainer = new VisualElement();
            nameButtonContainer.style.position = Position.Absolute;
            nameButtonContainer.style.top = new Length(110, LengthUnit.Pixel); // Position below the portrait
            nameButtonContainer.style.left = new Length(-70, LengthUnit.Pixel); // Align to the left margin
            nameButtonContainer.style.width = new Length(180, LengthUnit.Pixel);

            // Add a button below the portrait image with the character's name
            Button nameButton = new Button();
            Label nameLabel = new Label(personWeAreSpeakingTo.CharacterName.ToString())
            {
                style =
                {
                    color = Color.white,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = 16,
                }
            };
            nameButton.Add(nameLabel);

            nameButton.style.marginTop = new Length(10, LengthUnit.Pixel); // Adjust this margin if necessary
            nameButton.style.backgroundColor = new StyleColor(Color.clear);  // Transparent background
            nameButton.focusable = false;  // Prevent focus

            nameButton.AddToClassList("button");

            // Override click event to do nothing
            nameButton.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

            // Add the name button to the container
            nameButtonContainer.Add(nameButton);

            // Add the container for the name button to the menu (it won't affect the other button layout)
            //  menuContainer2.Add(nameButtonContainer);
        }
    }

    public static void StatText(Character personWeAreSpeakingTo, VisualElement menuContainer2,ref Button statText,ref Label statLabel, string greenOn, string redOn, int trust, int fear, string relationship, MemoryTags mood, string impression)
    {
        statText = new Button();//red: C03F13 green 50AA7C


        var statsString = TextConverter.GetStatString(trust, fear, relationship, mood, impression, greenOn, redOn);
        // Add a label to the button
        statLabel = new Label(statsString)
        {
            style =
            {

                unityTextAlign = TextAnchor.MiddleLeft,
                fontSize = 12,
                whiteSpace = WhiteSpace.Normal,  // Allow text wrapping within the label
                overflow = Overflow.Hidden,
                paddingBottom = new Length(5, LengthUnit.Pixel),  // Prevent cutting off text
                paddingTop = new Length(20, LengthUnit.Pixel),
            }
        };
        statText.Add(statLabel);

        // Apply button styling to mimic other buttons
        statText.style.position = Position.Absolute; // Set position to absolute
        statText.style.width = new Length(340, LengthUnit.Pixel);
        statText.style.flexDirection = FlexDirection.Column;  // Makes new content push upward


        statText.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
        statText.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
        statText.focusable = false;  // Prevent focus
        statText.style.left = -80;
        statText.style.top = 120;





        statText.style.marginTop = 0;
        statText.style.paddingTop = new Length(5, LengthUnit.Pixel);
        statText.AddToClassList("statStyle");

        // Override click event to do nothing
        statText.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        menuContainer2.Add(statText);
    }

    public static void Dialogue(ref string pastDialogue, string lastChosenOption, string currentDialogue,string currentSpeaker, int standardFontSize, float scrolldown, Button dialogue, VisualElement menuContainer)
    {

        string strippedPastDialogue = pastDialogue;

        if (lastChosenOption != "")
        { strippedPastDialogue = MyColor.StripColorTags(pastDialogue); }

        pastDialogue = $"<color=#7F807A>" +
                  $"{strippedPastDialogue}";

        pastDialogue = MyColor.WrapTextInPurpleTag(pastDialogue);

        // Conditionally add "YOU-{lastChosenOption}" if it's not an empty string
        if (lastChosenOption != "")
        {
            pastDialogue += @$"<color=#B3B4BC>YOU-""{lastChosenOption}""</color>" + "\n\n";
        }

        // Add the speaker and current dialogue in white
        if (currentDialogue != "")
        {
            pastDialogue += @$"<color=#FFFFFF>{currentSpeaker.ToUpper()}</color><color=#D5D6C8>- " +
                Regex.Replace(currentDialogue, @"^#(.*?)#(.*)", @"  <color=#a0a095>$1</color>" + "\n" + @"""$2""") +
                "</color>\n\n";


        }


        // Create the button and add the label
        dialogue = new Button();
        var removestars = System.Text.RegularExpressions.Regex.Replace(pastDialogue, @"\*", string.Empty);

        // Add a label to the button
        Label dialogueLabel = new Label(removestars)
        {
            style =
            {
                color = Color.white,  // Default color, will be overridden by rich text
                unityTextAlign = TextAnchor.MiddleLeft,
                fontSize = standardFontSize,
                whiteSpace = WhiteSpace.Normal,  // Allow text wrapping within the label
                overflow = Overflow.Hidden,
                paddingBottom = new Length(5, LengthUnit.Pixel),  // Prevent cutting off text
                paddingTop = new Length(20, LengthUnit.Pixel),
            }
        };
        dialogue.Add(dialogueLabel);

        // Apply button styling to mimic other buttons
        dialogue.style.position = Position.Absolute; // Set position to absolute
        dialogue.style.left = new Length(130 + 120 + 60 - 19 - 9, LengthUnit.Pixel);
        dialogue.style.top = new Length(40 + scrolldown, LengthUnit.Pixel); // Set initial top position
        dialogue.style.width = new Length(340, LengthUnit.Pixel);
        dialogue.style.flexDirection = FlexDirection.ColumnReverse;  // Makes new content push upward

        dialogue.style.overflow = Overflow.Hidden;  // Prevents content from pushing layout

        dialogue.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
        dialogue.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
        dialogue.focusable = false;  // Prevent focus
        dialogue.style.left = 80;
        dialogue.style.top = 220;





        dialogue.style.marginTop = 0;
        dialogue.style.paddingTop = new Length(5, LengthUnit.Pixel);
        dialogue.AddToClassList("button");

        // Override click event to do nothing
        dialogue.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

        menuContainer.Add(dialogue);

        // Adjust the top position dynamically based on content height
        dialogue.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            dialogue.style.top = new Length(90 + scrolldown - dialogue.resolvedStyle.height, LengthUnit.Pixel);
        });

    }

    public static void ActionText(Character personWeAreSpeakingTo, VisualElement menuContainer, float scrolldown)
    {
        var currentPlayerSocialAction = GameManager.Instance.GetPlayersCurrentSocialAction();
        if (currentPlayerSocialAction != SocializeType.none)
        {
            string socialActionText = $"<color=#7F807A>INTERACTION PAUSED BECAUSE YOU ARE BUSY \n{TextConverter.ChangeSocialInteractionToText(currentPlayerSocialAction, personWeAreSpeakingTo.CharacterName.ToString())}</color>";
            // Create the button and add the label
            var socialAction = new Button();

            // Add a label to the button
            Label socialActionLabel = new Label(socialActionText)
            {
                style =
            {
                color = Color.white,  // Default color, will be overridden by rich text
                unityTextAlign = TextAnchor.MiddleCenter,
                fontSize = 17,
                whiteSpace = WhiteSpace.Normal,  // Allow text wrapping within the label
                overflow = Overflow.Hidden,
                paddingBottom = new Length(5, LengthUnit.Pixel),  // Prevent cutting off text
                paddingTop = new Length(20, LengthUnit.Pixel),
            }
            };
            socialAction.Add(socialActionLabel);

            // Apply button styling to mimic other buttons
            socialAction.style.position = Position.Absolute; // Set position to absolute
            socialAction.style.left = new Length(130 + 120 + 60 - 19 - 9, LengthUnit.Pixel);
            socialAction.style.top = new Length(90 + scrolldown, LengthUnit.Pixel); // Set initial top position
            socialAction.style.width = new Length(340, LengthUnit.Pixel);
            socialAction.style.flexDirection = FlexDirection.ColumnReverse;  // Makes new content push upward

            socialAction.style.overflow = Overflow.Hidden;  // Prevents content from pushing layout

            socialAction.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
            socialAction.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
            socialAction.focusable = false;  // Prevent focus
            socialAction.style.left = 80;
            socialAction.style.top = 220;





            socialAction.style.marginTop = 160 + 40;
            socialAction.style.paddingTop = new Length(5, LengthUnit.Pixel);
            socialAction.AddToClassList("button");

            // Override click event to do nothing
            socialAction.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

            menuContainer.Add(socialAction);

            // Adjust the top position dynamically based on content height
            socialAction.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                socialAction.style.top = new Length(90 + scrolldown - socialAction.resolvedStyle.height, LengthUnit.Pixel);
            });
            return;
        }
    }


}
