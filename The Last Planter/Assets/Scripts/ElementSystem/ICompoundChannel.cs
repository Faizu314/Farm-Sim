using UnityEngine;
using System.Collections.Generic;
public interface ICompoundChannel
{
    public List<float> GetContent();
    public void UpdateContent(List<float> deltaContent);
}
