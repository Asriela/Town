using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mind;

// Class to represent a memory tag and its related sub-tags
[Serializable]
public class MemoryTagsGeneralizationPair
{
    public Mind.MemoryTags primeTag;           // Prime tag
    public List<Mind.MemoryTags> subTags = new(); // List of sub-tags related to the prime tag
}

public class Generalizations : MonoBehaviour
{
    [SerializeField]
    private List<MemoryTagsGeneralizationPair> _generalizations = new(); // List of generalizations

    // Expose the Generalizations as a List of MemoryTagsGeneralizationPair
    public List<MemoryTagsGeneralizationPair> GeneralizationsList
    {
        get { return _generalizations; }
    }

    // Add a new generalization (primeTag and subTags)
    public void AddGeneralization(Mind.MemoryTags primeTag, List<Mind.MemoryTags> subTags)
    {
        var existingGeneralization = _generalizations.FirstOrDefault(g => g.primeTag == primeTag);

        if (existingGeneralization == null)
        {
            _generalizations.Add(new MemoryTagsGeneralizationPair
            {
                primeTag = primeTag,
                subTags = new List<Mind.MemoryTags>(subTags)
            });
        }
        else
        {
            existingGeneralization.subTags.AddRange(subTags.Where(subTag => !existingGeneralization.subTags.Contains(subTag)));
        }
    }

    // Method to check if a sub-tag is under a prime tag
    public bool IsTagA(Mind.MemoryTags subTag, Mind.MemoryTags primeTag)
    {
        var generalization = _generalizations.FirstOrDefault(g => g.primeTag == primeTag);
        return generalization != null && generalization.subTags.Contains(subTag);
    }

    // Get all sub-tags for a specific prime tag
    public List<Mind.MemoryTags> GetSubTags(Mind.MemoryTags primeTag)
    {
        var generalization = _generalizations.FirstOrDefault(g => g.primeTag == primeTag);
        return generalization?.subTags ?? new List<Mind.MemoryTags>();
    }
}
