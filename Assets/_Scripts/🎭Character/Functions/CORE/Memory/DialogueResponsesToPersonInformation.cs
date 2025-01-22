using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mind;

// Class to represent a memory tag and the corresponding dialogue response based on relationship state
[Serializable]
public class MemoryTagsDialogueViewPair
{
    public Mind.MemoryTags memoryTags;  // Memory tag
    public List<DialogueViewPair> dialogueResponses = new();  // List of dialogue responses based on relationship state
}

[Serializable]
public class DialogueViewPair
{
    public ViewTowards relationshipState;           // ViewTowards relationship state
    public string dialogue;                         // Dialogue response for the given relationship state
    public List<CharacterMemoryTagPair> tagsInDialogue = new();  // List of character and tag pairs indicating whose tag it is
    public float relImpactOnMessenger;              // Relationship impact on the messenger (a float value)
}

// Pair to link character with a memory tag
[Serializable]
public class CharacterMemoryTagPair
{
    public Character knower;  // Character that knows about the memory tag
    public Character tagAboutWho;  // Character this tag is about
    public Mind.MemoryTags tag;  // The related memory tag
}
[Serializable]
public class CharacterMemoryDialoguePair
{
    public Character character;                         // The character
    public List<MemoryTagsDialogueViewPair> dialogues = new();  // List of memory tag-dialogue-view pairs
}

public class DialogueResponsesToPersonInformation : MonoBehaviour
{
    [SerializeField]
    private List<CharacterMemoryDialoguePair> _peopleDialogues = new(); // Nested structure

    // Expose dialogues as a dictionary of Character to List of MemoryTagsDialogueViewPair
    public Dictionary<Character, List<MemoryTagsDialogueViewPair>> PeopleDialogues
    {
        get
        {
            Dictionary<Character, List<MemoryTagsDialogueViewPair>> dictionary = new();
            foreach (var pair in _peopleDialogues)
            {
                dictionary[pair.character] = pair.dialogues;
            }
            return dictionary;
        }
    }

    // Add a character's dialogue response to memory tags based on relationship state
    public void AddPersonDialogue(Character person, List<Mind.MemoryTags> tags, ViewTowards relationshipState, string dialogue, List<CharacterMemoryTagPair> tagsInDialogue, float relImpactOnMessenger)
    {
        var existingPerson = _peopleDialogues.FirstOrDefault(p => p.character == person);

        if (existingPerson == null)
        {
            _peopleDialogues.Add(new CharacterMemoryDialoguePair
            {
                character = person,
                dialogues = tags.Select(tag => new MemoryTagsDialogueViewPair
                {
                    memoryTags = tag,
                    dialogueResponses = new List<DialogueViewPair>
                {
                    new DialogueViewPair
                    {
                        relationshipState = relationshipState,
                        dialogue = dialogue,
                        tagsInDialogue = tagsInDialogue,
                        relImpactOnMessenger = relImpactOnMessenger
                    }
                }
                }).ToList()
            });
        }
        else
        {
            foreach (var tag in tags)
            {
                var existingTag = existingPerson.dialogues.FirstOrDefault(d => d.memoryTags == tag);
                if (existingTag == null)
                {
                    existingPerson.dialogues.Add(new MemoryTagsDialogueViewPair
                    {
                        memoryTags = tag,
                        dialogueResponses = new List<DialogueViewPair>
                    {
                        new DialogueViewPair
                        {
                            relationshipState = relationshipState,
                            dialogue = dialogue,
                            tagsInDialogue = tagsInDialogue,
                            relImpactOnMessenger = relImpactOnMessenger
                        }
                    }
                    });
                }
                else
                {
                    if (!existingTag.dialogueResponses.Any(d => d.relationshipState == relationshipState))
                    {
                        existingTag.dialogueResponses.Add(new DialogueViewPair
                        {
                            relationshipState = relationshipState,
                            dialogue = dialogue,
                            tagsInDialogue = tagsInDialogue,
                            relImpactOnMessenger = relImpactOnMessenger
                        });
                    }
                }
            }
        }
    }

    // Get dialogue response for a specific character, memory tag, and relationship state
    public string GetDialogue(Character character, Mind.MemoryTags memoryTag, ViewTowards relationshipState)
    {
        // Convert relationshipState to int
        int relationshipStateInt = (int)relationshipState;

        var characterDialogues = PeopleDialogues.ContainsKey(character) ? PeopleDialogues[character] : null;

        if (characterDialogues != null)
        {
            var memoryTagDialogue = characterDialogues.FirstOrDefault(view => view.memoryTags == memoryTag);
            if (memoryTagDialogue != null)
            {
                // Check for a response with relationshipState at or above the given value
                foreach (var response in memoryTagDialogue.dialogueResponses)
                {
                    // Debugging: log or inspect relationshipState and its integer value
                    int responseStateInt = (int)response.relationshipState;

                    // Check if the response relationship state is at or above the given value
                    if (relationshipStateInt>=responseStateInt )
                    {

                        return response.dialogue;
                    }
                }
            }
        }

        return null; // Return null if no dialogue is found for the memory tag and relationship state
    }

    // Remove a dialogue response for a specific character, memory tag, and relationship state
    public void RemoveDialogue(Character person, Mind.MemoryTags tag, ViewTowards relationshipState)
    {
        var existingPerson = _peopleDialogues.FirstOrDefault(p => p.character == person);
        if (existingPerson != null)
        {
            var tagEntry = existingPerson.dialogues.FirstOrDefault(d => d.memoryTags == tag);
            if (tagEntry != null)
            {
                tagEntry.dialogueResponses.RemoveAll(d => d.relationshipState == relationshipState);

                if (tagEntry.dialogueResponses.Count == 0)
                {
                    existingPerson.dialogues.Remove(tagEntry);
                }

                if (existingPerson.dialogues.Count == 0)
                {
                    _peopleDialogues.Remove(existingPerson);
                }
            }
        }
    }
}
