using UnityEngine;
using Yakanashe.Yautl;

public class TouchIndicator : MonoBehaviour
{
    public SpriteRenderer IndicatorRenderer;

    private readonly Color _clearColor = new(1, 1, 1, 0);
    private readonly Vector3 offWorldPosition = new(-200, 100, -100);

    private void Awake()
    {
        transform.localScale = Vector3.one;
        transform.position = offWorldPosition;
        IndicatorRenderer.color = Color.clear;
    }

    public void Highlight(Node node)
    {
        TweenRunner.Instance.KillAllFrom(transform);
        
        transform.localScale = Vector3.one;
        transform.position = node.Position;
        IndicatorRenderer.color = _clearColor;

        IndicatorRenderer.ColorTo(Color.white, 0.3f, EaseType.OutCubic);
        transform.ScaleTo(new Vector3(0.7f, 0.7f, 0.7f), 0.3f, EaseType.OutCubic).OnComplete(() =>
        {
            transform.ScaleTo(new Vector3(0.7f, 0.7f, 0.7f), 1f).OnComplete(() =>
            {
                IndicatorRenderer.ColorTo(_clearColor, 0.3f, EaseType.InCubic);
                transform.ScaleTo(new Vector3(0.4f, 0.4f, 0.4f), 0.3f, EaseType.InCubic).OnComplete(() =>
                {
                    transform.position = offWorldPosition;
                });
            });
        });
    }
}
