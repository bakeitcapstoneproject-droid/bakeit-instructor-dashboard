using UnityEngine;

public sealed class CupcakeDonenessProbe : MonoBehaviour
{
    [SerializeField] private GameObject stickyTip;
    public Vector3 TipPosition => transform.position;
    public void ShowResult(bool sticky) { if (stickyTip) stickyTip.SetActive(sticky); }
}
