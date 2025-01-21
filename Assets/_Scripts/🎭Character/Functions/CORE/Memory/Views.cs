using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mind;


// Class to represent a memory tag and the corresponding view
[Serializable]
public class MemoryTagsFeelingsPair
{
    public Mind.MemoryTags memoryTags;  // Tag representing a memory
    public ViewTowards view;            // View or feeling towards that memory tag
}

[Serializable]
public class CharacterMemoryViewsPair
{
    public Character character;                             // The character
    public List<MemoryTagsFeelingsPair> views = new(); // List of memory tag-feeling pairs
}

public class Views : MonoBehaviour
{
    [SerializeField]
    private List<CharacterMemoryViewsPair> _peopleViews = new(); // Nested structure

    // Expose the PeopleViews as a Dictionary of Character to List of MemoryTagsFeelingsPair
    public Dictionary<Character, List<MemoryTagsFeelingsPair>> PeopleViews
    {
        get
        {
            Dictionary<Character, List<MemoryTagsFeelingsPair>> dictionary = new();
            foreach (var pair in _peopleViews)
            {
                dictionary[pair.character] = pair.views;
            }
            return dictionary;
        }
    }

    // Add a character's view towards a list of memory tags
    public void AddPersonView(Character person, List<Mind.MemoryTags> tags, ViewTowards view)
    {
        var existingPerson = _peopleViews.FirstOrDefault(p => p.character == person);

        if (existingPerson == null)
        {
            _peopleViews.Add(new CharacterMemoryViewsPair
            {
                character = person,
                views = tags.Select(tag => new MemoryTagsFeelingsPair
                {
                    memoryTags = tag,
                    view = view
                }).ToList()
            });
        }
        else
        {
            foreach (var tag in tags)
            {
                if (!existingPerson.views.Exists(t => t.memoryTags == tag))
                {
                    existingPerson.views.Add(new MemoryTagsFeelingsPair
                    {
                        memoryTags = tag,
                        view = view
                    });
                }
            }
        }
    }

    // Update the view of a person towards a specific tag
    public void UpdatePersonViewFeeling(Character person, Mind.MemoryTags tag, ViewTowards view)
    {
        var existingPerson = _peopleViews.FirstOrDefault(p => p.character == person);

        if (existingPerson != null)
        {
            var tagPair = existingPerson.views.FirstOrDefault(t => t.memoryTags == tag);
            if (tagPair != null)
            {
                tagPair.view = view;
            }
        }
    }

    // Get a list of people who have a specific view towards a set of tags
    public List<Character> GetPeopleByFeelingTowardsTag(ViewTowards view, params Mind.MemoryTags[] tags)
    {
        return _peopleViews
            .Where(p => p.views.Any(t => tags.Contains(t.memoryTags) && t.view == view))
            .Select(p => p.character)
            .ToList();
    }

    public ViewTowards? GetView(Character character, Mind.MemoryTags memoryTag)
    {
        // Check if the character exists in the views dictionary
        var characterViews = PeopleViews.ContainsKey(character) ? PeopleViews[character] : null;

        if (characterViews != null)
        {
            // Find the view for the specific memory tag
            var memoryTagView = characterViews.FirstOrDefault(view => view.memoryTags == memoryTag);

            // If the memory tag view is found, return the view
            if (memoryTagView != null)
            {
                return memoryTagView.view; 
            }
        }

        // Return null if no view for the memory tag is found
        return null;
    }
}

