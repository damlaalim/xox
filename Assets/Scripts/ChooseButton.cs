using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseButton : MonoBehaviour, IPointerDownHandler
{
    public TileState myState;

    [SerializeField] private Animator chooseSectionAnimator;
    private static readonly int ChooseX = Animator.StringToHash("chooseX");
    private static readonly int ChooseO = Animator.StringToHash("chooseO");
    
    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(Do());
        
        IEnumerator Do()
        {
            AudioManager.Instance.Play(AudioType.ChooseState);
            chooseSectionAnimator.SetTrigger(myState == TileState.X ? ChooseX : ChooseO);

            yield return new WaitForSeconds(1);
            GameManager.Instance.OnChoose(myState);
        }
    }
}