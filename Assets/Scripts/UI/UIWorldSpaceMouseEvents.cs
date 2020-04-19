using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIWorldSpaceMouseEvents : MonoBehaviour
{
	public GraphicRaycaster raycaster;
	public EventSystem eventSystem;

	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		var pointerData = new PointerEventData(eventSystem)
		{
			position = Input.mousePosition,
			scrollDelta = Input.mouseScrollDelta
		};
		var hits = new List<RaycastResult>();
		var eventData = new BaseEventData(eventSystem);
		ExecuteEvents.Execute(raycaster.gameObject, eventData, ExecuteEvents.pointerEnterHandler);

		raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD;
		raycaster.Raycast(pointerData, hits);
		for (int i = 0; i < hits.Count; i++)
		{
			DebugUtilz.DrawCrosshair(hits[i].worldPosition, 0.1f, Color.red, 0);
			//Debug.Log(hits[i]);
			//eventSystem.SetSelectedGameObject(hits[i].gameObject);
			//hits[i].gameObject.SendMessage("OnPointerEnter", pointerData, SendMessageOptions.DontRequireReceiver);
			//hits[i].gameObject.SendMessage("OnScroll", pointerData, SendMessageOptions.DontRequireReceiver);
		}
	}
}
