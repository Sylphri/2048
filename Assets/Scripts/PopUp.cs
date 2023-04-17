using UnityEngine;
using DG.Tweening;
using TMPro;

public class PopUp : MonoBehaviour
{ 
    public float Height { get; set; }
    public float LifeTime { get; set; }
    
    private void Start()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        DOTween.To(()=> text.alpha, x => text.alpha = x, 0, LifeTime);
        transform.DOMove(transform.position + Vector3.up * Height, LifeTime)
            .OnComplete(() => {
                Destroy(gameObject);    
            });
    }
}
