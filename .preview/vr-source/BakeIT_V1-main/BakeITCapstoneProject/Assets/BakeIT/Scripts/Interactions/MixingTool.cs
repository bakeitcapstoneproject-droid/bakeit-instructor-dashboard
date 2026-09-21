using UnityEngine;

public class MixingTool : MonoBehaviour
{
    [SerializeField] private MixingAction action;
    public MixingAction Action => action;
}
