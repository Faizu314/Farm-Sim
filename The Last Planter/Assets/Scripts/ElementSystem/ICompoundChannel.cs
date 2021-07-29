using UnityEngine;
using System.Collections.Generic;
public interface ICompoundChannel
{
    public List<float> GetContent();
    public float GetContent(int index);
    public void UpdateContent(List<float> deltaContent);
    public void UpdateContent(int index, float deltaContent);
}