using UnityEngine;

//thanks to professor Waren for helping with this.
public class OutlineToggle : MonoBehaviour
{
    // Which rendering layer index to use (set this to match your "Outline_1" index)
    // For example, Outline_1 is usually bit 1, Outline_2 = bit 2, etc.
    public int outlineLayer = 1;

    private Renderer rend;
    private uint outlineMask;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        outlineMask = 1u << outlineLayer; // convert index to bitmask
    }

    public void SetHighlighted(bool enabled)
    {
        if (enabled)
        {
            rend.renderingLayerMask |= outlineMask;   // add layer bit
        }
        else
        {
            rend.renderingLayerMask &= ~outlineMask;  // remove layer bit
        }
    }
}
