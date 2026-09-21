using UnityEngine;

[DisallowMultipleComponent]
public class DoughPortion : MonoBehaviour
{
    [SerializeField] private string batchId;
    [SerializeField, Min(0)] private int portionIndex;
    [SerializeField, Min(1)] private int totalPortions = 1;
    [SerializeField, Min(0f)] private float sourceBatchVolume;

    public string BatchId => batchId;
    public int PortionIndex => portionIndex;
    public int TotalPortions => totalPortions;
    public float SourceBatchVolume => sourceBatchVolume;

    public void Configure(
        string newBatchId,
        int newPortionIndex,
        int newTotalPortions,
        float newSourceBatchVolume)
    {
        batchId = string.IsNullOrWhiteSpace(newBatchId)
            ? System.Guid.NewGuid().ToString("N")
            : newBatchId;
        totalPortions = Mathf.Max(1, newTotalPortions);
        portionIndex = Mathf.Clamp(
            newPortionIndex,
            0,
            totalPortions - 1);
        sourceBatchVolume = Mathf.Max(0f, newSourceBatchVolume);
    }
}
