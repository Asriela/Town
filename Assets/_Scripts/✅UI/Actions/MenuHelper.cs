using System;

using Mind;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.Arm;
using static UnityEngine.GraphicsBuffer;

public static class MenuHelper
{

    public static void SetupActionMenuElements(InteractionMenu myMenu, ref Label _tooltipText, ref VisualElement actionImage, ref VisualElement ActionInfoPanel, ref VisualElement actionsBackPanel, ref VisualElement menuContainer2, float portraitWidth)
    {
        actionsBackPanel = new VisualElement();
        actionsBackPanel.style.backgroundColor = MyColor.GreyBack;
        actionsBackPanel.style.position = Position.Absolute;
        actionsBackPanel.style.left = new Length(-78, LengthUnit.Pixel); // Left of the menu background
        actionsBackPanel.style.top = new Length((49 * 6) + 5 + 20 + 10, LengthUnit.Pixel);
        actionsBackPanel.style.width = new Length(portraitWidth + 50, LengthUnit.Pixel); // Scale factor for portrait
        actionsBackPanel.style.height = new Length(250 + 20, LengthUnit.Pixel); // Scale factor for portrait
        actionsBackPanel.style.opacity = 0.99f;
        actionsBackPanel.style.borderTopLeftRadius = 10;
        actionsBackPanel.style.borderTopRightRadius = 10;
        actionsBackPanel.style.borderBottomLeftRadius = 10;
        actionsBackPanel.style.borderBottomRightRadius = 10;
        menuContainer2.Add(actionsBackPanel);

        ActionInfoPanel = new VisualElement();
        ActionInfoPanel.style.backgroundColor = Color.white;
        ActionInfoPanel.style.position = Position.Absolute;
        ActionInfoPanel.style.left = new Length(-278 + 40, LengthUnit.Pixel); // Left of the menu background
        ActionInfoPanel.style.top = new Length((49 * 6) + 5 + 20 + 10, LengthUnit.Pixel);
        ActionInfoPanel.style.width = new Length(portraitWidth + 30, LengthUnit.Pixel); // Scale factor for portrait
        ActionInfoPanel.style.height = new Length(230, LengthUnit.Pixel); // Scale factor for portrait
        ActionInfoPanel.style.opacity = 0.99f;
        ActionInfoPanel.style.borderTopLeftRadius = 10;
        ActionInfoPanel.style.borderTopRightRadius = 10;
        ActionInfoPanel.style.borderBottomLeftRadius = 10;
        ActionInfoPanel.style.borderBottomRightRadius = 10;
        ActionInfoPanel.style.flexDirection = FlexDirection.Column;
        ActionInfoPanel.style.alignItems = Align.Center;
        ActionInfoPanel.style.justifyContent = Justify.Center;
        ActionInfoPanel.visible = false;
        menuContainer2.Add(ActionInfoPanel);

        var picWidth = (166 / 4) * 3;
        var picHeight = (147 / 4) * 3;
        actionImage = new VisualElement();
        actionImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/portraits/portraitBack"));
        actionImage.style.position = Position.Absolute;
        actionImage.style.left = new Length(16, LengthUnit.Pixel); // Left of the menu background
        actionImage.style.top = 10;
        actionImage.style.width = picWidth; // Scale factor for portrait
        actionImage.style.height = picHeight; // Scale factor for portrait
        actionImage.style.opacity = 1;
        actionImage.style.unityTextAlign = TextAnchor.MiddleCenter;
        ActionInfoPanel.Add(actionImage);

        _tooltipText = new Label
        {
            style =
                {
                    color = Color.black,
                    fontSize = 12,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    whiteSpace = WhiteSpace.Normal,
                    alignSelf = Align.Center,
                    flexGrow = 1,
                    width = 160,
                }
        };
        ActionInfoPanel.Add(_tooltipText);

        myMenu.charmMenuButton = new();
        myMenu.giveMenuButton = new();
        myMenu.coerceMenuButton = new();

        myMenu.TimeBarWidth = 300;
            var barHeight =30;
        var left =140;
        var top=610+20;
        myMenu.TimePanel = new VisualElement();
        myMenu.TimePanel.style.backgroundColor = MyColor.GreyBack;
        myMenu.TimePanel.style.position = Position.Absolute;
        myMenu.TimePanel.style.left = left-25-40+10-4;
        myMenu.TimePanel.style.top = top-50;

        myMenu.TimePanel.style.width = myMenu.TimeBarWidth+20+80-20-4;
        myMenu.TimePanel.style.height = barHeight+80;
        myMenu.TimePanel.style.flexDirection = FlexDirection.Column;
        myMenu.TimePanel.style.alignItems = Align.Center;
        myMenu.TimePanel.style.justifyContent = Justify.Center;
        myMenu.TimePanel.visible = true;
        myMenu.menuContainer.Add(myMenu.TimeBarBack);

        myMenu.TimeText = new Label("TIME LEFT UNTIL ARREST")
        {
            style =
                {
                    color = Color.white,
                    fontSize = 14,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    whiteSpace = WhiteSpace.Normal,
                    alignSelf = Align.Center,
                    flexGrow = 1,
                    width = 160,
                }
        };
        myMenu.TimeText.style.position = Position.Absolute;
        myMenu.TimeText.style.left = left + myMenu.TimeBarWidth/2 -80-40+5;
        myMenu.TimeText.style.flexDirection = FlexDirection.Column;
        myMenu.TimeText.style.alignItems = Align.Center;
        myMenu.TimeText.style.justifyContent = Justify.Center;
        myMenu.TimeText.style.width = 200;
        myMenu.TimeText.style.top = top - 40+10;
        myMenu.menuContainer.Add(myMenu.TimeText);

        myMenu.TimeBarBack = new VisualElement();
        myMenu.TimeBarBack.style.backgroundColor = Color.clear;
        myMenu.TimeBarBack.style.borderTopWidth = 2;
        myMenu.TimeBarBack.style.borderBottomWidth = 2;
        myMenu.TimeBarBack.style.borderLeftWidth = 2;
        myMenu.TimeBarBack.style.borderRightWidth = 2;
        myMenu.TimeBarBack.style.borderTopColor = Color.white;
        myMenu.TimeBarBack.style.borderBottomColor = Color.white;
        myMenu.TimeBarBack.style.borderLeftColor = Color.white;
        myMenu.TimeBarBack.style.borderRightColor = Color.white;
        myMenu.TimeBarBack.style.position = Position.Absolute;
        myMenu.TimeBarBack.style.left = left-20;
        myMenu.TimeBarBack.style.top = top;

        myMenu.TimeBarBack.style.width = myMenu.TimeBarWidth;
        myMenu.TimeBarBack.style.height = barHeight;
        myMenu.TimeBarBack.style.flexDirection = FlexDirection.Column;
        myMenu.TimeBarBack.style.alignItems = Align.Center;
        myMenu.TimeBarBack.style.justifyContent = Justify.Center;
        myMenu.TimeBarBack.visible = true;
        myMenu.menuContainer.Add(myMenu.TimeBarBack);

        myMenu.TimeBarNext = new VisualElement();
        myMenu.TimeBarNext.style.backgroundColor = MyColor.Red;
        myMenu.TimeBarNext.style.position = Position.Absolute;
        myMenu.TimeBarNext.style.left = left-20;
        myMenu.TimeBarNext.style.top = top;
        myMenu.TimeBarNext.style.width = 0;
        myMenu.TimeBarNext.style.height = barHeight;
        myMenu.TimeBarNext.style.flexDirection = FlexDirection.Column;
        myMenu.TimeBarNext.style.alignItems = Align.Center;
        myMenu.TimeBarNext.style.justifyContent = Justify.Center;
        myMenu.TimeBarNext.visible = true;
        myMenu.menuContainer.Add(myMenu.TimeBarNext);

        myMenu.TimeBar = new VisualElement();
        myMenu.TimeBar.style.backgroundColor = Color.white;
        myMenu.TimeBar.style.position = Position.Absolute;
        myMenu.TimeBar.style.left = left - 20;
        myMenu.TimeBar.style.top = top;
        myMenu.TimeBar.style.width = myMenu.TimeBarWidth;
        myMenu.TimeBar.style.height = barHeight;
        myMenu.TimeBar.style.flexDirection = FlexDirection.Column;
        myMenu.TimeBar.style.alignItems = Align.Center;
        myMenu.TimeBar.style.justifyContent = Justify.Center;
        myMenu.TimeBar.visible = true;
        myMenu.menuContainer.Add(myMenu.TimeBar);



         
    }
    public static void SetupWinLooseElements(InteractionMenu myMenu)
    {
        var width = 1000;
        var height = 600;
        myMenu.EndGameBack = new VisualElement();
        myMenu.EndGameBack.style.backgroundColor = MyColor.GreyBack;
        myMenu.EndGameBack.style.position = Position.Absolute;
        myMenu.EndGameBack.style.left = 100; // Left of the menu background
        myMenu.EndGameBack.style.top = 50;
        myMenu.EndGameBack.style.width = new Length(width, LengthUnit.Pixel); // Scale factor for portrait
        myMenu.EndGameBack.style.height = new Length(height, LengthUnit.Pixel); // Scale factor for portrait
        myMenu.EndGameBack.style.opacity = 0.96f;
        myMenu.EndGameBack.style.borderTopLeftRadius = 10;
        myMenu.EndGameBack.style.borderTopRightRadius = 10;
        myMenu.EndGameBack.style.borderBottomLeftRadius = 10;
        myMenu.EndGameBack.style.borderBottomRightRadius = 10;
        myMenu.EndGameBack.visible = false;
        myMenu.root.Add(myMenu.EndGameBack);

        myMenu.WinLooseLabel = new Label
        {
            style =
                {
                    color = Color.white,
                    fontSize = 80,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    whiteSpace = WhiteSpace.Normal,
                    alignSelf = Align.Center,
                    flexGrow = 1,
                    width = 1000,
                }
        };
        myMenu.WinLooseLabel.style.position = Position.Absolute;
        myMenu.WinLooseLabel.style.left = 400-300; // Left of the menu background
        myMenu.WinLooseLabel.style.top = 200;
        myMenu.WinLooseLabel.AddToClassList("button");
        myMenu.root.Add(myMenu.WinLooseLabel);

        myMenu.WinLooseDetailsLabel = new Label
        {
            style =
                {
                    color = Color.white,
                    fontSize = 15,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    whiteSpace = WhiteSpace.Normal,
                    alignSelf = Align.Center,
                    flexGrow = 1,
                    width = 1000,
                }
        };
        myMenu.WinLooseDetailsLabel.style.position = Position.Absolute;
        myMenu.WinLooseDetailsLabel.style.left = 400-300; // Left of the menu background
        myMenu.WinLooseDetailsLabel.style.top = 400;
        myMenu.WinLooseDetailsLabel.AddToClassList("button");
        myMenu.root.Add(myMenu.WinLooseDetailsLabel);
    }

    public static void WinLooseScreen(InteractionMenu myMenu, GameState endGameState)
    {

        myMenu.WinLooseDetailsLabel.visible=true;
        myMenu.WinLooseLabel.visible = true;
        myMenu.EndGameBack.visible = true;
        var statsText="";

        var dohlson=WorldManager.Instance.GetCharacter(CharacterName.Dohlson);
        var onar = WorldManager.Instance.GetCharacter(CharacterName.Onar);
        var onarImp= onar.Impression.GetSocialImpression();
        var dolImp= dohlson.Impression.GetSocialImpression();
        statsText += onarImp == SocialImpression.none ? $"<color={MyColor.GreyHex}>Onar didnt have any impression of you</color>\n" :  $"<color={MyColor.GreyHex}>Onar found you</color> {onarImp}\n";
        statsText += dolImp == SocialImpression.none ? $"<color={MyColor.GreyHex}>Dohlson didnt have any impression of you</color>\n" : $"<color={MyColor.GreyHex}>Dohlson found you</color> {dolImp}\n";


        if (endGameState == GameState.won)
        {
            myMenu.WinLooseLabel.text = "YOU WON";
            myMenu.WinLooseDetailsLabel.text = GameManager.Instance.EndingText+"\n\n"+statsText;
        }
        else
        {
            myMenu.WinLooseLabel.text = "YOU FAILED";
            myMenu.WinLooseDetailsLabel.text = GameManager.Instance.EndingText+"\n\n" + statsText;
        }

    }
    public static void ActionsMenu(InteractionMenu myMenu, SocialActionMenuType actionsMenuTypeWeAreIn, Character personWeAreSpeakingTo)
    {


        foreach (var item in myMenu.ActionButtonList)
        {
            item.Clear();
        }

        myMenu.menuContainer2.Add(myMenu.actionsBackPanel);
        myMenu.menuContainer2.Add(myMenu.ActionInfoPanel);
        myMenu.menuContainer2.Add(myMenu.TimePanel);

        myMenu.menuContainer2.Add(myMenu.TimeBarNext);
        myMenu.menuContainer2.Add(myMenu.TimeBar);
        myMenu.menuContainer2.Add(myMenu.TimeText);
        myMenu.menuContainer2.Add(myMenu.TimeBarBack);


        var availableActions = SocialActionsHelper.GetAvailableActions(actionsMenuTypeWeAreIn, personWeAreSpeakingTo);

        float topOffset = 120; // Starting vertical position

        var player = WorldManager.Instance.ThePlayer;
        var relationship = personWeAreSpeakingTo.Relationships.GetRelationshipWith(personWeAreSpeakingTo, player);

        var maxTime = WorldManager.Instance.MaxTime;
        var timeProgress = WorldManager.Instance.TimeProgress;


        myMenu.TimeBar.style.width = (myMenu.TimeBarWidth / maxTime) * timeProgress;

        foreach (var action in availableActions)
        {
            if (!personWeAreSpeakingTo.Impression.ActionsUsesLeftCount.ContainsKey(action.Enum))
            {
                personWeAreSpeakingTo.Impression.ActionsUsesLeftCount.Add(action.Enum, action.AmountOfUses);
            }

            if (action.UsedUp == true)
                continue;
            var actionButton = new Button(); // Red: C03F13 Green: 50AA7C
            var hasEnoughRelationship = true;
            var brokeDown = personWeAreSpeakingTo.Impression.BrokeDown;
            var lowestRelReq = (int)action.LowestRelationshipRequirement;
            var highestRelReq = (int)action.HighestRelationshipRequirement;
            if (actionsMenuTypeWeAreIn == SocialActionMenuType.coerce)
            {

                hasEnoughRelationship = lowestRelReq >= relationship || (lowestRelReq==0 && highestRelReq <= relationship);
                if (lowestRelReq == 0)
                    hasEnoughRelationship = true;
            }
            else
            {
                hasEnoughRelationship = lowestRelReq <= relationship;
            }





            var actionString = action.Name;

            if (!hasEnoughRelationship)
                actionString += $" [Rel: {lowestRelReq}]";
            // Add a label to the button
            var usesLeft= personWeAreSpeakingTo.Impression.ActionsUsesLeftCount[action.Enum];
            var usesLeftString="";
            if (usesLeft>0)
            {
                usesLeftString=usesLeft+" ";
            }
            var actionLabel = new Label(usesLeftString + actionString)
            {
                style =
            {
            color = hasEnoughRelationship && brokeDown==false ? Color.white : Color.grey,
            unityTextAlign = TextAnchor.MiddleLeft,
            fontSize = 12,
            whiteSpace = WhiteSpace.Normal,  // Allow text wrapping within the label
            overflow = Overflow.Visible,
            paddingBottom = new Length(0, LengthUnit.Pixel),  // Prevent cutting off text
            paddingTop = new Length(0, LengthUnit.Pixel),
            width = new Length(200, LengthUnit.Pixel),
            }
            };
            actionLabel.style.left=-5;
            actionButton.Add(actionLabel);
            actionButton.AddToClassList("statStyle");
            // Apply button styling to mimic other buttons
            actionButton.style.position = Position.Absolute; // Set position to absolute
            actionButton.style.width = new Length(130, LengthUnit.Pixel);
            actionButton.style.flexDirection = FlexDirection.Column;  // Makes new content push upward

            actionButton.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
            actionButton.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background

            actionButton.focusable = false;  // Prevent focus
            actionButton.style.left = -73;
            actionButton.style.top = 260 + topOffset; // Set the vertical position based on topOffset
            actionButton.style.unityFont = Resources.Load<Font>("Fonts/CALIBRIL");
            actionButton.style.paddingBottom = new Length(2, LengthUnit.Pixel);
            actionButton.style.paddingTop = new Length(2, LengthUnit.Pixel);
            actionButton.style.borderTopLeftRadius = 0;
            actionButton.style.borderTopRightRadius = 0;
            actionButton.style.borderBottomLeftRadius = 0;
            actionButton.style.borderBottomRightRadius = 0;
            // Increment topOffset for the next button
            var tooltip = action.Tooltip;
            var actionName = action.Name;
            var points = action.Points;
            var time= action.TimeLength;
            var actionTitle = actionName.ToLower().Replace(" ", "");
            actionButton.RegisterCallback<MouseEnterEvent>(e =>
            {
                var maxTime= WorldManager.Instance.MaxTime;
                var timeProgress = WorldManager.Instance.TimeProgress;
                var timeAddition = timeProgress+ time;

                myMenu.TimeBar.style.width= (myMenu.TimeBarWidth/ maxTime) * timeProgress;
                myMenu.TimeBarNext.style.width = (myMenu.TimeBarWidth / maxTime) * timeAddition;
                if (hasEnoughRelationship && brokeDown==false)
                {
                    actionButton.style.backgroundColor = new StyleColor(Color.white);
                    actionLabel.style.color = Color.black;
                    actionLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                    myMenu.TooltipText.style.unityFontStyleAndWeight = FontStyle.Normal;
                    myMenu.TooltipText.text = action.Tooltip;
                    myMenu.TooltipText.style.top = 55;
                    myMenu.TooltipText.style.color = Color.black;
                    myMenu.TooltipText.style.unityFont = Resources.Load<Font>("Fonts/CALIBRIB");
                    myMenu.ActionImage.visible = true;
                    myMenu.ActionImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>($"Sprites/actions/{actionTitle}"));
                    myMenu.ActionInfoPanel.style.backgroundColor = Color.white;
                    myMenu.ActionInfoPanel.style.height = new Length(220, LengthUnit.Pixel);
                }
                else
                {
                    myMenu.TooltipText.text = $"{lowestRelReq} RELATIONSHIP TO UNLOCK \nYOU HAVE {relationship}";
                    myMenu.TooltipText.style.color = MyColor.DarkGrey;
                    myMenu.TooltipText.style.unityFont = Resources.Load<Font>("Fonts/CALIBRIL");
                    myMenu.TooltipText.style.unityFontStyleAndWeight = FontStyle.Normal;
                    // myMenu.ActionImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>($"Sprites/actions/notUnlocked"));
                    myMenu.ActionImage.visible = false;
                    myMenu.ActionInfoPanel.style.backgroundColor = MyColor.GreyBack;
                    myMenu.TooltipText.style.top = 0;
                    myMenu.ActionInfoPanel.style.height = new Length(100, LengthUnit.Pixel);
                }
                myMenu.ActionInfoPanel.visible = true;



                var addTrust = points > 0 ? $" + {points}" : "";
                var addFear = points < 0 ? $" + {-points}" : "";
                var texttrust = $"{personWeAreSpeakingTo.Impression.TrustTowardsPlayer}{addTrust}";
                var textfear = $"{personWeAreSpeakingTo.Impression.FearTowardsPlayer}{addFear}";

                var relAddition = "";
                if (action.RelationshipImpact > 0)
                    relAddition = $" + {action.RelationshipImpact}";
                else if (action.RelationshipImpact < 0)
                    relAddition = $" - {-action.RelationshipImpact}";

                var relationshipText = $"{TextConverter.GetRelationshipStatusText(personWeAreSpeakingTo)}{relAddition}";

                var moodAddition = "";
                if (action.ActionEffects.Count != 0)
                {
                    moodAddition = $" => {action.ActionEffects[0]}";
                }

                var moodtext = $"{personWeAreSpeakingTo.State.VisualState[0]}{moodAddition}";
                var impressionText = personWeAreSpeakingTo.Impression.GetSocialImpressionText();
                myMenu.statLabel.text = TextConverter.GetStatString(texttrust, textfear, relationshipText, moodtext, impressionText, points > 0 ? MyColor.GreenHex : MyColor.WhiteHex, points < 0 ? MyColor.RedHex : MyColor.WhiteHex);
            });

            // Mouse leave event: hide outline
            actionButton.RegisterCallback<MouseLeaveEvent>(e =>
            {
                var maxTime = WorldManager.Instance.MaxTime;
                var timeProgress = WorldManager.Instance.TimeProgress;


                myMenu.TimeBar.style.width = (myMenu.TimeBarWidth / maxTime) * timeProgress;
                myMenu.TimeBarNext.style.width = 0;

                actionButton.style.backgroundColor = new StyleColor(Color.clear);
                actionLabel.style.color = hasEnoughRelationship && brokeDown ==false ? Color.white : Color.grey;
                actionLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
                myMenu.ActionInfoPanel.visible = false;

                var texttrust = $"{personWeAreSpeakingTo.Impression.TrustTowardsPlayer}";
                var textfear = $"{personWeAreSpeakingTo.Impression.FearTowardsPlayer}";


                myMenu.ActionImage.visible = false;
                var relationshipText = $"{TextConverter.GetRelationshipStatusText(personWeAreSpeakingTo)}";


                var moodtext = $"{personWeAreSpeakingTo.State.VisualState[0]}";
                var impressionText = personWeAreSpeakingTo.Impression.GetSocialImpressionText();
                myMenu.statLabel.text = TextConverter.GetStatString(texttrust, textfear, relationshipText, moodtext, impressionText, MyColor.WhiteHex, MyColor.WhiteHex);

            });

            // Increment topOffset for the next button
            topOffset += 30; // Adjust this value for more or less spacing between buttons

            actionButton.clicked += () =>
            {
               // if (hasEnoughRelationship && brokeDown==false)
                {
                    myMenu.DoingAction = action;
                    GameManager.Instance.UpdateInteractionMenu(personWeAreSpeakingTo, "");
                    player.RadialActionsHelper.PerformAction(personWeAreSpeakingTo, action, myMenu);
                }
                //GameManager.Instance.UIClicked = true;




                //StartCoroutine(CheckForInputAfterDelay());
            };
            myMenu.menuContainer2.Add(actionButton);
            myMenu.ActionButtonList.Add(actionButton);
        }



        myMenu.charmMenuButton.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/UI/menuCharm"));
        myMenu.charmMenuButton.style.position = Position.Absolute;
        myMenu.charmMenuButton.style.overflow = Overflow.Visible;
        myMenu.charmMenuButton.AddToClassList("statStyle");
        myMenu.charmMenuButton.style.left = -73;
        myMenu.charmMenuButton.style.top = 340;
        myMenu.charmMenuButton.style.width = 31; // Scale factor for portrait
        myMenu.charmMenuButton.style.height = 30; // Scale factor for portrait

        myMenu.charmMenuButton.style.opacity = 1;
        myMenu.charmMenuButton.style.unityTextAlign = TextAnchor.MiddleCenter;
        myMenu.charmMenuButton.style.borderTopLeftRadius = 0;
        myMenu.charmMenuButton.style.borderTopRightRadius = 0;
        myMenu.charmMenuButton.style.borderBottomLeftRadius = 0;
        myMenu.charmMenuButton.style.borderBottomRightRadius = 0;
        myMenu.charmMenuButton.style.color = myMenu.ActionsMenuTypeWeAreIn == SocialActionMenuType.charm ? MyColor.Green : Color.white;
        myMenu.charmMenuButton.clicked += () =>
        {
            myMenu.ActionsMenuTypeWeAreIn = SocialActionMenuType.charm;
            ActionsMenu(myMenu, myMenu.ActionsMenuTypeWeAreIn, personWeAreSpeakingTo);

        };
        myMenu.menuContainer2.Add(myMenu.giveMenuButton);
        myMenu.menuContainer2.Add(myMenu.charmMenuButton);
        myMenu.menuContainer.Add(myMenu.coerceMenuButton);

        myMenu.giveMenuButton.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/UI/menuGive"));
        myMenu.giveMenuButton.style.position = Position.Absolute;

        myMenu.giveMenuButton.style.overflow = Overflow.Visible;
        myMenu.giveMenuButton.AddToClassList("statStyle");
        myMenu.giveMenuButton.style.left = -73 + 40;
        myMenu.giveMenuButton.style.top = 340;
        myMenu.giveMenuButton.style.width = 31; // Scale factor for portrait
        myMenu.giveMenuButton.style.height = 30; // Scale factor for portrait
        myMenu.giveMenuButton.style.opacity = 1;
        myMenu.giveMenuButton.style.unityTextAlign = TextAnchor.MiddleCenter;
        myMenu.giveMenuButton.style.borderTopLeftRadius = 0;
        myMenu.giveMenuButton.style.borderTopRightRadius = 0;
        myMenu.giveMenuButton.style.borderBottomLeftRadius = 0;
        myMenu.giveMenuButton.style.borderBottomRightRadius = 0;
        myMenu.giveMenuButton.style.color = myMenu.ActionsMenuTypeWeAreIn == SocialActionMenuType.coerce ? MyColor.Red : Color.white;
        myMenu.giveMenuButton.clicked += () =>
        {
            myMenu.ActionsMenuTypeWeAreIn = SocialActionMenuType.give;
            ActionsMenu(myMenu, myMenu.ActionsMenuTypeWeAreIn, personWeAreSpeakingTo);

        };

        myMenu.coerceMenuButton.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("Sprites/UI/menuCoercion"));
        myMenu.coerceMenuButton.style.position = Position.Absolute;

        myMenu.coerceMenuButton.style.overflow = Overflow.Visible;
        myMenu.coerceMenuButton.AddToClassList("statStyle");
        myMenu.coerceMenuButton.style.left = -73 + 80;
        myMenu.coerceMenuButton.style.top = 340;
        myMenu.coerceMenuButton.style.width = 31; // Scale factor for portrait
        myMenu.coerceMenuButton.style.height = 30; // Scale factor for portrait
        myMenu.coerceMenuButton.style.opacity = 1;
        myMenu.coerceMenuButton.style.unityTextAlign = TextAnchor.MiddleCenter;
        myMenu.coerceMenuButton.style.borderTopLeftRadius = 0;
        myMenu.coerceMenuButton.style.borderTopRightRadius = 0;
        myMenu.coerceMenuButton.style.borderBottomLeftRadius = 0;
        myMenu.coerceMenuButton.style.borderBottomRightRadius = 0;
        myMenu.coerceMenuButton.style.color = myMenu.ActionsMenuTypeWeAreIn == SocialActionMenuType.coerce ? MyColor.Red : Color.white;
        myMenu.coerceMenuButton.clicked += () =>
        {
            myMenu.ActionsMenuTypeWeAreIn = SocialActionMenuType.coerce;
            ActionsMenu(myMenu, myMenu.ActionsMenuTypeWeAreIn, personWeAreSpeakingTo);

        };








    }






















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

    public static void StatText(Character personWeAreSpeakingTo, VisualElement menuContainer2, ref Button statText, ref Label statLabel, string greenOn, string redOn, int trust, int fear, string relationship, MemoryTags mood, string impression)
    {
        statText = new Button();//red: C03F13 green 50AA7C


        var statsString = TextConverter.GetStatString(trust.ToString(), fear.ToString(), relationship, mood.ToString(), impression, greenOn, redOn);
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

    public static void Dialogue(ref string pastDialogue, string lastChosenOption, string currentDialogue, string currentSpeaker, int standardFontSize, float scrolldown, Button dialogue, VisualElement menuContainer)
    {

        string strippedPastDialogue = pastDialogue;

        if (lastChosenOption != "")
        { strippedPastDialogue = MyColor.StripColorTags(pastDialogue); }

        pastDialogue = $"<color=#7F807A>" +
                  $"{strippedPastDialogue}";

        pastDialogue = MyColor.WrapTextInYellowTag(pastDialogue);
        pastDialogue = MyColor.WrapTextInPurpleTag(pastDialogue);
        pastDialogue = MyColor.WrapTextInAquaTag(pastDialogue);
        
        // Conditionally add "YOU-{lastChosenOption}" if it's not an empty string
        if (lastChosenOption != "")
        {
            pastDialogue += @$"<color=#B3B4BC>YOU-""{lastChosenOption}""</color>" + "\n\n";
        }

        // Add the speaker and current dialogue in white
        if (currentDialogue != "")
        {
            pastDialogue += @$"<color=#FFFFFF>{currentSpeaker.ToUpper()}</color><color=#D5D6C8>- " +
        
                "</color>\n\n";


        }
        //         Regex.Replace(currentDialogue, @"^#(.*?)#(.*)", @"  <color=#a0a095>$1</color>" + "\n" + @"""$2""")

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

    public static void ActionText(ActionOption DoingAction, Character personWeAreSpeakingTo, VisualElement menuContainer, float scrolldown)
    {

        string socialActionText = $"<color=#7F807A>{TextConverter.ChangeSocialInteractionToText(DoingAction.Enum, personWeAreSpeakingTo.CharacterName.ToString())}</color>";
        // Create the button and add the label
        var actionImage = new VisualElement();
        actionImage = new VisualElement();
        var actionName = DoingAction.Name;
        var actionTitle = actionName.ToLower().Replace(" ", "");
        actionImage.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>($"Sprites/actions/{actionTitle}"));
        actionImage.style.position = Position.Absolute;

        actionImage.style.width = 166; // Scale factor for portrait
        actionImage.style.height = 147; // Scale factor for portrait
        actionImage.style.opacity = 1;
        actionImage.style.unityTextAlign = TextAnchor.MiddleCenter;
        actionImage.style.left = 180;
        actionImage.style.top = 390;


        menuContainer.Add(actionImage);
        var socialAction = new Button();

        // Add a label to the button
        Label socialActionLabel = new Label(socialActionText)
        {
            style =
            {
                color = Color.white,  // Default color, will be overridden by rich text
                unityTextAlign = TextAnchor.MiddleCenter,
                alignSelf =Align.Center,
                justifyContent =Justify.Center,
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
        socialAction.style.top = new Length(120 + scrolldown, LengthUnit.Pixel); // Set initial top position
        socialAction.style.width = new Length(340, LengthUnit.Pixel);
        socialAction.style.flexDirection = FlexDirection.ColumnReverse;  // Makes new content push upward

        socialAction.style.overflow = Overflow.Hidden;  // Prevents content from pushing layout

        socialAction.style.alignItems = Align.FlexStart;  // Ensure text starts from the top
        socialAction.style.backgroundColor = new StyleColor(Color.clear);  // Remove button background
        socialAction.focusable = false;  // Prevent focus
        socialAction.style.left = 90;
        socialAction.style.top = 320;





        socialAction.style.marginTop = 160 + 130;
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


    }


}
