using UnityEngine;
using System;


/// <Optimization>
/// There should be one global OnClickEventHandler and every script will register to it
/// a function that takes the hit transform as the parameter. This way only one rayCast will
/// be done instead of x amount. x => no. of plant elements in the scene.
/// <Optimization>


public class OnClickEvent : MonoBehaviour
{
    private Action onClickEvent;
    private Camera mainCamera;
    private Ray ray;

    private void Start()
    {
        mainCamera = Camera.main;
        ray = new Ray();
        Physics.queriesHitBackfaces = true;
    }

    public void RegisterClickEvent(Action onClickEvent)
    {
        this.onClickEvent = onClickEvent;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit))
            {
                if (rayHit.transform == transform)
                {
                    onClickEvent?.Invoke();
                }
            }
        }
    }
}
