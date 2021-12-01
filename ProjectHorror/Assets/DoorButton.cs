using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorButton : MonoBehaviour
{
    [HideInInspector]
    public bool hasBeenActived = false;

    [SerializeField] private string identifier = "";
    [SerializeField] private float moveDistance;
    [SerializeField] private float buttonPressDuration = 1;

    private bool isPressed = false;

    public void PressButton()
    {
        if (!isPressed)
        {
            StartCoroutine(StartButtonPress());
        }
        else return;
    }

    private IEnumerator StartButtonPress()
    {
        isPressed = true;

        AudioManager.instance.Play("buttonpress");
        DoorController.instance.ButtonHasBeenPressed(identifier);
        LeanTween.move(this.gameObject, new Vector3(transform.position.x + moveDistance, transform.position.y, transform.position.z), buttonPressDuration / 2).setEaseInOutBack();

        yield return new WaitForSeconds(buttonPressDuration / 2);

        LeanTween.move(this.gameObject, new Vector3(transform.position.x - moveDistance, transform.position.y, transform.position.z), buttonPressDuration / 2).setEaseInOutBack();
        yield return new WaitForSeconds(buttonPressDuration / 2);

        isPressed = false;
    }

    [SerializeField] string hoverText;
    [SerializeField] OnInteractEvent OnInteract = new OnInteractEvent();

    public string GetHoverText() => hoverText;

    public void Interact(PlayerInput player) => OnInteract.Invoke(player);

    [System.Serializable]
    public class OnInteractEvent : UnityEvent<PlayerInput> { }
}
